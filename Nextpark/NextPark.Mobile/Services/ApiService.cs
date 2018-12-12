using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NextPark.Mobile.Core.Settings;
using NextPark.Models;
using Plugin.Connectivity;

namespace NextPark.Mobile.Services
{
    public class ApiService : IApiService
    {

        private readonly string TokenType = "Bearer";

        public string AuthToken { get; set; }

        public ApiService()
        {

        }

        public async Task<Response> CheckConnection()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Please turn on your internet settings.",
                };
            }

            var isReachable = await CrossConnectivity.Current.IsRemoteReachable("google.com");
            if (!isReachable)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Check you internet connection.",
                };
            }

            return new Response
            {
                IsSuccess = true,
                Message = "Ok",
            };

        }

        public async Task<Response> Get<TVm>(string endpoint, int id)
        {
            var isConneted = await CheckConnection();
            if (!isConneted.IsSuccess) return isConneted;

            try
            {
                var url = $"{endpoint}/{id}";
                var client = GetHttpClient();
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString()
                    };

                var result = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<TVm>(result);
                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Result = model
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> Get<TVm>(string endpoint)
        {

            var isConneted = await CheckConnection();
            if (!isConneted.IsSuccess) return isConneted;

            try
            {
                var url = $"{endpoint}";
                var client = GetHttpClient();
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString()
                    };
                var result = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<List<TVm>>(result);

                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Result = model
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> Post<TVm>(string endpoint, TVm tvm)
        {
            var isConneted = await CheckConnection();
            if (!isConneted.IsSuccess) return isConneted;

            try
            {
                var url = $"{endpoint}";
                var client = GetHttpClient();

                var json = JsonConvert.SerializeObject(tvm);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpoint, content);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.ReasonPhrase
                    };

                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TVm>(resultJson);

                return new Response
                {
                    IsSuccess = true,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> Put<TVm>(string endpoint, int id,  TVm tvm)
        {
            var isConneted = await CheckConnection();
            if (!isConneted.IsSuccess) return isConneted;

            try
            {
                var url = $"{endpoint}/{id}";
                var client = GetHttpClient();

                var json = JsonConvert.SerializeObject(tvm);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(endpoint, content);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.ReasonPhrase
                    };

                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TVm>(resultJson);

                return new Response
                {
                    IsSuccess = true,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> Delete<TVm>(string endpoint, int id)
        {
            var isConneted = await CheckConnection();
            if (!isConneted.IsSuccess) return isConneted;

            try
            {
                var url = $"{endpoint}/{id}";
                var client = GetHttpClient();
                var response = await client.DeleteAsync(endpoint);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.ReasonPhrase
                    };

                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TVm>(resultJson);

                return new Response
                {
                    IsSuccess = true,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            if (string.IsNullOrEmpty(AuthSettings.Token)) return client;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TokenType, AuthSettings.Token);
            return client;
        }
    }
}
