using NextPark.Enums;
using System;
using System.Collections.Generic;

namespace NextPark.Models
{
    public class EventModel : BaseModel
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<MyDayOfWeek> WeekRepeat { get; set; }

        //  public List<MyMonthOfYear> MonthRepeat { get; set; }
    }
}