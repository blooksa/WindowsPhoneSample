using System;
using System.Collections.Generic;

namespace WindowsPhoneSample.Core.Extensions
{
    public static class ListExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);
            }
        }

        public static void AddRange<T>(this  IList<T> source, IEnumerable<T> items)
        {
            foreach (T element in items)
            {
                source.Add(element);
            }
        }
    }
}