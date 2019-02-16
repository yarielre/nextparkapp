﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NextPark.Mobile.Settings;
using NextPark.Mobile.UIModels;
using NextPark.Models;
using Xamarin.Forms.Maps;

namespace NextPark.Mobile.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IApiService _apiService;

        public Position LastMapPosition { get; set; }
        public List<UIParkingModel> ParkingList { get; set; }
        public List<UIParkingModel> UserParkingList { get; set; }
        public List<OrderModel> UserOrderList { get; set; }

        public ProfileService(IApiService apiService)
        {
            _apiService = apiService;
            ParkingList = new List<UIParkingModel>();
            UserParkingList = new List<UIParkingModel>();
            UserOrderList = new List<OrderModel>();
        }

        public async Task<UpdateUserCoinModel> UpdateUserCoins(UpdateUserCoinModel model)
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) throw new Exception("Connessione ad internet assente");

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var endpoint = $"{ApiSettings.ProfileEndPoint}/editcoins";

            var client = _apiService.GetHttpClient();
            var response = await client.PostAsync(endpoint, content);

            if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(string.Format("Bad request: {0}", response.ReasonPhrase) );

            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UpdateUserCoinModel>(resultJson);

            return result == null ? new UpdateUserCoinModel() : result as UpdateUserCoinModel;
        }

        public async Task<EditProfileModel> UpdateUserData(EditProfileModel model)
        {
            var isConneted = await _apiService.CheckConnection();
            if (!isConneted.IsSuccess) throw new Exception("Internet correction error.");

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var endpoint = $"{ApiSettings.ProfileEndPoint}/edit";

            var client = _apiService.GetHttpClient();
            var response = await client.PostAsync(endpoint, content);

            if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(string.Format("Bad request: {0}", response.ReasonPhrase));

            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<EditProfileModel>(resultJson);

            return result == null ? new EditProfileModel() : result as EditProfileModel;
        }

        public UIParkingModel GetParkingById(int searchId)
        {
            foreach (UIParkingModel parking in ParkingList) {
                if (parking.Id == searchId) {
                    return parking;
                }
            }
            return null;
        }

        public UIParkingModel GetUserParkingById(int searchId)
        {
            foreach (UIParkingModel parking in UserParkingList) {
                if (parking.Id == searchId) {
                    return parking;
                }
            }
            return null;
        }

        public OrderModel GetUserOrderById(int searchId)
        {
            foreach (OrderModel order in UserOrderList)
            {
                if (order.Id == searchId)
                {
                    return order;
                }
            }
            return null;
        }
    }
}
