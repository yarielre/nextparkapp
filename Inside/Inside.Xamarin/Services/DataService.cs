using Inside.Xamarin.Helpers;
using Inside.Xamarin.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inside.Domain.Models;

namespace Inside.Xamarin.Services
{
    public class DataService
    {

        public ApiService InsideApi;
        private static DataService _instance;
        public static DataService GetInstance()
        {
            return _instance ?? new DataService();
        }
        public DataService()
        {
            this.InsideApi = new ApiService();
        }
        
        public async Task<Response> GetParkingTypesModel()
        {

            var parkingTypes = await InsideApi.GetAll<ParkingTypeModel>(HostSetting.ParkingTypeEndPoint);
            
            if (!parkingTypes.IsSuccess)
            {
                return parkingTypes;
            }
            else
            {
                return new Response
                {
                    IsSuccess = parkingTypes.IsSuccess,
                    Message = parkingTypes.Message,
                    Result = new List<ParkingTypeModel>(
                    parkingTypes.Result as List<ParkingTypeModel> ?? throw new InvalidOperationException("Parking Types are null."))
                };
            }
        }
        public async Task<Response> GetParkingCategories()
        {
            var parkingCategories = await InsideApi.GetAll<ParkingCategoryModel>(HostSetting.ParkingCategoryEndPoint);

            if (!parkingCategories.IsSuccess)
            {
               
                return parkingCategories;
            }
            else
            {
                return new Response
                {
                    IsSuccess = parkingCategories.IsSuccess,
                    Message = parkingCategories.Message,
                    Result = new List<ParkingCategoryModel>(
                    parkingCategories.Result as List<ParkingCategoryModel> ?? throw new InvalidOperationException("Parking Categories are null."))
                };

            }
        }
        public async Task<Response> AddParkingEvent(EventModel model)
        {
            var response = await InsideApi.Post<EventModel, EventModel>(HostSetting.EventEndPoint + "/addevent", model);

            if (!response.IsSuccess)
            {
                return response;
            }
            else {
                return new Response
                {
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Result = response.Result as EventModel ?? throw new InvalidOperationException("Event result is null.")
                };
            }
        }
        public async Task<Response> AddCoins(UpdateUserCoinModel model)
        {
            var response = await InsideApi.Post<UpdateUserCoinModel, UserModel>(HostSetting.AddCoins, model);

            if (!response.IsSuccess)
            {
                return response;
            }
            else
            {
                return new Response
                {
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Result = response.Result as UserModel ?? throw new InvalidOperationException("User result is null.")
                };
            }
        }
        public async Task<Response> AddParking(ParkingModel model) {

            var response = await InsideApi.Post<ParkingModel, ParkingModel>(HostSetting.ParkingEndPoint + "/addparking", model);

            if (!response.IsSuccess)
            {
                return response;
            }
            else
            {
                return new Response
                {
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Result = response.Result as ParkingModel ?? throw new InvalidOperationException("Parking result is null.")
                };
            }

        }
        public async Task<Response> EditParking(ParkingModel model)
        {

            var response = await InsideApi.Post<ParkingModel, ParkingModel>(HostSetting.ParkingEndPoint + "/editparking", model);

            if (!response.IsSuccess)
            {
                return response;
            }
            else
            {
                return new Response
                {
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Result = response.Result as ParkingModel ?? throw new InvalidOperationException("Parking result is null.")
                };
            }

        }

        public async Task<Response> GetParkingRentedByHours(int userId)
        {
            var response = await InsideApi.Post<int, ParkingModel>(HostSetting.ParkingEndPoint + "/parkingrentedbyhours", userId);
            if (!response.IsSuccess)
            {
                return response;
            }
            return new Response
            {
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                Result = response.Result as ParkingModel
            };
        }

        public async Task<Response> GetParkingRentedForMonth(int userId)
        {
            var response = await InsideApi.Post<int, ParkingModel>(HostSetting.ParkingEndPoint + "/parkingrentedformonth", userId);
            if (!response.IsSuccess)
            {
                return response;
            }
            return new Response
            {
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                Result = response.Result as ParkingModel
            };
        }
    }
}
