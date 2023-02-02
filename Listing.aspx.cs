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

namespace KazGraph
{
    public partial class Listing : System.Web.UI.Page
    {
        //private IMSGraphServices _mSGraphServices;
        //private IAuthenticationService _authenticationService;
        private static string MSGraphTokenURL = WebConfigurationManager.AppSettings["MSGraphTokenURL"];
        private static string TenantID = WebConfigurationManager.AppSettings["ADTenantID"];
        private static string GrantType = WebConfigurationManager.AppSettings["ADGrantType"];
        private static string ADClientID = WebConfigurationManager.AppSettings["ADClientID"];
        private static string ADClientSecret = WebConfigurationManager.AppSettings["ADClientSecret"];
        private static string ADResource = WebConfigurationManager.AppSettings["ADResource"];
        private static string ADGraphOneDrive = WebConfigurationManager.AppSettings["ADGraphOneDrive"];


        //public Listing(IMSGraphServices mSGraphServices)
        //{
        //    _mSGraphServices = mSGraphServices;
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                    BindGridList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OrganizationsUsersList Method: " + ex.Message + " " + ex.StackTrace.ToString());
                //return result;
            }
        }


        protected void btnGetOneDriveFileListing_Click(object sender, EventArgs e)
        {
            lblGetOneDriveFileListing.Text = "Connect to OneDrive and load the data into screen table";
            #region Info
            //Microsoft Graph Dev center   https://developer.microsoft.com/en-us/graph
            //Login:  di...@crm....   Ol@7  registered on test app   ClientID=dd81a74f-ed6d-42a4-9e8a-ea4cc710d8b3    consented to security roles, now working
            //Login  dsch@h...   DSh...6     this works in test app
            //Login di@out...  Cr@5      need to grant permissions   https://learn.microsoft.com/en-us/graph/api/driveitem-list-children?view=graph-rest-1.0&tabs=http   there is a video showing how to grant consent
            // permissions Files.Read, Files.Read.All, Sites.Read.All   Granted in the Azure portal:  https://portal.azure.com/#home    
            // OAuth permissions, application permissions gives all data (or delegated permissions which is user based permissions)
            //application permissions, Admin needs to explicitly grant consent (permissions)
            //in azure portal, go to myapplication - API permissions

            //Sample core app:  https://learn.microsoft.com/en-us/samples/microsoftgraph/msgraph-sample-aspnet-core/microsoft-graph-sample-aspnet-core-app/
            //sample code:  https://github.com/microsoftgraph/msgraph-sample-aspnet-core/tree/main/

            //https://graph.microsoft.com/v1.0/me/drive/root/children

            // Microsoft Graph Tool:   https://developer.microsoft.com/en-us/graph/graph-explorer
            #endregion

            BindGridList();
            lblGetOneDriveFileListing.Text = "Connected to OneDrive.";
        }

        protected void btnPopulateTable_Click(object sender, EventArgs e)
        {
            lblPopulateTable.Text = "Populate Table control below with data from db Kazoo table FileList ";

            //TreeviewTest1
            {
                //populate treeview
                //TreeNode treenode1 = new TreeNode("Treenode", "TreenodeValue");
                //TreeView1.Nodes.Add(treenode1);
            }

            //TreeView Test2
            {
                //TreeNode tNode;
                //tNode = TreeView1.Nodes.Add(new TreeNode("Websites"));


                TreeView1.Nodes.Add(new TreeNode("Websites"));
                TreeView1.Nodes[0].ChildNodes.Add(new TreeNode("Net-informations.com"));
                TreeView1.Nodes[0].ChildNodes[0].ChildNodes.Add(new TreeNode("CLR"));

                TreeView1.Nodes[0].ChildNodes.Add(new TreeNode("Vb.net-informations.com"));
                TreeView1.Nodes[0].ChildNodes[1].ChildNodes.Add(new TreeNode("String Tutorial"));
                TreeView1.Nodes[0].ChildNodes[1].ChildNodes.Add(new TreeNode("Excel Tutorial"));

                TreeView1.Nodes[0].ChildNodes.Add(new TreeNode("Csharp.net-informations.com"));
                TreeView1.Nodes[0].ChildNodes[2].ChildNodes.Add(new TreeNode("ADO.NET"));
                TreeView1.Nodes[0].ChildNodes[2].ChildNodes[0].ChildNodes.Add(new TreeNode("Dataset"));

                TreeView1.ExpandAll();
            }

            return;

            //get connection
            //System.Configuration.ConnectionStringSettingsCollection connectionStrings = System.Web.Configuration.WebConfigurationManager.ConnectionStrings as System.Configuration.ConnectionStringSettingsCollection;

            ////lblPopulateTable.Text =  connectionStrings[1].Name.ToString() + " :  " + connectionStrings[1].ConnectionString.ToString();
            //string databaseconnection = connectionStrings[1].ConnectionString.ToString();






            //List<GetFileList> fileList1 = new List<GetFileList>();

            //DataService dataservice1 = new DataService();
            //dataservice1.ReturnFileListfromDB(ref fileList1);

            //TreeNode treenode1 = new TreeNode("Treenode", "TreenodeValue");
            //int filelistcount = fileList1.Count;
            //for (int i = 0; i < filelistcount; i++)
            //{
            //    GetFileList item1 = fileList1[i];
            //    string treenodetext1 = item1.FileName.ToString();
            //    treenode1 = new TreeNode(treenodetext1.ToString(), "TreenodeValue");
            //    //treenode1.ChildNodes
            //    TreeView1.Nodes.Add(treenode1);

            //    //TreeView1.Nodes.Add(item1.FileName.ToString() + ", Size=" + item1.FileSizeByte.ToString());
            //}


            //public async Task<IEnumerable<GetFileList>> GetLatestAsync()
            //DataServices.DataService dataservice = new DataServices.DataService();
            //System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<KazGraph.DataServices.GetFileList>> filelist1 = dataservice.GetLatestAsync();

            //lblPopulateTable.Text = filelist1.ToString();
            //lblPopulateTable.Text = filelist1.Result.ToString();



        }

        protected void BindGridList()
        {
            #region Code Connections
            //getUsersAsync().GetAwaiter().GetResult();
            //GetGraphAccessTokenAsync();
            //var tid = TokenGenAsync();

            //DataSet ds = new DataSet();
            //grvODrive.DataSource = ds;
            //grvODrive.DataBind();
            var TokenID = string.Empty;
            Task t1 = Task.Run(async () =>
            {
                TokenID = JsonConvert.SerializeObject(await GetGraphAccessToken().ConfigureAwait(false));
                //lblGetOneDriveFileListing.Text = Convert.ToString(JsonConvert.DeserializeObject<GraphAuthTokenResponse>(TokenID).access_token);
            });
            t1.Wait();
            List<clsOneDriveRootValue> te = new List<clsOneDriveRootValue>();
            Task t2 = Task.Run(async () =>
            {
                var sam = JsonConvert.SerializeObject(await GetGraphAccessOneDrive(Convert.ToString(JsonConvert.DeserializeObject<GraphAuthTokenResponse>(TokenID).access_token)).ConfigureAwait(false));
                te = JsonConvert.DeserializeObject<List<clsOneDriveRootValue>>(sam);
            });
            t2.Wait();
            #endregion
            grvODrive.DataSource = te?.Where(x => x.name != "root").OrderByDescending(x => x.createdDateTime).ToList();
            grvODrive.DataBind();
        }

        protected void grvODrive_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvODrive.PageIndex = e.NewPageIndex;
            BindGridList();
        }

        protected void grvODrive_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //myDeserializedClass.Where(x=>x.name!="root" && x.parentReference.path=="/drive/root:/Sandesh Folder" ).ToList()

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //string S_id = grvODrive.DataKeys[e.Row.RowIndex].Value.ToString();               
                //GridView gv = (GridView)e.Row.FindControl("gv2");
                //gv.DataSource = getDataChild(sQuery);
                //gv.DataBind();
            }
        }
        public async Task<GraphAuthTokenResponse> GetGraphAccessToken()
        {
            string res = string.Empty;
            GraphAuthTokenResponse myDeserializedClass = null;
            try
            {
                #region WasteCode
                var tokenResults = new GraphAuthTokenResponse();

                var url = MSGraphTokenURL + TenantID + "/oauth2/token";
                var dict = new Dictionary<string, string>
                {
                    { "Content-Type", "application/x-www-form-urlencoded" },
                    { "grant_type", GrantType },
                    { "client_id", ADClientID },
                    { "client_secret", ADClientSecret },
                    { "resource", ADResource }
                };

                var client = new HttpClient();
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
                #endregion
                return myDeserializedClass;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Authentication GetGraphAccessToken Method: " + ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }
        private async Task<List<clsOneDriveRootValue>> GetGraphAccessOneDrive(string AccessToken)
        {
            string res = string.Empty;
            List<clsOneDriveRootValue> myDeserializedClass = null;
            try
            {
                #region WasteCode
                var tokenResults = new GraphAuthTokenResponse();

                var url = ADGraphOneDrive.Replace("UserID", "f64752bf-c0e8-4c1d-b164-48a97d191949");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                var req = new HttpRequestMessage(HttpMethod.Get, url);
                string output = string.Empty;
                await client.SendAsync(req)
                       .ContinueWith(responseTask =>
                       {
                           if (responseTask.Result.IsSuccessStatusCode)
                           {
                               output = responseTask.Result.Content.ReadAsStringAsync().Result;

                           }
                       });
                #endregion
                myDeserializedClass = JsonConvert.DeserializeObject<clsOneDriveRoot>(JsonConvert.DeserializeObject(output).ToString()).value;
                return myDeserializedClass;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Authentication GetGraphAccessToken Method: " + ex.Message + " " + ex.StackTrace.ToString());
                return null;
            }
        }
        
        public DataSet JsonToDataSet(string jsonString)
        {
            DataSet ds = new DataSet();
            try
            {
                jsonString = "{ \"rootNode\": {" + jsonString.Trim().TrimStart('{').TrimEnd('}') + "} }";
                XmlDocument xd = (XmlDocument)JsonConvert.DeserializeXmlNode(jsonString);
                ds.ReadXml(new XmlNodeReader(xd));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ds;
        }
        public async static Task getUsersAsync()
        {
            var clientId = ADClientID;// "AD APP ID";
            var clientSecret = ADClientSecret;// "AD APP Secret";
            var tenantId = TenantID;//"mydomain.onmicrosoft.com";
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithTenantId(tenantId)
                .WithClientSecret(clientSecret)
                .Build();

            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);

            var groups = await graphClient.Users.Request().Select(x => new { x.Id, x.DisplayName }).GetAsync();
            foreach (var group in groups)
            {
                Debug.WriteLine($"{group.DisplayName}, {group.Id}");
            }
        }
    }
}