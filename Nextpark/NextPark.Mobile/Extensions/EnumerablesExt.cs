using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NextPark.Mobile.Extensions
{
    public static class Extensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> list)
        {
            var collection = new ObservableCollection<T>();
            foreach (var item in list)
                collection.Add(item);

            return collection;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IList<T> list)
        {
            var collection = new ObservableCollection<T>();
            foreach (var item in list)
                collection.Add(item);

            return collection;
        }
    }
}