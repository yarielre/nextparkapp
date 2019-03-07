using NextPark.Enums;
using NextPark.Enums.Enums;
using System;
using System.Collections.Generic;

namespace NextPark.Models
{
    public class EventModel : BaseModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid RepetitionId { get; set; }
        public DateTime RepetitionEndDate { get; set; }
        public RepetitionType RepetitionType { get; set; }

        public bool EditRepetition { get; set; }

        //Repetitions
        public List<DayOfWeek> WeeklyRepeDayOfWeeks { get; set; }              //List of days of week for weekly repeat
        public List<int> MonthlyRepeatDay { get; set; }                       //List of number day for monthly repeat

        public int ParkingId { get; set; }
    }
}