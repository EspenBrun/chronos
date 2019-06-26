using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Chronos.Models;

namespace Chronos.ImportHelpers
{
    public static class DataTableConverter
    {
        public static List<TimeBlock> ToTimeBlocks(DataTable dataTable)
        {
            var columnName = dataTable.Columns[1].ColumnName;
            var isJibble = columnName == "Team Member";
            var isTimeTracker = columnName == "In";

            if (isTimeTracker)
                return TimeTrackerTimeBlocks(dataTable);

            if (isJibble)
                return JibbleTimeBlocks(dataTable);

            return new List<TimeBlock>();
        }

        private static List<TimeBlock> JibbleTimeBlocks(DataTable dataTable)
        {
            var timeBlocks = new List<TimeBlock>();
            foreach (DataRow row in dataTable.Rows)
            {
                var inOrOut = row["In/Out"].ToString();
                if(inOrOut == "OUT")
                    continue;

                var jibbleDate = row["Date"].ToString();
                var jibbleTime = row["Time"].ToString();
                var jibbleDuration = row["Duration"].ToString();
                var stampIn = DateTimeParser.FromJibble("2018", jibbleDate, jibbleTime);
                var worked = DateTimeParser.TimeSpanFromJibbleDuration(jibbleDuration);

                var block = new TimeBlock
                {
                    In = stampIn,
                    Out = stampIn.Add(worked),
                    Worked = worked
                };
                timeBlocks.Add(block);
            }

            return timeBlocks;
        }

        private static List<TimeBlock> TimeTrackerTimeBlocks(DataTable dataTable)
        {
            var timeBlocks = new List<TimeBlock>();
            foreach (DataRow row in dataTable.Rows)
            {
                timeBlocks.Add(TimeBlockFrom(row));
            }

            return timeBlocks;
        }

        private static TimeBlock TimeBlockFrom(DataRow row)
        {
            var stampIn = DateTime.Parse(row["In"].ToString());
            var stampOut = DateTime.Parse(row["Out"].ToString());
            return new TimeBlock
            {
                In = stampIn,
                Out = stampOut,
                Worked = stampOut.Subtract(stampIn)
            };
        }
    }
}