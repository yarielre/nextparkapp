using System;
namespace NextPark.Mobile.Core.Settings
{
    public static class ApiSettings
    {
        public static string BaseUrl = "http://inside-web.azurewebsites.net"; //"http://vps519741.ovh.net:5043"; //"http://vps519741.ovh.net:5043"; // "http://192.168.0.10/Inside.Web"; // "http://192.168.1.53/Inside.Web";//"http://149.202.41.48:5041";  //"http://10.0.2.2:5041"; //"http://149.202.41.48:5041";

        public static string AuthEndPoint => string.Format("{0}/api/auth", BaseUrl);
        public static string ProfileEndPoint => string.Format("{0}/api/profile", BaseUrl);
        public static string OrdersEndPoint => string.Format("{0}/api/order", BaseUrl);
        public static string ParkingCategoriesEndPoint => string.Format("{0}/api/parkingcategories", BaseUrl);
        public static string ParkingTypesEndPoint => string.Format("{0}/api/parkingtypes", BaseUrl);
        public static string ParkingsEndPoint => string.Format("{0}/api/parkings", BaseUrl);
        public static string EventsEndPoint => string.Format("{0}/api/events", BaseUrl);

    }
}
