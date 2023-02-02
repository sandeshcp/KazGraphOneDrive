using KazGraph.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace KazGraph.Service
{
    public interface ICommonService
    {
        DataSet JsonToDataSet(string jsonString);
        Task<GetTokenResponse> GetTokenData(string secret, string id, string ip, string email);
        UpdateTokenResponse UpdateToken(UpdateTokenRequest updateTokenRequest);
        Task<IPResponse> GetIPAddress();
    }
    public class CommonService : ICommonService
    {
        IConfigurationService _configurationService;
        
        public CommonService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;            
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
        public async Task<GetTokenResponse> GetTokenData(string secret, string id, string ip, string email)
        {
            GetTokenResponse getTokenResponse = new GetTokenResponse();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    string endpoint = _configurationService.GetTokenDetailsURL(id, secret, ip, email);
                    using (var Response = await client.GetAsync(endpoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            string apiResults = await Response.Content.ReadAsStringAsync();
                            getTokenResponse = JsonConvert.DeserializeObject<GetTokenResponse>(apiResults);

                            return getTokenResponse;
                        }
                        else
                        {
                            getTokenResponse.purpose = "failed " + Response.StatusCode;
                            return getTokenResponse;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Common GetTokenData Method: " + ex.Message + " " + ex.StackTrace.ToString());
                return getTokenResponse;
            }
        }
        public UpdateTokenResponse UpdateToken(UpdateTokenRequest updateTokenRequest)
        {
            var result = new UpdateTokenResponse();
            try
            {
                var client = new RestClient(_configurationService.UpdateTokenURL());
                //client.Timeout = -1;
                var request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                var body = @"{" + "\n" +
                @"    ""id"": " + '"' + updateTokenRequest.id + '"' + "," + "\n" +
                @"    ""secret"":" + '"' + updateTokenRequest.secret + '"' + "," + "\n" +
                @"    ""token"":" + '"' + updateTokenRequest.token + '"' + "," + "\n" +
                @"    ""email"":" + '"' + updateTokenRequest.email + '"' + "," + "\n" +
                @"    ""phone"":" + '"' + updateTokenRequest.phone + '"' + "" + "\n" +
                @"}";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                var response = client.ExecutePut(request);
                DataSet ds = JsonToDataSet(response.Content);
                if (ds.Tables.Count > 0)
                {
                    dynamic PdJson = null;
                    PdJson = JsonConvert.DeserializeObject(response.Content);

                    var res = JsonConvert.SerializeObject(PdJson);
                    result = JsonConvert.DeserializeObject<UpdateTokenResponse>(res);
                    return result;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Common UpdateToken Method: " + ex.Message + " " + ex.StackTrace.ToString());
                return result;
            }
        }
        public async Task<IPResponse> GetIPAddress()
        {
            IPResponse ipResponse = new IPResponse();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    string endpoint = _configurationService.GetIPAddressURL();
                    using (var Response = await client.GetAsync(endpoint))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            string apiResults = await Response.Content.ReadAsStringAsync();
                            ipResponse = JsonConvert.DeserializeObject<IPResponse>(apiResults);

                            return ipResponse;
                        }
                        else
                        {
                            return ipResponse;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Common GetIPAddress Method: " + ex.Message + " " + ex.StackTrace.ToString());
                return ipResponse;
            }
        }
    }
}