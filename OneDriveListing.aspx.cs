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
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                #region DDLTenantbind
                GetTenantList();
                #endregion
                TokenGenerate();
                #region DDlUserBind
                DDLUserBind();
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

                List<ListItem> users = new List<ListItem>();
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
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DdlUser Binding Method: " + ex.Message + " " + ex.StackTrace.ToString());
            }
        }

        private void TokenGenerate()
        {
            if (string.IsNullOrWhiteSpace((string)Session["Token"]))
            {
                var TokenID = string.Empty;
                Task t1 = Task.Run(async () =>
                {
                    TokenID = JsonConvert.SerializeObject(await GetGraphAccessToken().ConfigureAwait(false));
                });
                t1.Wait();
                Session["Token"] = JsonConvert.DeserializeObject<GraphAuthTokenResponse>(TokenID.ToString()).access_token;
            }
        }
        private List<clsOneDriveRootValue> BindGridList(string query)
        {
            #region Code Connections
            string userid = "550dc14d-db6a-4ca3-bfd4-d47f11fba852";
            if (Session["Token"] == null)
            {
                TokenGenerate();
            }
            if (Session["ddluserSelected"] != null)
            {
                userid = Convert.ToString(Session["ddluserSelected"]);
            }

            List<clsOneDriveRootValue> te = new List<clsOneDriveRootValue>();
            Task t2 = Task.Run(async () =>
            {
                var sam = JsonConvert.SerializeObject(await GetGraphAccessOneDrive(Convert.ToString(Session["Token"]), userid).ConfigureAwait(false));
                te = JsonConvert.DeserializeObject<List<clsOneDriveRootValue>>(sam);
            });
            t2.Wait();
            #endregion
            return te?.Where(x => x.parentReference.path == query).OrderByDescending(x => x.createdDateTime).ToList();
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

        public static async Task<GraphAuthTokenResponse> GetGraphAccessToken()
        {
            string res = string.Empty;
            GraphAuthTokenResponse myDeserializedClass = null;
            try
            {
                #region GraphAccessToken
                var url = MSGraphTokenURL + TenantID + "/oauth2/token";
                var dict = new Dictionary<string, string>
                {
                    { "Content-Type", "application/x-www-form-urlencoded" },
                    { "grant_type", GrantType },
                    { "client_id", ADClientID },
                    { "client_secret", ADClientSecret },
                    { "resource", ADResource }
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
                return myDeserializedClass;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Authentication GetGraphAccessToken Method: " + ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }

        private static async Task<List<clsOneDriveRootValue>> GetGraphAccessOneDrive(string AccessToken, string UserID)
        {
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

        protected void ddlUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlUser.SelectedValue == "-1")
            {

            }
            else
            {
                var selectName = ddlUser.SelectedValue;
                Session["ddluserSelected"] = ddlUser.SelectedValue;
                List<clsOneDriveRootValue> obj = new List<clsOneDriveRootValue>();
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

        public void GetTenantList()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["KazooDB"].ConnectionString);
            string com = "Select * from AzureConnection";
            SqlDataAdapter adpt = new SqlDataAdapter(com, con);
            DataTable dt = new DataTable();
            adpt.Fill(dt);
            dllTenent.DataTextField = "AzureConnectionName";
            dllTenent.DataValueField = "AzureConnectionID";
            dllTenent.DataSource = dt;
            dllTenent.DataBind();            
            
            var LICountry = new ListItem("----Select----", "-1");
            dllTenent.Items.Insert(0, LICountry);
        }
    }
}