using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace KazGraph.Service
{
    public interface IMSGraphServices
    {
        Task<Event> CreateEvent(string userId, string startDtTime, string enddtTime);
        Task<Event> UpdateEvent(string id, string meetingEventId, string startDtTime, string enddtTime);
        string GetUserTimeZone(string userID, GraphServiceClient graphServiceClient);
        string GetUserProfilePhoto(string userID, string userdisplayName);
        Task<IGraphServiceUsersCollectionPage> GraphUsersList();
        string GetOnlinemeetingsByMeetingURL(string userID, string meetingURL);
        Task<IGraphServiceDevicesCollectionPage> GetDeviceDetails();
    }
    public class MSGraphService : IMSGraphServices
    {
        IAuthenticationService _authenticationService;
        //IConfigurationService _configurationService;
        //ICommonService _commonService;

        public MSGraphService(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            // _configurationService = configurationService;
            //_commonService = commonService;
        }

        public async Task<Event> CreateEvent(string userId, string startDtTime, string enddtTime)
        {
            var result = new Event();
            try
            {
                var access_token = _authenticationService.GetGraphAccessToken();
                var startTime = startDtTime;
                var endTime = enddtTime;

                var graphserviceClient = new GraphServiceClient(
                    new DelegateAuthenticationProvider(
                        (requestMessage) =>
                        {
                            requestMessage.Headers.Add("Prefer", "outlook.timezone");
                            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", access_token);
                            return Task.FromResult(0);
                        }));

                //Get user's TimeZone
                var timezone = GetUserTimeZone(userId, graphserviceClient);



                //result = await graphserviceClient.Users[userId].Drives
                //    .Request()
                //    .GetAsync();

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MSGraphServices CreateEvent Method: " + ex.Message + " " + ex.StackTrace.ToString());
                return result;
            }
        }

        public Task<IGraphServiceDevicesCollectionPage> GetDeviceDetails()
        {
            throw new NotImplementedException();
        }

        public string GetOnlinemeetingsByMeetingURL(string userID, string meetingURL)
        {
            throw new NotImplementedException();
        }

        public string GetUserProfilePhoto(string userID, string userdisplayName)
        {
            throw new NotImplementedException();
        }

        public string GetUserTimeZone(string userID, GraphServiceClient graphServiceClient)
        {
            throw new NotImplementedException();
        }

        public async Task<IGraphServiceUsersCollectionPage> GraphUsersList()
        {
            try
            {
                var access_token = _authenticationService.GetGraphAccessToken();
                var graphserviceClient = new GraphServiceClient(
                    new DelegateAuthenticationProvider(
                        (requestMessage) =>
                        {
                            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", access_token);
                            return Task.FromResult(0);
                        }));

                var users = await graphserviceClient.Users
                    .Request()
                    .Select("businessPhones,givenName,displayName,jobTitle,mail,mobilePhone,officeLocation,id,preferredLanguage,surname,userPrincipalName")
                    .GetAsync();
                return users;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MSGraphServices GraphUsersList Method: " + ex.Message + " " + ex.StackTrace.ToString());
                throw new Exception(ex.Message);
            }
        }

        public Task<Event> UpdateEvent(string id, string meetingEventId, string startDtTime, string enddtTime)
        {
            throw new NotImplementedException();
        }
    }
}