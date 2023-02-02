using KazGraph.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth.Extensions;
//using RestSharp.Deserializers;
using RestSharp.Extensions;
//using RestSharp.Serialization;
//using RestSharp.Serialization.Json;
//using RestSharp.Serialization.Xml;
//using RestSharp.Validation;
using RestSharp;
using System.Web.Hosting;
using System.Diagnostics;

namespace KazGraph.Service
{
    public interface IAuthenticationService
    {
        string GetGraphAccessToken();
    }
    public class AuthenticationService : IAuthenticationService
    {
        IConfigurationService _configurationService;
        ICommonService _commonService;

        public AuthenticationService(IConfigurationService configurationService, ICommonService commonService)
        {
            _configurationService = configurationService;
            _commonService = commonService;
        }
        public string GetGraphAccessToken()
        {
            string res = string.Empty;

            try
            {
                var tokenResults = new GraphAuthTokenResponse();
                var client = new RestClient(_configurationService.GetGraphTokenURL() + _configurationService.GetTenantID() + "/oauth2/token");
                //client.ti = -1;

                var request = new RestRequest();
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("grant_type", _configurationService.GetGrantype());
                request.AddParameter("client_id", _configurationService.GetClientID());
                request.AddParameter("client_secret", _configurationService.GetClientSecret());
                request.AddParameter("resource", _configurationService.GetResource());

                var response = client.ExecutePostAsync(request);
                DataSet ds = _commonService.JsonToDataSet(response.Result.ToString());
                if (ds.Tables[0].Columns.Contains("access_token"))
                {
                    dynamic PdJson = null;
                    PdJson = JsonConvert.DeserializeObject(response.Result.ToString());
                    res = JsonConvert.SerializeObject(PdJson, Newtonsoft.Json.Formatting.Indented);
                    GraphAuthTokenResponse graphAuthTokenResponse = JsonConvert.DeserializeObject<GraphAuthTokenResponse>(res);

                    return graphAuthTokenResponse.access_token;
                }
                else
                {
                    return res;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Authentication GetGraphAccessToken Method: " + ex.Message + " " + ex.StackTrace.ToString());
                return res;
            }
        }
    }
}