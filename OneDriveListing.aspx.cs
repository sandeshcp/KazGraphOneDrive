using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Dapper;
using KazGraph;
using KazGraph.DataServices;
using KazGraph.Models;
using KazGraph.Service;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using RestSharp;
using Microsoft.Graph.Auth;
using ClientCredential = Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential;
using AuthenticationContext = Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext;
using System.Net.Http;
using System.IO;
using Microsoft.Graph.CallRecords;
using Microsoft.Ajax.Utilities;
using ListItem = System.Web.UI.WebControls.ListItem;
using System.Data.SqlClient;
using Azure.Core;
using System.Configuration;
using System.Data.Common;
using System.Collections;
using KazGraph.BAL;
using Microsoft.Graph.ExternalConnectors;

namespace KazGraph
{
    public partial class OneDriveListing : System.Web.UI.Page
    {
        #region PrivateKeyValue
        private static string MSGraphTokenURL = WebConfigurationManager.AppSettings["MSGraphTokenURL"];
        private static string TenantID = WebConfigurationManager.AppSettings["ADTenantID"];
        private static string GrantType = WebConfigurationManager.AppSettings["ADGrantType"];
        private static string ADClientID = WebConfigurationManager.AppSettings["ADClientID"];
        private static string ADClientSecret = WebConfigurationManager.AppSettings["ADClientSecret"];
        private static string ADResource = WebConfigurationManager.AppSettings["ADResource"];
        private static string ADGraphOneDrive = WebConfigurationManager.AppSettings["ADGraphOneDrive"];
        private static string ADGraphUser = WebConfigurationManager.AppSettings["ADGraphUser"];
        private static string ADScopes = WebConfigurationManager.AppSettings["ADScopes"];

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                #region DDLTenantbind
                GetTenantList();
                #endregion

                if (!string.IsNullOrWhiteSpace(Request.QueryString["name"]))
                {
                    //var Data_session = new SessionModel();
                    List<clsOneDriveRootValue> obj = new List<clsOneDriveRootValue>();
                    //TokenGenerate();
                    obj = BindGridList(Request.QueryString["name"]);
                    gvOneDriveItem.DataSource = obj;
                    gvOneDriveItem.DataBind();
                }
                else
                {
                    //var Data_session = new SessionModel();
                    List<clsOneDriveRootValue> obj = new List<clsOneDriveRootValue>();
                    obj = BindGridList("/drive/root:");
                    gvOneDriveItem.DataSource = obj;
                    gvOneDriveItem.DataBind();
                }
            }
        }

        private void DDLUserBind()
        {
            try
            {
                var objuser = string.Empty;
                Task t1 = Task.Run(async () =>
                {
                    objuser = JsonConvert.SerializeObject(await getUserList().ConfigureAwait(false));
                });
                t1.Wait();
                if (objuser != "null")
                {
                    List<ListItem> users = new List<ListItem>
                    {
                        new ListItem("All", "0")
                    };
                    foreach (clsUserDetails su in JsonConvert.DeserializeObject<List<clsUserDetails>>(objuser))
                    {
                        users.Add(new ListItem(su.displayName, su.id));
                    }
                    ddlUser.DataTextField = "Text";
                    ddlUser.DataValueField = "Value";
                    ddlUser.DataSource = users;
                    ddlUser.DataBind();
                    ListItem LICountry = new ListItem("----Select----", "-1");
                    ddlUser.Items.Insert(0, LICountry);

                    if (Session["ddluserSelected"] != null)
                    {
                        ddlUser.SelectedValue = Convert.ToString(Session["ddluserSelected"]);
                        ddlActionTrigger();
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("DdlUser Binding Method: " + ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        private void TokenGenerate()
        {
            if (!string.IsNullOrWhiteSpace(Convert.ToString(HttpContext.Current.Session["dllTenentSelected"])))
            {
                if (string.IsNullOrWhiteSpace((string)Session["Token"]))
                {
                    var TokenID = string.Empty;
                    Task t1 = Task.Run(async () =>
                    {
                        TokenID = JsonConvert.SerializeObject(await GetGraphAccessToken().ConfigureAwait(false));
                    });
                    t1.Wait();
                    if (TokenID != "null")
                    {
                        Session["Token"] = JsonConvert.DeserializeObject<GraphAuthTokenResponse>(TokenID.ToString()).access_token;
                    }
                }
            }
        }
        private List<clsOneDriveRootValue> BindGridList(string query)
        {
            #region Code Connections
            string userid = string.Empty;
            string actiontrigger = string.Empty;
            string AzureConnectionID = string.Empty;

            if (Session["Token"] == null)
            {
                TokenGenerate();
            }
            if (Session["ddluserSelected"] != null)
            {
                userid = Convert.ToString(Session["ddluserSelected"]);
            }
            if (Session["ddlActionSelected"] != null)
            {
                actiontrigger = Convert.ToString(Session["ddlActionSelected"]);
            }

            if (Convert.ToString(Session["AzureConnectionID"]) != null)
            {
                AzureConnectionID = Convert.ToString(Session["AzureConnectionID"]);
            }
            if (!string.IsNullOrWhiteSpace(userid))
            {
                if (actiontrigger != null)
                {
                    List<clsOneDriveRootValue> te = new List<clsOneDriveRootValue>();
                    if (actiontrigger == "2") //to db store
                    {
                        OneDriveBal obj = new OneDriveBal();
                        Task t11 = Task.Run(async () =>
                        {
                            var sam = JsonConvert.SerializeObject(await GetGraphAccessOneDrive(Convert.ToString(Session["Token"]), userid).ConfigureAwait(false));
                            te = JsonConvert.DeserializeObject<List<clsOneDriveRootValue>>(sam);

                            var sam2 = JsonConvert.SerializeObject(await obj.InsertItem(te, AzureConnectionID).ConfigureAwait(false));
                            //te = JsonConvert.DeserializeObject<List<clsOneDriveRootValue>>(sam2);
                        });
                        t11.Wait();
                        #endregion
                        return te?.Where(x => x.parentReference.path == query).OrderByDescending(x => x.createdDateTime).ToList();
                    }
                    else if (actiontrigger == "1") //Get From db store
                    {
                        OneDriveBal obj = new OneDriveBal();
                        Task t22 = Task.Run(async () =>
                        {
                            var sam = JsonConvert.SerializeObject(await obj.SelectItem(userid, AzureConnectionID).ConfigureAwait(false));
                            te = JsonConvert.DeserializeObject<List<clsOneDriveRootValue>>(sam);
                        });
                        t22.Wait();
                        return te?.Where(x => x.parentReference.path == query).OrderByDescending(x => x.createdDateTime).ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
                return null;
            }
            else
                return null;
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string customerId = gvOneDriveItem.DataKeys[e.Row.RowIndex].Value.ToString();
                //string sam= gvOneDriveItem.Rows[e.Row.RowIndex].Cells[4].Text;
                string name = e.Row.Cells[8].Text;
                HyperLink TestLabel = (HyperLink)e.Row.FindControl("HyperLink1");
                string FullName = TestLabel.Text;
                GridView gvOneDriveItemChild = e.Row.FindControl("gvOneDriveItemChild") as GridView;
                gvOneDriveItemChild.DataSource = BindGridList(string.Format("{0}/{1}", name, FullName));
                gvOneDriveItemChild.DataBind();
            }
        }

        public async Task<GraphAuthTokenResponse> GetGraphAccessToken()
        {
            string res = string.Empty;
            GraphAuthTokenResponse myDeserializedClass = null;
            try
            {
                string _TenentSelected = Convert.ToString(Session["dllTenentSelected"]);
                if (_TenentSelected != null)
                {
                    //var anInstanceofMyClass = new OneDriveListing();
                    //var TenantDetails = await anInstanceofMyClass.GetTenantDetails(_TenentSelected).ConfigureAwait(false);                    
                    //var anInstanceofMyClass = new OneDriveListing();
                    var TenantDetails = await GetTenantDetails(_TenentSelected).ConfigureAwait(false);
                    #region GraphAccessToken
                    var url = MSGraphTokenURL + TenantDetails.TenantID + "/oauth2/token";
                    var dict = new Dictionary<string, string>
                {
                    { "Content-Type", "application/x-www-form-urlencoded" },
                    { "grant_type", TenantDetails.GrantType },
                    { "client_id", TenantDetails.ClientID},
                    { "client_secret", TenantDetails.ClientSecret},
                    { "resource", TenantDetails.Resource }
                };

                    using (var client = new HttpClient())
                    {
                        var req = new HttpRequestMessage(HttpMethod.Post, url)
                        {
                            Content = new FormUrlEncodedContent(dict)
                        };
                        await client.SendAsync(req)
                               .ContinueWith(responseTask =>
                               {
                                   if (responseTask.Result.IsSuccessStatusCode)
                                   {
                                       myDeserializedClass = JsonConvert.DeserializeObject<GraphAuthTokenResponse>(Convert.ToString(responseTask.Result.Content.ReadAsStringAsync().Result));
                                   }
                               });
                    }

                    #endregion
                }


                return myDeserializedClass;
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("Authentication GetGraphAccessToken Method: " + ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Authentication GetGraphAccessToken Method: " + ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        private static async Task<List<clsOneDriveRootValue>> GetGraphAccessOneDrive(string AccessToken, string UserID)
        {
            #region MyRegion

            #endregion
            string res = string.Empty;
            List<clsOneDriveRootValue> myDeserializedClass = null;
            try
            {
                #region GraphAccessOneDrive
                var url = ADGraphOneDrive.Replace("UserID", UserID).Replace("ADResource", ADResource);
                string output = string.Empty;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    await client.SendAsync(req)
                           .ContinueWith(responseTask =>
                           {
                               if (responseTask.Result.IsSuccessStatusCode)
                               {
                                   output = responseTask.Result.Content.ReadAsStringAsync().Result;

                               }
                           });
                }

                #endregion
                if (!string.IsNullOrWhiteSpace(output))
                    myDeserializedClass = JsonConvert.DeserializeObject<clsOneDriveRoot>(JsonConvert.DeserializeObject(output).ToString()).value;
                return myDeserializedClass;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Authentication GetGraphAccessToken Method: " + ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        private async Task<List<clsUserDetails>> getUserList()
        {
            string res = string.Empty;
            List<clsUserDetails> myDeserializedClass = null;
            try
            {
                if (Session["Token"] == null)
                {
                    TokenGenerate();
                }
                #region GetUserList

                string output = string.Empty;
                var url = ADGraphUser.Replace("ADResource", ADResource);
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Convert.ToString(Session["Token"]));
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    await client.SendAsync(req)
                           .ContinueWith(responseTask =>
                           {
                               if (responseTask.Result.IsSuccessStatusCode)
                               {
                                   output = responseTask.Result.Content.ReadAsStringAsync().Result;
                               }
                           });
                }
                #endregion
                if (!string.IsNullOrWhiteSpace(output))
                    myDeserializedClass = JsonConvert.DeserializeObject<clsUserDetailsRoot>(JsonConvert.DeserializeObject(output).ToString()).value;
                return myDeserializedClass;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Authentication GetGraphAccessToken Method: " + ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }



        public void GetTenantList()
        {
            try
            {
                OneDriveBal objt = new OneDriveBal();
                dllTenent.DataTextField = "AzureConnectionName";
                dllTenent.DataValueField = "AzureConnectionID";
                dllTenent.DataSource = objt.GetTenantList();
                dllTenent.DataBind();

                var LICountry = new ListItem("----Select----", "-1");
                dllTenent.Items.Insert(0, LICountry);

                if (Session["dllTenentSelected"] != null)
                {
                    dllTenent.SelectedValue = Convert.ToString(Session["dllTenentSelected"]);
                    DDLUserBind();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + ex.Message + "')", true);
            }
        }

        public async Task<clsTenantList> GetTenantDetails(string id)
        {
            try
            {

                OneDriveBal objt = new OneDriveBal();
                var azuretenantdetails = await objt.GetTenantDetails(id);
                Session["AzureConnectionID"] = azuretenantdetails.AzureConnectionID;
                return azuretenantdetails;
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + ex.Message + "')", true);
            }
            return null;
        }

        protected void dllTenent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dllTenent.SelectedValue == "-1")
            {
                gridReset();
            }
            else
            {
                gridReset();
                //var sam1 = GetTenantDetails(Convert.ToString(dllTenent.SelectedValue));
                var selectName = dllTenent.SelectedValue;
                Session["dllTenentSelected"] = dllTenent.SelectedValue;
                Session["Token"] = string.Empty;
                TokenGenerate();
                #region DDlUserBind
                DDLUserBind();
                #endregion
                //return accessToken;

                #region WasteToken
                ////var tenantId = "27d1c216-24…";

                //// var clientId = "877edd8a-5a…";
                //var sam1 = GetTenantDetails(Session["dllTenentSelected"].ToString());
                //var authorityUri = $"https://login.microsoftonline.com/{sam1.TenantID}";
                //var redirectUri = "https://localhost:44342/OneDriveListing";
                //var scopes = new List<string> { "Files.Read", "Files.Read.All", "Files.ReadWrite", "Files.ReadWrite.All", "Sites.Read.All", "Sites.ReadWrite.All" };

                //string redirectUri = "https://myapp.azurewebsites.net";
                //IConfidentialClientApplication publicClient = ConfidentialClientApplicationBuilder.Create(sam1.ClientID)
                //    .WithClientSecret(sam1.ClientSecret)
                //    .WithRedirectUri(redirectUri)
                //    .Build();



                //var publicClient = PublicClientApplicationBuilder
                //              .Create(sam1.ClientID)
                //              .WithAuthority(new Uri(authorityUri))
                //              .WithRedirectUri(redirectUri)
                //              .Build();

                //object output = string.Empty;
                //publicClient.GetAccountsAsync();

                ////var accessToken = accessTokenRequest.ExecuteAsync().Result.AccessToken;



                //var objuser = string.Empty;
                //Task t1 = Task.Run(async () =>
                //{
                //    //var accessTokenRequest = publicClient.AcquireTokenByUsernamePassword(scopes, "projectdeployee@gmail.com", "NewYear2023!").ExecuteAsync().ContinueWith(responseTask =>
                //    var accessTokenRequest = publicClient.AcquireTokenInteractive(scopes).ExecuteAsync().ContinueWith(responseTask =>
                //    {
                //        if (responseTask.IsCompleted)
                //        {
                //            output = responseTask.Result;

                //        }
                //    });
                //    //objuser = JsonConvert.SerializeObject(await getUserList().ConfigureAwait(false));
                //});
                //t1.Wait();
                //var sam = output.ToString();
                #endregion
            }
        }

        protected void ddlUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridReset();
            if (ddlUser.SelectedValue == "-1")
            {

            }
            else
            {
                var selectName = ddlUser.SelectedValue;
                Session["ddluserSelected"] = ddlUser.SelectedValue;
            }
        }

        protected void ddlAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAction.SelectedValue == "-1")
            {
                gridReset();
            }
            else
            {
                List<clsOneDriveRootValue> obj = new List<clsOneDriveRootValue>();
                var selectName = ddlAction.SelectedValue;
                Session["ddlActionSelected"] = ddlAction.SelectedValue;

                if (!string.IsNullOrWhiteSpace(Request.QueryString["name"]))
                {
                    obj = BindGridList(Request.QueryString["name"]);
                    gvOneDriveItem.DataSource = obj;
                    gvOneDriveItem.DataBind();
                }
                else
                {
                    obj = BindGridList("/drive/root:");
                    gvOneDriveItem.DataSource = obj;
                    gvOneDriveItem.DataBind();
                }
            }
        }

        public void gridReset()
        {
            List<clsOneDriveRootValue> obj = new List<clsOneDriveRootValue>();
            gvOneDriveItem.DataSource = obj;
            gvOneDriveItem.DataBind();
            ddlActionTrigger();
        }

        public void ddlActionTrigger()
        {
            try
            {
                OneDriveBal objt = new OneDriveBal();
                ddlAction.DataTextField = "Text";
                ddlAction.DataValueField = "Value";
                ddlAction.DataSource = objt.GetActionList();
                ddlAction.DataBind();
                if (string.IsNullOrWhiteSpace(Request.QueryString["name"]))
                {
                    ddlAction.SelectedValue = "-1";
                    Session["ddlActionSelected"] = ddlAction.SelectedValue;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + ex.Message + "')", true);
            }
        }
    }
}