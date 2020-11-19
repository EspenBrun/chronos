using Chronos.ImportHelpers;
using Xunit;

namespace Chronos.Tests.ImportHelpers
{
    public class DateTimeParserTests
    {
        [Fact]
        public void CanParseJibbleDateTime()
        {
            // Thu-01-Feb,Espen K. Brun,IN,8:30 am,9.25
            var exception = Record.Exception(() => DateTimeParser.FromJibble("2018", "Thu-01-Feb", "8:30 am"));
            Assert.Null(exception);
        }

        [Fact]
        public void ParsedJibbleDateTimeIsCorrect()
        {
            const string year = "2018";
            const string month = "Feb";
            const string day = "02";
            const string hour = "9";
            const string minute = "30";
            var dateTime = DateTimeParser.FromJibble(year, $"Fri-{day}-{month}", $"{hour}:{minute} am");
            Assert.Equal(year, dateTime.Year.ToString());
            Assert.Equal(2, dateTime.Month);
            Assert.Equal(2, dateTime.Day);
            Assert.Equal(hour, dateTime.Hour.ToString());
            Assert.Equal(minute, dateTime.Minute.ToString());
        }
    }
}