using System;
using System.Threading.Tasks;

namespace NextPark.Mobile.Services
{
    public interface IPushService
    {
        void Start();
        /*
         * CODE to go directly on order page if push of an expired order arrives when app were closed
         * 
        //Task<bool> NotificationHandler();
         */
    }
}
