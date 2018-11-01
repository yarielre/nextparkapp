using System;
using System.Collections.Generic;
using AutoMapper;
using Inside.Domain.Entities;
using Inside.Domain.Models;
using MyDayOfWeek = Inside.Domain.Enums.MyDayOfWeek;

namespace Inside.Web.MapperTools
{
    public class DayOfWeeksResolver:IValueResolver<Event, EventModel, List<MyDayOfWeek>>
    {
        public List<MyDayOfWeek> Resolve(Event source, EventModel destination, List<MyDayOfWeek> destMember, ResolutionContext context)
        {
            List<MyDayOfWeek> list = new List<MyDayOfWeek>();
            if (String.IsNullOrEmpty(source.WeekRepeat))
            {
                return list;
            }
            string[] daysRepeat = source.WeekRepeat.Split('-');
                foreach (var day in daysRepeat)
                {
                    MyDayOfWeek dayAsEnum = (MyDayOfWeek)Enum.Parse(typeof(MyDayOfWeek), day);
                    list.Add(dayAsEnum);
                }
            
             return list;
        }
    }
    public class DayOfWeeksFromViewModelResolver : IValueResolver<EventModel, Event, string>
    {
        public string Resolve(EventModel source, Event destination, string destMember, ResolutionContext context)
        {
            return String.Join('-',source.WeekRepeat);
        }
    }
}
