using System;
using Xunit;

namespace Chronos.Tests
{
    public class General
    {
        [Fact]
        public void TimeSpam_Add_IsAdded()
        {
            var timeSpan = new TimeSpan();

            Assert.Equal(0, timeSpan.Hours);

            var timeSpanToAdd = new TimeSpan(1,0,0);
            timeSpan = timeSpan.Add(timeSpanToAdd);

            Assert.Equal(1, timeSpan.Hours);

            timeSpan = timeSpan.Add(timeSpanToAdd);

            Assert.Equal(2, timeSpan.Hours);
        }
    }
}