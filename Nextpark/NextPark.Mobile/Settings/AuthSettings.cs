using System;
using NextPark.Models;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace NextPark.Mobile.Core.Settings
{
    public static class AuthSettings
    {
        #region Attributes
        private static ISettings AppSettings => CrossSettings.Current;
        private static readonly string stringDefault = string.Empty;
        private static readonly double doubleDefault = 0.0;

        private const string _tokenId = "Token";
        private const string _userId = "UserId";
        private const string _userName = "UserName";
        private const string _apiUrl = "apiUrl";
        private const string _userCoin = "UserCoin";
        #endregion

        #region Properties
        public static UserModel User { get; set; }

        public static string Token
        {
            //TODO: Encrypt & decrypt
            get => AppSettings.GetValueOrDefault(_tokenId, stringDefault);
            set => AppSettings.AddOrUpdateValue(_tokenId, value);
        }
        public static string UserId
        {
            //TODO: Encrypt & decrypt
            get => AppSettings.GetValueOrDefault(_userId, stringDefault);
            set => AppSettings.AddOrUpdateValue(_userId, value);
        }
        public static string UserName
        {
            //TODO: Encrypt & decrypt
            get => AppSettings.GetValueOrDefault(_userName, stringDefault);
            set => AppSettings.AddOrUpdateValue(_userName, value);
        }
        public static double UserCoin
        {
            //TODO: Encrypt & decrypt
            get => AppSettings.GetValueOrDefault(_userCoin, 0.0);
            set => AppSettings.AddOrUpdateValue(_userCoin, value);
        }
        #endregion

        #region Comments
        //Use ip: 10.0.2.2:5041  for default android emulator + current port
        //Use ip: 10.0.3.2:5041  for genymotion + current port
        //Use ip: 149.202.41.48:5041  for online server + current port
        //Use localhost:5041 for phisycal device
        #endregion
    }
}
