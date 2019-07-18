using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NextPark.Enums.Enums;
using NextPark.Mobile.Settings;
using NextPark.Models;
using Xamarin.Essentials;

namespace NextPark.Mobile.Services
{
    public class ApiService : IApiService
    {

        private readonly string TokenType = "Bearer";        

        public string AuthToken { get; set; }

        public ApiService()
        {            
        }

        public ApiResponse CheckConnection()
        {
            var current = Connectivity.NetworkAccess;

            if (current == Xamarin.Essentials.NetworkAccess.Internet)
            {
                // Connection to internet is available
                return new ApiResponse
                {
                    IsSuccess = true,
                    Message = "Ok"
                };
            }
            return new ApiResponse
            {
                IsSuccess = false,
                Message = "Check you internet connection.",
                ErrorType = ErrorType.InternetConnectionError
            };
            /*
            if (_crossConnectivity == null)
            {

                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Network connectivity not available.",
                    ErrorType = ErrorType.InternetConnectionError
                };
            }

            if (!_crossConnectivity.IsConnected)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Please turn on your internet settings.",
                    ErrorType = ErrorType.InternetConnectionError
                };
            }
            var isReachable = await _crossConnectivity.IsReachable("www.google.com", 1000).ConfigureAwait(false);
            //var isReachable = await _crossConnectivity.IsRemoteReachable(ApiSettings.BaseUri, ApiSettings.BasePort, 1000);

            if (!isReachable)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Check you internet connection.",
                    ErrorType = ErrorType.InternetConnectionError
                };
            }

            return new ApiResponse
            {
                IsSuccess = true,
                Message = "Ok"
            };
            */
        }

        public async Task<ApiResponse<TVm>> Get<TVm>(string endpoint, int id)
        {
            var isConneted = CheckConnection();
            if (!isConneted.IsSuccess)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = isConneted.IsSuccess,
                    Message = isConneted.Message,
                    ErrorType = isConneted.ErrorType,
                    Result = default(TVm)
                };
            }

            try
            {
                var url = $"{endpoint}/{id}";
                var client = GetHttpClient();
                var response = await client.GetAsync(url).ConfigureAwait(false);

                //if (!response.IsSuccessStatusCode)
                //    return new ApiResponse
                //    {
                //        IsSuccess = false,
                //        Message = response.StatusCode.ToString()
                //    };

                var resultAsJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TVm>>(resultAsJson);
                return apiResponse;
                //return new ApiResponse
                //{
                //    IsSuccess = true,
                //    Message = "Ok",
                //    Result = model
                //};
            }
            catch (Exception ex)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorType = ErrorType.Exception
                };
            }
        }

        public async Task<ApiResponse<TVm>> Get<TVm> (string endpoint)
        {

            var isConneted = CheckConnection();
            if (!isConneted.IsSuccess)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = isConneted.IsSuccess,
                    Message = isConneted.Message,
                    ErrorType = isConneted.ErrorType,
                    Result = default(TVm)
                };
            }

            try
            {
                var url = $"{endpoint}";
                var client = GetHttpClient();
                var response = await client.GetAsync(url).ConfigureAwait(false);

                //if (!response.IsSuccessStatusCode)
                //    return new ApiResponse
                //    {
                //        IsSuccess = false,
                //        Message = response.StatusCode.ToString()
                //    };
                var resultAsJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TVm>>(resultAsJson);
                if (apiResponse == null) {
                    return new ApiResponse<TVm>
                    {
                        IsSuccess = false,
                        Message = "",
                        ErrorType = ErrorType.None
                    };
                }
                return apiResponse;

            }
            catch (Exception ex)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorType = ErrorType.Exception
                };
            }
        }

        public async Task<ApiResponse<TVm>> Post<TVm>(string endpoint, TVm tvm)
        {
            var isConneted = CheckConnection();
            if (!isConneted.IsSuccess)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = isConneted.IsSuccess,
                    Message = isConneted.Message,
                    ErrorType = isConneted.ErrorType,
                    Result = default(TVm)
                };
            }

            try
            {
                var url = $"{endpoint}";
                var client = GetHttpClient();

                var json = JsonConvert.SerializeObject(tvm);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content).ConfigureAwait(false);

                var resultAsJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TVm>>(resultAsJson);
                return apiResponse;
            }
            catch (Exception ex)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorType = ErrorType.Exception
                };
            }
            //if (!response.IsSuccessStatusCode)
            //{

            //    ApiResponse recivedApiResponse = null;
            //    try
            //    {
            //        var apiResultJson = await response.Content.ReadAsStringAsync();
            //        recivedApiResponse = JsonConvert.DeserializeObject<ApiResponse>(apiResultJson);
            //    }
            //    catch
            //    {

            //    }
            //    if (recivedApiResponse == null)
            //    {
            //        return new ApiResponse
            //        {
            //            IsSuccess = false,
            //            Message = response.Content.ReadAsStringAsync().Result
            //        };
            //    }

            //    return recivedApiResponse;
            //}



        }

        public async Task<ApiResponse<TVm>> Post<TParam, TVm>(string endpoint, TParam tvm)
        {
            var isConneted = CheckConnection();
            if (!isConneted.IsSuccess)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = isConneted.IsSuccess,
                    Message = isConneted.Message,
                    ErrorType = isConneted.ErrorType,
                    Result = default(TVm)
                };
            }

            try
            {
                var json = JsonConvert.SerializeObject(tvm);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                // Ecco
                var client = GetHttpClient();
                var response = await client.PostAsync(endpoint, content).ConfigureAwait(false);

                //if (!response.IsSuccessStatusCode)
                //{
                //    ApiResponse recivedApiResponse = null;
                //    try
                //    {
                //        var apiResultJson = await response.Content.ReadAsStringAsync();
                //        recivedApiResponse = JsonConvert.DeserializeObject<ApiResponse>(apiResultJson);
                //    }
                //    catch
                //    {

                //    }
                //    if (recivedApiResponse == null) {
                //        return new ApiResponse
                //        {
                //            IsSuccess = false,
                //            Message = response.Content.ReadAsStringAsync().Result
                //        };
                //    }

                //    return recivedApiResponse;

                //}

                var resultAsJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TVm>>(resultAsJson);
                return apiResponse;
            }
            catch (Exception ex)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorType = ErrorType.Exception
                };
            }
        }

        public async Task<ApiResponse<TVm>> Put<TVm>(string endpoint, int id, TVm tvm)
        {
            var isConneted = CheckConnection();
            if (!isConneted.IsSuccess)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = isConneted.IsSuccess,
                    Message = isConneted.Message,
                    ErrorType = isConneted.ErrorType,
                    Result = default(TVm)
                };
            }

            try
            {
                var url = $"{endpoint}/{id}";
                var client = GetHttpClient();

                var json = JsonConvert.SerializeObject(tvm);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(url, content).ConfigureAwait(false);

                var resultAsJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TVm>>(resultAsJson);
                return apiResponse;
            }
            catch (Exception ex)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorType = ErrorType.Exception
                };
            }
            //    if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound)
            //        return new ApiResponse
            //        {
            //            IsSuccess = false,
            //            Message = response.ReasonPhrase
            //        };

            //    var resultJson = await response.Content.ReadAsStringAsync();
            //    var result = JsonConvert.DeserializeObject<TVm>(resultJson);

            //    return new ApiResponse
            //    {
            //        IsSuccess = true,
            //        Result = result
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ApiResponse
            //    {
            //        IsSuccess = false,
            //        Message = ex.Message
            //    };
            //}
        }

       

        public async Task<ApiResponse<TVm>> Put<TVm>(string url, TVm tvm)
        {
            var isConneted = CheckConnection();
            if (!isConneted.IsSuccess)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = isConneted.IsSuccess,
                    Message = isConneted.Message,
                    ErrorType = isConneted.ErrorType,
                    Result = default(TVm)
                };
            }

            try
            {
                var client = GetHttpClient();

                var json = JsonConvert.SerializeObject(tvm);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(url, content).ConfigureAwait(false);
                var resultAsJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TVm>>(resultAsJson);
                return apiResponse;
            }
            catch (Exception ex)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorType = ErrorType.Exception
                };
            }
            //if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound)
            //    return new ApiResponse
            //    {
            //        IsSuccess = false,
            //        Message = response.ReasonPhrase
            //    };

            //var resultJson = await response.Content.ReadAsStringAsync();
            //var result = JsonConvert.DeserializeObject<List<TVm>>(resultJson);

            //return new ApiResponse
            //{
            //    IsSuccess = true,
            //    Result = result
            //};
            //}
            //catch (Exception ex)
            //{
            //    return new ApiResponse
            //    {
            //        IsSuccess = false,
            //        Message = ex.Message
            //    };
            //}
        }

        public async Task<ApiResponse<TVm>> Put<TParam,TVm>(string url, TParam tParam)
        {
            var isConneted = CheckConnection();
            if (!isConneted.IsSuccess)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = isConneted.IsSuccess,
                    Message = isConneted.Message,
                    ErrorType = isConneted.ErrorType,
                    Result = default(TVm)
                };
            }

            try
            {
                var client = GetHttpClient();

                var json = JsonConvert.SerializeObject(tParam);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(url, content).ConfigureAwait(false);
                var resultAsJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TVm>>(resultAsJson);
                return apiResponse;
            }
            catch (Exception ex)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorType = ErrorType.Exception
                };
            }
        }

        public async Task<ApiResponse<TVm>> Delete<TVm>(string url)
        {
            var isConneted = CheckConnection();
            if (!isConneted.IsSuccess)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = isConneted.IsSuccess,
                    Message = isConneted.Message,
                    ErrorType = isConneted.ErrorType,
                    Result = default(TVm)
                };
            }

            try
            {
                var client = GetHttpClient();
                var response = await client.DeleteAsync(url).ConfigureAwait(false);
                var resultAsJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TVm>>(resultAsJson);
                return apiResponse;
            }
            catch (Exception ex)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorType = ErrorType.Exception
                };
            }
            //    if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound)
            //        return new ApiResponse
            //        {
            //            IsSuccess = false,
            //            Message = response.ReasonPhrase
            //        };

            //    var resultJson = await response.Content.ReadAsStringAsync();
            //    var result = JsonConvert.DeserializeObject<List<TVm>>(resultJson);

            //    return new ApiResponse
            //    {
            //        IsSuccess = true,
            //        Result = result
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new ApiResponse
            //    {
            //        IsSuccess = false,
            //        Message = ex.Message
            //    };
            //}
        }
        public async Task<ApiResponse<TVm>> Delete<TVm>(string endpoint, int id)
        {
            var isConneted = CheckConnection();
            if (!isConneted.IsSuccess)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = isConneted.IsSuccess,
                    Message = isConneted.Message,
                    ErrorType = isConneted.ErrorType,
                    Result = default(TVm)
                };
            }
            try
            {
                var url = $"{endpoint}/{id}";
                var client = GetHttpClient();
                var response = await client.DeleteAsync(url).ConfigureAwait(false);
                var resultAsJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TVm>>(resultAsJson);
                return apiResponse;
            }
            catch (Exception ex)
            {
                return new ApiResponse<TVm>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorType = ErrorType.Exception
                };
            }
            //if (response.StatusCode == HttpStatusCode.BadRequest)
            //    return new ApiResponse
            //    {
            //        IsSuccess = false,
            //        Message = response.ReasonPhrase
            //    };

            //var resultJson = await response.Content.ReadAsStringAsync();
            //var result = JsonConvert.DeserializeObject<TVm>(resultJson);

            //return new ApiResponse
            //{
            //    IsSuccess = true,
            //    Result = result
            //};
            // }
            //catch (Exception ex)
            //{
            //    return new ApiResponse
            //    {
            //        IsSuccess = false,
            //        Message = ex.Message
            //    };
            //}
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
