using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace KazGraph.Service
{
    public interface IConfigurationService
    {
        string GetGraphTokenURL();
        string GetTenantID();
        string GetGrantype();
        string GetClientID();
        string GetClientSecret();
        string GetResource();
        string GetTokenDetailsURL(string id, string secret, string ip, string email);
        string UpdateTokenURL();
        string GetIPAddressURL();
    }

    public class ConfigurationService : IConfigurationService
    {
        //private IConfiguration _Configuration;
        //public ConfigurationService(IConfiguration configuration)
        //{
        //    _Configuration = configuration;
        //}
        public string GetapibaseURL()
        {
            return WebConfigurationManager.AppSettings["WebAPIBaseUrl"];//_Configuration.GetSection("WebAPIBaseUrl").Value;
        }

        public string GetGraphTokenURL()
        {
            return WebConfigurationManager.AppSettings["MSGraphTokenURL"];
        }

        public string GetTenantID()
        {
            var sam = WebConfigurationManager.AppSettings["AzureAd:TenantID"];
            return ConfigurationManager.AppSettings["AzureAd:TenantID"];
        }

        public string GetGrantype()
        {
            return ConfigurationManager.AppSettings["AzureAd:GrantType"];
        }
        public string GetClientID()
        {
            return ConfigurationManager.AppSettings["AzureAd:ClientID"];
        }

        public string GetClientSecret()
        {
            return ConfigurationManager.AppSettings["AzureAd:ClientSecret"];
        }
        public string GetResource()
        {
            return ConfigurationManager.AppSettings["AzureAd:Resource"];
        }

        public string GetTokenDetailsURL(string id, string secret, string ip, string email)
        {
            return GetapibaseURL() + "/user/snipits/token?secret=" + secret + "&id=" + id + "&deviceid=" + ip + "&email=" + email;
        }

        public string UpdateTokenURL()
        {
            return GetapibaseURL() + "/user/snipits/token";
        }
        public string GetIPAddressURL()
        {            
            return ConfigurationManager.AppSettings["IPAddressURL"];
        }
    }
}