using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inside.Domain.Entities;
using Inside.Domain.Enum;
using Inside.WebApi.ViewModels;

namespace Inside.WebApi.MapperTools
{
    public class MonthOfYearsResolver:IValueResolver<Event,EventViewModel,List<MyMonthOfYear>>
    {
        public List<MyMonthOfYear> Resolve(Event source, EventViewModel destination, List<MyMonthOfYear> destMember, ResolutionContext context)
        {
            List<MyMonthOfYear> list = new List<MyMonthOfYear>();
            string[] monthRepeat = source.MonthRepeat.Split('-');
            
            foreach (var month in monthRepeat)
            {
                MyMonthOfYear monthAsEnum = (MyMonthOfYear) Enum.Parse(typeof(MyMonthOfYear), month);
                list.Add(monthAsEnum);
            }
            return list;
        }
    }

    public class MotnthOfYearFromViewModelResolver:IValueResolver<EventViewModel, Event,string>
    {
        public string Resolve(EventViewModel source, Event destination, string destMember, ResolutionContext context)
        {
            return String.Join('-', source.MonthRepeat);
        }
    }
}
