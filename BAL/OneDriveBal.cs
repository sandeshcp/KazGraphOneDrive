using KazGraph.DAL;
using KazGraph.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace KazGraph.BAL
{
    public class OneDriveBal
    {
        public DataTable GetTenantList() // passing Bussiness object Here 
        {
            try
            {
                OneDriveDal objTenant = new OneDriveDal(); // Creating object of Dataccess
                return objTenant.GetTenantList(); // calling Method of DataAccess 
            }
            catch
            {
                throw;
            }
        }

        public async Task<clsTenantList> GetTenantDetails(string id)
        {
            try
            {
                OneDriveDal objTenant = new OneDriveDal(); // Creating object of Dataccess
                return await objTenant.GetTenantDetails(id); // calling Method of DataAccess 
            }
            catch
            {
                throw;
            }
        }

        public List<ListItem> GetActionList()
        {
            try
            {
                OneDriveDal objTenant = new OneDriveDal(); // Creating object of Dataccess
                return objTenant.GetActionList(); // calling Method of DataAccess 
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> InsertItem(string UID,List<clsOneDriveRootValue> ObjBO, string AzureConnectionID)
        {
            try
            {
                OneDriveDal objTenant = new OneDriveDal(); // Creating object of Dataccess
                return await objTenant.InsertItem(UID,ObjBO, AzureConnectionID); // calling Method of DataAccess 
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<clsOneDriveRootValue>> SelectItem(string UID,string AzureConnectionID)
        {
            try
            {
                OneDriveDal objTenant = new OneDriveDal();
                return await objTenant.SelectItem(UID, AzureConnectionID);
            }
            catch
            {
                throw;
            }
        }

    }
}