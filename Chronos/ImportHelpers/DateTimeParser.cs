using System;
using System.Globalization;
using Microsoft.AspNetCore.Rewrite.Internal.IISUrlRewrite;

namespace Chronos.ImportHelpers
{
    public static class DateTimeParser
    {
        // Thu-01-Feb,Espen K. Brun,IN,8:30 am,9.25
        public static DateTime FromJibble(string year, string date, string time)
        {
            var fullDateTime = $"{year}-{date} {time}";
            return DateTime.ParseExact(fullDateTime, "yyyy-ddd-dd-MMM h:mm tt", CultureInfo.InvariantCulture);
        }

        public static TimeSpan TimeSpanFromJibbleDuration(string duration)
        {
            var contains = duration.Contains(".");
            if(!contains)
                throw new ArgumentException($"Jibble duration did not contain an expected \".\", was {duration}");

            var durationSplit = duration.Split(".");
            var hours = int.Parse(durationSplit[0]);
            var minutes = int.Parse(durationSplit[1])*60/100;
            return new TimeSpan(hours, minutes, 0);
        }
    }
}