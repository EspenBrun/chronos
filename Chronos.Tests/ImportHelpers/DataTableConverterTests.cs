using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Chronos.ImportHelpers;
using Xunit;

namespace Chronos.Tests.ImportHelpers
{
    public class DataTableConverterTests
    {
        [Fact]
        public void TwoRowsWithInOut_OneRowReturned()
        {
            const string firstLine = "Date,Team Member,In/Out,Time,Duration";
            var columnNames = firstLine.Split(",").ToList();
            var dataTable = new DataTable();
            columnNames.ForEach(columnName => dataTable.Columns.Add(columnName));

            var lines = new List<string>
            {
                "Thu-01-Feb,Espen K. Brun,IN,8:30 am,9.25",
                "Thu-01-Feb,Espen K. Brun,OUT,5:45 pm"
            };
            lines.ForEach(line =>
            {
                var values = line?.Split(',');

                var row = dataTable.NewRow();
                row.ItemArray = values;
                dataTable.Rows.Add(row);
            });

            var timeBlocks = DataTableConverter.ToTimeBlocks(dataTable);

            Assert.Single(timeBlocks);
            var timeBlock = timeBlocks.Single();
            Assert.Equal(new TimeSpan(9,15, 0), timeBlock.Worked);
        }

        [Fact]
        public void TimeSpanFromJibbleDuration_DoesNotContainDot_ThrowsArgumentException()
        {
            var exception = Assert.Throws<ArgumentException>(() => DateTimeParser.TimeSpanFromJibbleDuration("9"));

            Assert.Contains("Jibble duration did not contain an expected", exception.Message);
        }

        [Fact]
        public void TimeSpanFromJibbleDuration_TimeSpanIsCorrect()
        {
            var timeSpan = DateTimeParser.TimeSpanFromJibbleDuration("9.25");

            Assert.Equal(9, timeSpan.Hours);
            Assert.Equal(15, timeSpan.Minutes);
        }
    }
}