namespace Inside.Web.MapperTools
{
    //public class MonthOfYearsResolver:IValueResolver<Event,EventModel,List<MyMonthOfYear>>
    ////{
    ////    public List<MyMonthOfYear> Resolve(Event source, EventModel destination, List<MyMonthOfYear> destMember, ResolutionContext context)
    ////    {
    ////        List<MyMonthOfYear> list = new List<MyMonthOfYear>();
    ////        string[] monthRepeat = source.MonthRepeat.Split('-');

    ////        foreach (var month in monthRepeat)
    ////        {
    ////            MyMonthOfYear monthAsEnum = (MyMonthOfYear) Enum.Parse(typeof(MyMonthOfYear), month);
    ////            list.Add(monthAsEnum);
    ////        }
    ////        return list;
    ////    }
    //}

    //public class MotnthOfYearFromViewModelResolver:IValueResolver<EventModel, Event,string>
    //{
    //    public string Resolve(EventModel source, Event destination, string destMember, ResolutionContext context)
    //    {
    //        return String.Join('-', source.MonthRepeat);
    //    }
    //}
}
