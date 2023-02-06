using Azure;
using KazGraph.Models;
using Newtonsoft.Json;
//using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IdentityModel.Metadata;
using System.IO;
using System.Linq;
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

        public int InsertItem(object ObjBO) // passing Bussiness object Here 
        {
            try
            {

                //SqlCommand cmd = new SqlCommand("sprocUserinfoInsertUpdateSingleItem", con);
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@Name", ObjBO.Name);
                //cmd.Parameters.AddWithValue("@Address", ObjBO.address);
                //cmd.Parameters.AddWithValue("@EmailID", ObjBO.EmailID);
                //cmd.Parameters.AddWithValue("@Mobilenumber", ObjBO.Mobilenumber);
                //con.Open();
                //int Result = cmd.ExecuteNonQuery();
                //cmd.Dispose();
                //return Result;
                return 0;

            }
            catch
            {
                throw;
            }
            finally
            {

                con.Close();
                con.Dispose();
            }
        }

        public async Task<List<clsOneDriveRootValue>> SelectItem(string UID) // passing Bussiness object Here 
        {
            List<clsOneDriveRootValue> objitem = new List<clsOneDriveRootValue>();
            clsTenantList obj = new clsTenantList();
            SqlDataReader reader;
            SqlCommand command = new SqlCommand();
            try
            {
                await con.OpenAsync();
                String sql, Output = " ";
                sql = "SELECT   TesttblID      ,odatatype      ,id      ,name      ,webUrl      ,size      ,parentReferencedriveType      ,parentReferencedriveId      ,parentReferenceid      ,parentReferencepath      " +
                    ",fileSystemInfocreatedDateTime      ,fileSystemInfolastModifiedDateTime      ,filemimeType      ,filehashesquickXorHash      ,ISNULL(folderchildCount,-1) as folderchildCount ,eTag      ,cTag      " +
                    ",createdByuseremail      ,createdByuserid      ,createdByuserdisplayName      ,createdDateTime      ,lastModifiedByuseremail      ,lastModifiedByuserid      ,lastModifiedByuserdisplayName      ,lastModifiedDateTime     " +
                    " ,sharedscope  FROM Testtbl where createdByuserid='" + UID + "' ORDER BY createdDateTime DESC ";

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
                            file = new File
                            {
                                mimeType = item.filemimeType,
                                hashes = new Hashes
                                {
                                    quickXorHash = item.filehashesquickXorHash,
                                },
                            },
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
                            }
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