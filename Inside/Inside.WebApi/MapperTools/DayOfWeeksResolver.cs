using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inside.Domain.Entities;
using Inside.Domain.Enum;
using Inside.WebApi.ViewModels;
using MyDayOfWeek = Inside.Domain.Enum.MyDayOfWeek;

namespace Inside.WebApi.MapperTools
{
    public class DayOfWeeksResolver:IValueResolver<Event, EventViewModel, List<MyDayOfWeek>>
    {
        public List<MyDayOfWeek> Resolve(Event source, EventViewModel destination, List<MyDayOfWeek> destMember, ResolutionContext context)
        {
            List<MyDayOfWeek> list = new List<MyDayOfWeek>();
            string[] daysRepeat = source.WeekRepeat.Split('-');
            foreach (var day in daysRepeat)
            {
              MyDayOfWeek dayAsEnum = (MyDayOfWeek) Enum.Parse(typeof(MyDayOfWeek), day);
              list.Add(dayAsEnum);
            }
            ;
            return list;
        }
    }
    public class DayOfWeeksFromViewModelResolver : IValueResolver<EventViewModel, Event, string>
    {
        public string Resolve(EventViewModel source, Event destination, string destMember, ResolutionContext context)
        {
            return String.Join('-',source.WeekRepeat);
        }
    }
}
