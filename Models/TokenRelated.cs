using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KazGraph.Models
{
    public class GetTokenResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public GetToken data { get; set; }
        public string purpose { get; set; }
        public object info { get; set; }
    }
    public class GetToken
    {
        public string jwt { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public bool firstlogin { get; set; }
    }
    public class UpdateTokenResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public object data { get; set; }
        public string purpose { get; set; }
        public object info { get; set; }
    }
    public class UpdateTokenRequest
    {
        public string token { get; set; }
        public string secret { get; set; }
        public string id { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }
    public class IPResponse
    {
        public string ip { get; set; }
    }
}