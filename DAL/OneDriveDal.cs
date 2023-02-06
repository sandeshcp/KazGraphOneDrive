using Azure;
using KazGraph.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

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
    }
}