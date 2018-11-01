using System;
using Inside.Domain.Core;

namespace Inside.Domain.Entities
{
    public class Event : BaseEntity
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string WeekRepeat { get; set; }

        //   public string MonthRepeat { get; set; }
    }
}