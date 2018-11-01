using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inside.Domain.Entities;
using Inside.Domain.Enum;
using DayOfWeek = Inside.Domain.Enum;

namespace Inside.WebApi.ViewModels
{
    public class EventViewModel:BaseViewModel
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public List<MyMonthOfYear> MonthRepeat { get; set; }
        public List<DayOfWeek.MyDayOfWeek> WeekRepeat { get; set; }
    }
}
