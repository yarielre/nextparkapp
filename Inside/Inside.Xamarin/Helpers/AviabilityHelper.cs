using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inside.Domain.Enums;
using Inside.Domain.Models;

namespace Inside.Xamarin.Helpers
{
    public static class AviabilityHelper
    {
        public static bool IsAvialable(ParkingModel parking)
        {
            if (parking.IsRented)
            {
                return false;
            }
            //Revisar los tipos de parqueo en la BD.
            if (parking.ParkingType.Type == "By Hours")
            {
                TimeSpan currentTime = DateTime.Now.TimeOfDay;
                var day = (int)DateTime.Now.DayOfWeek;
                MyDayOfWeek currentDay = (MyDayOfWeek)day;
                if (parking.ParkingEvent.EndTime < currentTime)
                {
                    return false;
                }
                if (!parking.ParkingEvent.WeekRepeat.Exists(p => p == currentDay))
                {
                    return false;
                }
            }

            //Revisar los tipos de parqueo en la BD.
            if (parking.ParkingType.Type == "For Month")
            {
                if (parking.IsRented)
                {
                    return false;
                }
                if (DateTime.Now < parking.ParkingEvent.StartDate && DateTime.Now > parking.ParkingEvent.EndDate)
                {
                    return false;
                }
            }
            
          
            return true;
        }

        public static string GetAviability(ParkingModel parking)
        {
            string aviability;
            //Revisar los tipos de parqueo en la BD.
            if (parking.ParkingType.Type == "By Hours")
            {
                var timeSubstract = parking.ParkingEvent.EndTime - DateTime.Now.TimeOfDay;
                 aviability = $"{timeSubstract.Hours}h:{timeSubstract.Minutes}m";
            }
            else
            {
                var daySubstract = parking.ParkingEvent.EndDate - DateTime.Now;
                aviability = $"{daySubstract.Days}d {daySubstract.Hours}h:{daySubstract.Minutes}m";
            }
                return aviability;

        }

        public static string GetRentTimeLeft(ParkingModel parking)
        {
            string aviability ="";
            if (parking!=null)
            {
                //Revisar los tipos de parqueo en la BD.
                if (parking.ParkingType.Type == "By Hours")
                {
                    var timeSubstract = parking.RentByHour.EndTime - DateTime.Now.TimeOfDay;
                    aviability = $"{timeSubstract.Hours}h : {timeSubstract.Minutes}m : {timeSubstract.Seconds}s";
                }
                else
                {
                    var daySubstract = parking.RentForMonth.EndDate - DateTime.Now;
                       aviability = $"{daySubstract.Days}d {daySubstract.Hours}h : {daySubstract.Minutes}m";
                   // aviability = $"{daySubstract.Days} days left";
                }
            }
            
            return aviability;
        }

    }
}
