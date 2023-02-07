using Azure;
using KazGraph.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using File = KazGraph.Models.File;
using FileSystemInfo = KazGraph.Models.FileSystemInfo;
using Hashes = KazGraph.Models.Hashes;

namespace KazGraph.DAL
{
    public class OneDriveDal
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["KazooDB"].ConnectionString);
        public DataTable GetTenantList()
        {
            SqlDataAdapter adpt = new SqlDataAdapter();
            SqlCommand command = new SqlCommand();
            try
            {
                con.Open();
                string com = "Select * from AzureConnection";
                adpt = new SqlDataAdapter(com, con);
                DataTable dt = new DataTable();
                adpt.Fill(dt);
                return dt;
            }
            catch
            {
                throw;
            }
            finally
            {
                adpt.Dispose();
                command.Dispose();
                con.Close();
                con.Dispose();
            }
        }

        public async Task<clsTenantList> GetTenantDetails(string id)
        {
            clsTenantList obj = new clsTenantList();
            SqlDataReader dataReader;
            SqlCommand command = new SqlCommand();
            try
            {
                con.Open();
                String sql, Output = " ";
                sql = "Select AzureConnectionID, AccountID,AzureConnectionName,Token,ClientID,ClientSecret,Resource,GrantType,TenantID,Scopes from AzureConnection where AzureConnectionID='" + id + "'";

                command = new SqlCommand(sql, con);

                dataReader = await command.ExecuteReaderAsync();
                while (dataReader.Read())
                {
                    obj.AzureConnectionID = Guid.Parse(Convert.ToString(dataReader.GetValue(0)));
                    if (!string.IsNullOrWhiteSpace(dataReader["AccountID"] as string))
                    {
                        obj.AccountID = Guid.Parse(Convert.ToString(dataReader["AccountID"]));
                    }
                    obj.AzureConnectionName = Convert.ToString(dataReader.GetValue(2));
                    obj.Token = Convert.ToString(dataReader.GetValue(3));
                    obj.ClientID = Convert.ToString(dataReader.GetValue(4));
                    obj.ClientSecret = Convert.ToString(dataReader.GetValue(5));
                    obj.Resource = Convert.ToString(dataReader.GetValue(6));
                    obj.GrantType = Convert.ToString(dataReader.GetValue(7));
                    obj.TenantID = Convert.ToString(dataReader.GetValue(8));
                    obj.Scopes = Convert.ToString(dataReader.GetValue(9));
                }

                return obj;
            }
            catch
            {
                throw;
            }
            finally
            {
                command.Dispose();
                con.Close();
                con.Dispose();
            }
        }

        public List<ListItem> GetActionList()
        {
            try
            {
                List<ListItem> act = new List<ListItem>
                        {
                            new ListItem("----Select----", "-1"),
                            new ListItem("From DB", "1"),
                            new ListItem("To DB", "2")
                        };
                return act;
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetUserList()
        {
            SqlDataAdapter adpt = new SqlDataAdapter();
            SqlCommand command = new SqlCommand();
            try
            {
                con.Open();
                string com = "Select * from AzureConnection";
                adpt = new SqlDataAdapter(com, con);
                DataTable dt = new DataTable();
                adpt.Fill(dt);
                return dt;
            }
            catch
            {
                throw;
            }
            finally
            {
                adpt.Dispose();
                command.Dispose();
                con.Close();
                con.Dispose();
            }
        }

        public async Task<int> InsertItem(string UID,List<clsOneDriveRootValue> ObjBO, string AzureConnectionID) // passing Bussiness object Here 
        {
            try
            {
                List<OneDriveItemDTOstring> objlist = new List<OneDriveItemDTOstring>();

                foreach (var item in ObjBO?.ToList())
                {
                    bool filetype = false;
                    bool CreatedBytype = false;
                    bool lastModifiedBytype = false;
                    bool folderchildCounttype = false;
                    if (string.IsNullOrWhiteSpace(Convert.ToString(item.file)))
                    {
                        filetype = true;
                    }

                    if (string.IsNullOrWhiteSpace(Convert.ToString(item.createdBy)))
                    {
                        CreatedBytype = true;
                    }

                    if (string.IsNullOrWhiteSpace(Convert.ToString(item.lastModifiedBy)))
                    {
                        lastModifiedBytype = true;
                    }
                    if (string.IsNullOrWhiteSpace(Convert.ToString(item.folder)))
                    {
                        folderchildCounttype = true;
                    }

                    objlist.Add(new OneDriveItemDTOstring()
                    {
                        id = Convert.ToString(item.id),
                        name = Convert.ToString(item.name),
                        webUrl = Convert.ToString(item.webUrl),
                        size = (item.size as int?) ?? 0,
                        parentReferencedriveType = (string)(item.parentReference.driveType ?? (object)DBNull.Value),
                        parentReferencedriveId = item.parentReference.driveId,
                        parentReferenceid = item.parentReference.id,
                        parentReferencepath = item.parentReference.path,
                        fileSystemInfocreatedDateTime = item.fileSystemInfo.createdDateTime.ToString("G", new CultureInfo("en-US")),
                        fileSystemInfolastModifiedDateTime = item.fileSystemInfo.lastModifiedDateTime.ToString("G", new CultureInfo("en-US")),

                        filemimeType = (filetype == true ? string.Empty : item.file.mimeType),
                        filehashesquickXorHash = (filetype == true ? string.Empty : item.file.hashes.quickXorHash),

                        folderchildCount = folderchildCounttype == true ? 0 : item.folder.childCount,
                        eTag = Convert.ToString(item.eTag),
                        cTag = Convert.ToString(item.cTag),
                        createdByuseremail = CreatedBytype == true ? string.Empty : Convert.ToString(item.createdBy.user.email),
                        createdByuserid = CreatedBytype == true ? string.Empty : Convert.ToString(item.createdBy.user.id),
                        createdByuserdisplayName = CreatedBytype == true ? string.Empty : Convert.ToString(item.createdBy.user.displayName),

                        lastModifiedByuseremail = lastModifiedBytype == true ? string.Empty : Convert.ToString(item.lastModifiedBy.user.email),
                        lastModifiedByuserid = lastModifiedBytype == true ? string.Empty : Convert.ToString(item.lastModifiedBy.user.id),
                        lastModifiedByuserdisplayName = lastModifiedBytype == true ? string.Empty : Convert.ToString(item.lastModifiedBy.user.displayName),
                        lastModifiedDateTime = item.lastModifiedDateTime.ToString("G", new CultureInfo("en-US")),
                        createdDateTime = item.createdDateTime.ToString("G", new CultureInfo("en-US")),
                        AzureConnectionID = Guid.Parse(AzureConnectionID)
                    }
                    );
                }

                if (objlist.Count > 0)
                {
                    #region old style
                    //Creating Data Table
                    //DataTable dt = new DataTable("OneDriveItem");
                    //dt.Columns.AddRange(new DataColumn[26] {
                    //    new DataColumn("odatatype", typeof(string))
                    //,new DataColumn("id", typeof(string))
                    //,new DataColumn("name", typeof(string))
                    //,new DataColumn("webUrl", typeof(string))
                    //,new DataColumn("size", typeof(string))
                    //,new DataColumn("parentReferencedriveType", typeof(string))
                    //,new DataColumn("parentReferencedriveId", typeof(string))
                    //,new DataColumn("parentReferenceid", typeof(string))
                    //,new DataColumn("parentReferencepath", typeof(string))
                    //,new DataColumn("fileSystemInfocreatedDateTime", typeof(string))
                    //,new DataColumn("fileSystemInfolastModifiedDateTime", typeof(string))
                    //,new DataColumn("filemimeType", typeof(string))
                    //,new DataColumn("filehashesquickXorHash", typeof(string))
                    //,new DataColumn("folderchildCount", typeof(string))
                    //,new DataColumn("eTag", typeof(string))
                    //,new DataColumn("cTag", typeof(string))
                    //,new DataColumn("createdByuseremail", typeof(string))
                    //,new DataColumn("createdByuserid", typeof(string))
                    //,new DataColumn("createdByuserdisplayName", typeof(string))
                    //,new DataColumn("createdDateTime", typeof(string))
                    //,new DataColumn("lastModifiedByuseremail", typeof(string))
                    //,new DataColumn("lastModifiedByuserid", typeof(string))
                    //,new DataColumn("lastModifiedByuserdisplayName", typeof(string))
                    //,new DataColumn("lastModifiedDateTime", typeof(string))
                    //,new DataColumn("sharedscope", typeof(string))
                    //,new DataColumn("AzureConnectionID", typeof(string))
                    //}); 
                    #endregion

                    DataTable dt = ToDataTable(objlist);
                    dt.Columns.Remove("TesttblID");
                    //dt.Columns.Remove("id");
                    //dt.Columns.Remove("name");
                    //dt.Columns.Remove("webUrl");
                    //dt.Columns.Remove("size");
                    //dt.Columns.Remove("parentReferencedriveType");
                    //dt.Columns.Remove("parentReferencedriveId");
                    //dt.Columns.Remove("parentReferenceid");
                    //dt.Columns.Remove("parentReferencepath");
                    //dt.Columns.Remove("fileSystemInfocreatedDateTime");
                    //dt.Columns.Remove("fileSystemInfolastModifiedDateTime");
                    //dt.Columns.Remove("filemimeType");
                    //dt.Columns.Remove("filehashesquickXorHash");
                    //dt.Columns.Remove("folderchildCount");
                    //dt.Columns.Remove("eTag");
                    //dt.Columns.Remove("cTag");
                    //dt.Columns.Remove("createdByuseremail");
                    //dt.Columns.Remove("createdByuserid");
                    //dt.Columns.Remove("createdByuserdisplayName");
                    //dt.Columns.Remove("createdDateTime");
                    //dt.Columns.Remove("lastModifiedByuseremail");
                    //dt.Columns.Remove("lastModifiedByuserid");
                    //dt.Columns.Remove("lastModifiedByuserdisplayName");
                    //dt.Columns.Remove("lastModifiedDateTime");
                    ////dt.Columns.Remove("sharedscope");
                    //dt.Columns.Remove("AzureConnectionID");

                    //Creating the connection object
                    await con.OpenAsync();
                    //You can pass any stored procedure
                    //As I am using Higher version of SQL Server, so, I am using the Stored Procedure which uses MERGE Function
                    using (SqlCommand cmd = new SqlCommand("InsertOneDriveItems", con))
                    {
                        //Set the command type as StoredProcedure
                        cmd.CommandType = CommandType.StoredProcedure;
                        //Add the input parameter required by the stored procedure
                        cmd.Parameters.AddWithValue("@OneDriveItem", dt);
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@AzureConnectionID", AzureConnectionID);
                        //Execute the command
                        int sa = cmd.ExecuteNonQuery();
                    }
                    return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

                con.Close();
                con.Dispose();
            }
        }

        public async Task<List<clsOneDriveRootValue>> SelectItem(string UID, string AzureConnectionID) // passing Bussiness object Here 
        {
            List<clsOneDriveRootValue> objitem = new List<clsOneDriveRootValue>();
            clsTenantList obj = new clsTenantList();
            SqlDataReader reader;
            SqlCommand command = new SqlCommand();
            try
            {
                await con.OpenAsync();
                String sql, Output = " ";

                if (UID == "0")
                {

                    sql = "SELECT   TesttblID      ,odatatype      ,id      ,name      ,webUrl      ,size      ,parentReferencedriveType      ,parentReferencedriveId      ,parentReferenceid      ,parentReferencepath      " +
                        ",fileSystemInfocreatedDateTime      ,fileSystemInfolastModifiedDateTime      ,filemimeType      ,filehashesquickXorHash      ,ISNULL(folderchildCount,-1) as folderchildCount ,eTag      ,cTag      " +
                        ",createdByuseremail      ,createdByuserid      ,createdByuserdisplayName      ,createdDateTime      ,lastModifiedByuseremail      ,lastModifiedByuserid      ,lastModifiedByuserdisplayName      ,lastModifiedDateTime     " +
                        " ,sharedscope,AzureConnectionID  FROM Testtbl where AzureConnectionID='" + AzureConnectionID + "' ORDER BY createdDateTime DESC ";
                }
                else
                {
                    sql = "SELECT   TesttblID      ,odatatype      ,id      ,name      ,webUrl      ,size      ,parentReferencedriveType      ,parentReferencedriveId      ,parentReferenceid      ,parentReferencepath      " +
                        ",fileSystemInfocreatedDateTime      ,fileSystemInfolastModifiedDateTime      ,filemimeType      ,filehashesquickXorHash      ,ISNULL(folderchildCount,-1) as folderchildCount ,eTag      ,cTag      " +
                        ",createdByuseremail      ,createdByuserid      ,createdByuserdisplayName      ,createdDateTime      ,lastModifiedByuseremail      ,lastModifiedByuserid      ,lastModifiedByuserdisplayName      ,lastModifiedDateTime     " +
                        " ,sharedscope,AzureConnectionID  FROM Testtbl where createdByuserid='" + UID + "' ORDER BY createdDateTime DESC ";
                }

                command = new SqlCommand(sql, con);
                reader = await command.ExecuteReaderAsync();
                var dataTable = new DataTable();
                dataTable.Load(reader);
                List<OneDriveItemDTO> objDTO = new List<OneDriveItemDTO>();
                if (dataTable.Rows.Count > 0)
                {
                    var serializedMyObjects = JsonConvert.SerializeObject(dataTable);
                    // Here you get the object
                    objDTO = (List<OneDriveItemDTO>)JsonConvert.DeserializeObject(serializedMyObjects, typeof(List<OneDriveItemDTO>));

                    foreach (var item in objDTO)
                    {

                        objitem.Add(new clsOneDriveRootValue()
                        {
                            id = item.id,
                            name = item.name,
                            webUrl = item.webUrl,
                            size = (item.size as int?) ?? 0,
                            parentReference = new ParentReference
                            {
                                driveType = item.parentReferencedriveType,
                                driveId = item.parentReferencedriveId,
                                id = item.parentReferenceid,
                                path = item.parentReferencepath
                            },
                            fileSystemInfo = new FileSystemInfo
                            {
                                createdDateTime = item.fileSystemInfocreatedDateTime,
                                lastModifiedDateTime = item.fileSystemInfolastModifiedDateTime
                            },
                            file = (item.filemimeType == "" && item.filehashesquickXorHash == "" ? null : new File
                            {
                                mimeType = item.filemimeType,
                                hashes = new Hashes
                                {
                                    quickXorHash = item.filehashesquickXorHash,
                                },
                            }),
                            folder = (item.folderchildCount == -1 ? null : new Models.Folder
                            {
                                childCount = item.folderchildCount,
                            }),
                            eTag = item.eTag,
                            cTag = item.cTag,
                            createdBy = new CreatedBy
                            {
                                user = new User
                                {
                                    email = item.createdByuseremail,
                                    id = item.createdByuserid,
                                    displayName = item.createdByuserdisplayName
                                },
                            },
                            lastModifiedBy = new LastModifiedBy
                            {
                                user = new User
                                {
                                    email = item.lastModifiedByuseremail,
                                    id = item.lastModifiedByuserid,
                                    displayName = item.lastModifiedByuserdisplayName
                                },
                            },
                            createdDateTime = item.createdDateTime,
                            lastModifiedDateTime = item.lastModifiedDateTime

                            //sharedscope = item.sharedscope = item.sharedscope
                        });


                    }
                }
                return objitem;
            }
            catch
            {
                throw;
            }
            finally
            {
                command.Dispose();
                con.Close();
                con.Dispose();
            }
        }


        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }



        //public static T Def<T>(this SqlDataReader r, int ord)
        //{
        //    var t = r.GetSqlValue(ord);
        //    if (t == DBNull.Value) return default(T);
        //    return ((INullable)t).IsNull ? default(T) : (T)t;
        //}

        //public static T? Val<T>(this SqlDataReader r, int ord) where T : struct
        //{
        //    var t = r.GetSqlValue(ord);
        //    if (t == DBNull.Value) return null;
        //    return ((INullable)t).IsNull ? (T?)null : (T)t;
        //}

        //public static T Ref<T>(this SqlDataReader r, int ord) where T : class
        //{
        //    var t = r.GetSqlValue(ord);
        //    if (t == DBNull.Value) return null;
        //    return ((INullable)t).IsNull ? null : (T)t;
        //}
    }
}