using System;
using System.Collections.Generic;
using System.Text;
using Inside.Xamarin.Interfaces;
using Inside.Xamarin.Resources;
using Xamarin.Forms;

namespace Inside.Xamarin.Helpers
{
    public static class Languages
    {
        static Languages()
        {
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            Resource.Culture = ci;
            DependencyService.Get<ILocalize>().SetLocale(ci);
        }

        public static string GeneralYes
        {
            get { return Resource.GeneralYes; }
        }
        public static string GeneralNo
        {
            get { return Resource.GeneralNo; }
        }
        public static string GeneralError
        {
            get { return Resource.GeneralError; }
        }
        public static string GeneralCheckInternetConnection
        {
            get { return Resource.GeneralCheckInternetConnection; }
        }
        public static string GeneralAccept
        {
            get { return Resource.GeneralAccept; }
        }
        public static string GeneralCancel
        {
            get { return Resource.GeneralCancel; }
        }
        public static string GeneralConfirm
        {
            get { return Resource.GeneralConfirm; }
        }
        public static string GeneralCongratulations
        {
            get { return Resource.GeneralCongratulations; }
        }
        public static string GeneralServerError
        {
            get { return Resource.GeneralServerError; }
        }
        public static string GeneralPasswordError
        {
            get { return Resource.GeneralPasswordError; }
        }
        public static string GeneralLogin
        {
            get { return Resource.GeneralLogin; }
        }
        public static string GeneralRegister
        {
            get { return Resource.GeneralRegister; }
        }
        public static string GeneralCreate
        {
            get { return Resource.GeneralCreate; }
        }
        public static string GeneralEdit
        {
            get { return Resource.GeneralEdit; }
        }
        public static string GeneralSave
        {
            get { return Resource.GeneralSave; }
        }
        public static string GeneralSuccess
        {
            get { return Resource.GeneralSuccess; }
        }

        public static string MasterItemLogout
        {
            get { return Resource.MasterItemLogout; }
        }
        public static string MasterItemLogoutAlert
        {
            get { return Resource.MasterItemLogoutAlert; }
        }
        public static string MasterItemLogoutHeaderAlert
        {
            get { return Resource.MasterItemLogoutHeaderAlert; }
        }

        public static string MasterItemEditProfile
        {
            get { return Resource.MasterItemEditProfile; }
        }
        public static string MasterItemEditProfileAlert
        {
            get { return Resource.MasterItemEditProfileAlert; }
        }
        public static string MasterItemEditProfileHeaderAlert
        {
            get { return Resource.MasterItemEditProfileHeaderAlert; }
        }

        public static string MasterItemChangePassword
        {
            get { return Resource.MasterItemChangePassword; }
        }
        public static string MasterItemChangePasswordAlert
        {
            get { return Resource.MasterItemChangePasswordAlert; }
        }
        public static string MasterItemChangePasswordHeaderAlert
        {
            get { return Resource.MasterItemChangePasswordHeaderAlert; }
        }

        public static string ParkingCreateAddAlert
        {
            get { return Resource.ParkingCreateAddAlert; }
        }
        public static string ParkingCreateTakePhotoAlert
        {
            get { return Resource.ParkingCreateTakePhotoAlert; }
        }
        public static string ParkingCreateTakePhotoHeaderAlert
        {
            get { return Resource.ParkingCreateTakePhotoHeaderAlert; }
        }
        public static string ParkingCreatePickPhotoHeaderAlert
        {
            get { return Resource.ParkingCreatePickPhotoHeaderAlert; }
        }
        public static string ParkingCreatePickPhotoAlert
        {
            get { return Resource.ParkingCreatePickPhotoAlert; }
        }
        public static string ParkingCategoryLoadAlert
        {
            get { return Resource.ParkingCategoryLoadAlert; }
        }
        public static string ParkingCreateAddPhotoHeaderAlert
        {
            get { return Resource.ParkingCreateAddPhotoHeaderAlert; }
        }
        public static string ParkingCreateAddPhotoOption1
        {
            get { return Resource.ParkingCreateAddPhotoOption1; }
        }
        public static string ParkingCreateAddPhotoOption2
        {
            get { return Resource.ParkingCreateAddPhotoOption2; }
        }
        public static string ParkingEditAlert
        {
            get { return Resource.ParkingEditAlert; }
        }
        public static string ParkingInfoRentAlert
        {
            get { return Resource.ParkingInfoRentAlert; }
        }
        public static string ParkingTypeLoadAlert
        {
            get { return Resource.ParkingTypeLoadAlert; }
        }

        public static string RegisterAddUser
        {
            get { return Resource.RegisterAddUser; }
        }
        public static string RegisterSuccess
        {
            get { return Resource.RegisterSuccess; }
        }
        public static string RegisterEmailEmptyAlert
        {
            get { return Resource.RegisterEmailEmptyAlert; }
        }
        public static string RegisterPasswordConfAlert
        {
            get { return Resource.RegisterPasswordConfAlert; }
        }
        public static string RegisterUserEmptyAlert
        {
            get { return Resource.RegisterUserEmptyAlert; }
        }

        public static string EditProfileSuccessAlert
        {
            get { return Resource.EditProfileSuccessAlert; }
        }
        public static string EditProfileOldPasswordAlert
        {
            get { return Resource.EditProfileOldPasswordAlert; }
        }
        public static string ChangePasswordSuccessAlert
        {
            get { return Resource.ChangePasswordSuccessAlert; }
        }

        public static string LoginErrorServerAlert
        {
            get { return Resource.LoginErrorServerAlert; }
        }
        public static string LoginUserPasswordIncorrectAlert
        {
            get { return Resource.LoginUserPasswordIncorrectAlert; }
        }
        public static string LoginPasswordAlert
        {
            get { return Resource.LoginPasswordAlert; }
        }
        public static string LoginUserNameAlert
        {
            get { return Resource.LoginUserNameAlert; }
        }

        public static string TabsHome
        {
            get { return Resource.TabsHome; }
        }
        public static string TabsCoin
        {
            get { return Resource.TabsCoin; }
        }
        public static string TabsRentByHours
        {
            get { return Resource.TabsRentByHours; }
        }
        public static string TabsRentForMonth
        {
            get { return Resource.TabsRentForMonth; }
        }
        public static string CoinBuyNow
        {
            get { return Resource.CoinBuyNow; }
        }
        public static string CoinBuyConfirmAlert
        {
            get { return Resource.CoinBuyConfirmAlert; }
        }
        public static string CoinBuyCongratulationsAlert
        {
            get { return Resource.CoinBuyCongratulationsAlert; }
        }

        public static string XamlLoginUsernameLabel
        {
            get
            {
                return Resource.XamlLoginUsernameLabel;
            }
        }
        public static string XamlLoginUsernamePlaceholder
        {
            get
            {
                return Resource.XamlLoginUsernamePlaceholder;
            }
        }
        public static string XamlLoginPasswordLabel
        {
            get
            {
                return Resource.XamlLoginPasswordLabel;
            }
        }
        public static string XamlLoginPasswordPlaceholder
        {
            get
            {
                return Resource.XamlLoginPasswordPlaceholder;
            }
        }
        public static string XamlLoginRemeberMeText
        {
            get
            {
                return Resource.XamlLoginRemeberMeText;
            }
        }
        public static string XamlParkingCreatePriceLabel
        {
            get
            {
                return Resource.XamlParkingCreatePriceLabel;
            }
        }
        public static string XamlParkingCreatePricePlaceholder
        {
            get
            {
                return Resource.XamlParkingCreatePricePlaceholder;
            }
        }
        public static string XamlParkingCreateCategoryLabel
        {
            get { return Resource.XamlParkingCreateCategoryLabel; }
        }
        public static string XamlParkingCreateCategoryPicker
        {
            get { return Resource.XamlParkingCreateCategoryPicker; }
        }
        public static string XamlParkingCreateTypeLabel
        {
            get { return Resource.XamlParkingCreateTypeLabel; }
        }
        public static string XamlParkingCreateTypePicker
        {
            get { return Resource.XamlParkingCreateTypePicker; }
        }
        public static string XamlParkingInfoRentDateLabel
        {
            get { return Resource.XamlParkingInfoRentDateLabel; }
        }
        public static string XamlParkingInfoRentFromLabel
        {
            get { return Resource.XamlParkingInfoRentFromLabel; }
        }
        public static string XamlParkingInfoRentToLabel
        {
            get { return Resource.XamlParkingInfoRentToLabel; }
        }
        public static string XamlParkingInfoRentLabel
        {
            get { return Resource.XamlParkingInfoRentLabel; }
        }
        public static string XamlCreateParkingPopUpLabel
        {
            get { return Resource.XamlCreateParkingPopUpLabel; }
        }
        public static string XamlRegisterEmailLabel
        {
            get { return Resource.XamlRegisterEmailLabel; }
        }
        public static string XamlRegisterEmailPlaceholder
        {
            get { return Resource.XamlRegisterEmailPlaceholder; }
        }
        public static string XamlRegisterConfimPasswordLabel
        {
            get { return Resource.XamlRegisterConfimPasswordLabel; }
        }
        public static string XamlRegisterConfimPasswordPlaceholder
        {
            get { return Resource.XamlRegisterConfimPasswordPlaceholder; }
        }
        public static string XamlEditProfileNewPasswordLabel
        {
            get { return Resource.XamlEditProfileNewPasswordLabel; }
        }
        public static string XamlEditProfileOldPasswordLabel
        {
            get { return Resource.XamlEditProfileOldPasswordLabel; }
        }
        public static string XamlEditProfileNewPasswordPlaceholder
        {
            get { return Resource.XamlEditProfileNewPasswordPlaceholder; }
        }
        public static string XamlEditProfileOldPasswordPlaceholder
        {
            get { return Resource.XamlEditProfileOldPasswordPlaceholder; }
        }
        public static string XamlEditNamePlaceholder
        {
            get { return Resource.XamlEditNamePlaceholder; }
        }
    }
}
