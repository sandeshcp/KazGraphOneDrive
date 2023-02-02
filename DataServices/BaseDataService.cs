using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ByteSizeLib;

namespace KazGraph.DataServices
{
    public class BaseDataService
    {
        private static string _sqlConnectionString;
        public static void Configure(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }


        //System.Configuration.ConnectionStringSettingsCollection connectionStrings = System.Web.Configuration.WebConfigurationManager.ConnectionStrings as System.Configuration.ConnectionStringSettingsCollection;

        ////lblPopulateTable.Text =  connectionStrings[1].Name.ToString() + " :  " + connectionStrings[1].ConnectionString.ToString();
        //string databaseconnection = connectionStrings[1].ConnectionString.ToString();


        protected IDbConnection GetDbConnection() => new SqlConnection("Server=CRM345;Database=Kazoo;Trusted_Connection=True;");
    
    }

 


}