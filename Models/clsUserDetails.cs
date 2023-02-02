using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KazGraph.Models
{   
    public class clsUserDetailsRoot
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        public List<clsUserDetails> value { get; set; }
    }

    public class clsUserDetails
    {
        public List<object> businessPhones { get; set; }
        public string displayName { get; set; }
        public string givenName { get; set; }
        public object jobTitle { get; set; }
        public string mail { get; set; }
        public object mobilePhone { get; set; }
        public object officeLocation { get; set; }
        public object preferredLanguage { get; set; }
        public string surname { get; set; }
        public string userPrincipalName { get; set; }
        public string id { get; set; }
    }

    public class clsTenantList
    {
        public Guid AzureConnectionID { get; set; }
        public Guid AccountID { get; set; }
        public string AzureConnectionName { get; set; }
        public string Token { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string Resource { get; set; }
        public string GrantType { get; set; }
        public string TenantID { get; set; }
        public string Scopes { get; set; }
    }
}