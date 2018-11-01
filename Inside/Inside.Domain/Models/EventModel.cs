using System;
using System.Collections.Generic;
using Inside.Domain.Enums;

namespace Inside.Domain.Models
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