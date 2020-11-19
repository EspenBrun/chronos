using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Chronos.Enums;
using Chronos.ImportHelpers;
using Chronos.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite.Internal.PatternSegments;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : Controller
    {
        private readonly TodoContext Context;

        public ImportController(TodoContext context)
        {
            Context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> List()
        {
            return Ok(await Context.TimeBlocks.OrderByDescending(x => x.In).ToListAsync());
        }

        [HttpGet("average")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> Average()
        {
            var timeBlocks = await Context.TimeBlocks.OrderByDescending(x => x.In).ToListAsync();
            var toIgnore = new DateTime(2018, 10, 26); // day off to visit Kristian in Denmark

            timeBlocks.RemoveAll(t => t.In.Date == toIgnore);

            return Ok(new
            {
                TimeBlocksTotal = AverageTimeBlock(timeBlocks, null, out _),
                TimeBlocks2017 = AverageTimeBlock(timeBlocks, 2017, out var workSpans2017),
                TimeBlocks2018 = AverageTimeBlock(timeBlocks, 2018, out var workSpans2018),
                TimeBlocks2019 = AverageTimeBlock(timeBlocks, 2019, out var workSpans2019),
                workSpans2017,
                workSpans2018,
                workSpans2019
            });
        }

        private static AverageTimeBlock AverageTimeBlock(List<TimeBlock> timeBlocksGiven, int? year, out List<WorkSpan> weekdaysGrouped)
        {
            var timeBlocks = year == null ? timeBlocksGiven : timeBlocksGiven.Where(t => t.In.Year == year).ToList();

            var totalWorked = new TimeSpan(timeBlocks.Sum(t => t.Worked.Ticks));
            var numberOfEntries = timeBlocks.Count;
            var weekends = timeBlocks.Where(t => t.In.DayOfWeek == DayOfWeek.Saturday || t.In.DayOfWeek == DayOfWeek.Sunday || Calendar.DaysOff.Contains(t.In.Date))
                .ToList();
            var weekDays = timeBlocks.Where(t => t.In.DayOfWeek != DayOfWeek.Saturday && t.In.DayOfWeek != DayOfWeek.Sunday && !Calendar.DaysOff.Contains(t.In.Date))
                .ToList();
            var numberOfWeekendEntries = weekends.Count;
            var numberOfWeekdayEntries = weekDays.Count;

            var lunchTicks = new TimeSpan(0, 30, 0).Ticks;
            weekdaysGrouped = weekDays.GroupBy(x => x.In.Date, x => x,
                (key, y) =>
                {
                    var timeSpan = new TimeSpan(y.Sum(t => t.Worked.Ticks) - lunchTicks);
                    return new WorkSpan {Date = key, Worked = timeSpan};
                }).ToList();

            var totalWorkedAfterGroup = new TimeSpan(weekdaysGrouped.Sum(r => r.Worked.Ticks) + weekends.Sum(r => r.Worked.Ticks));

            return new AverageTimeBlock
            {
                Year = year == null ? "Total" : year.ToString(),
                NumberOfEntries = numberOfEntries,
                TotalWorked = totalWorked.TotalHours,
                ExcessHours = totalWorked.TotalHours - Calendar.Aarsverk,
                NumberOfWeekdayEntries = numberOfWeekdayEntries,
                NumberOfWeekendEntries = numberOfWeekendEntries,
                TotalWorkedAfterGroup = totalWorkedAfterGroup.TotalHours,
                AverageWorkDay = new TimeSpan(totalWorkedAfterGroup.Ticks / numberOfWeekdayEntries).TotalHours
            };
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, TimeBlocKPut item)
        {
            if (id != item.Id)
                return BadRequest(ErrorCodes.IdsNotMatching.ToString());

            var stampIn = item.In;
            var stampOut = item.Out;
            var timeBlock = new TimeBlock
            {
                Id = item.Id,
                In = stampIn,
                Out = stampOut,
                Worked = stampOut.Subtract(stampIn)
            };
            Context.Entry(timeBlock).State = EntityState.Modified;
            await Context.SaveChangesAsync();

            return Ok(timeBlock);
        }

        [HttpPost]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            ValidateFile(files);
            var dataTable = ParseToDataTable(files);

            if(dataTable.Rows.Count == 0)
                return Ok(new { rows = dataTable.Rows.Count, columns = dataTable.Columns.Count});

            var timeBlocks = DataTableConverter.ToTimeBlocks(dataTable);

            Context.AddRange(timeBlocks);
            await Context.SaveChangesAsync();

            return Ok(new { rows = dataTable.Rows.Count, columns = dataTable.Columns.Count});
        }

        private static DataTable ParseToDataTable(IReadOnlyList<IFormFile> files)
        {
            using (var stream = files[0].OpenReadStream())
                return ReportStream.ParseToDataTable(stream);
        }

        private static void ValidateFile(List<IFormFile> files)
        {
            if (files == null || !files.Any()) throw new Exception("Invalid file");
        }
    }

    public class TimeBlocKPut
    {
        public int Id { get; set; }
        public DateTime In { get; set; }
        public DateTime Out { get; set; }
    }

    public class AverageTimeBlock
    {
        public string Year { get; set; }
        public int NumberOfEntries { get; set; }
        public double TotalWorked { get; set; }
        public double ExcessHours { get; set; }
        public int NumberOfWeekdayEntries { get; set; }
        public int NumberOfWeekendEntries { get; set; }
        public double AverageWorkDay { get; set; }
        public double TotalWorkedAfterGroup { get; set; }
    }

    public class WorkSpan
    {
        public DateTime Date { get; set; }
        public TimeSpan Worked { get; set; }
    }

    public static class Calendar
    {
        public static List<DateTime> DaysOff => new List<DateTime>
        {
            new DateTime(2019, 5,31), // vacation
            new DateTime(2019, 2,15), // needed to visit Mor
            new DateTime(2019, 2,18), // vacation
            new DateTime(2019, 2,19), // vacation
            new DateTime(2019, 2,20), // vacation
            new DateTime(2019, 2,21), // vacation
            new DateTime(2019, 2,22) // vacation
        };

        public static int Aarsverk => 1695;
        public static double Workday => 7.5;
        public static int AarsverkDays => (int) (Aarsverk / Workday);
        public static int PsWeeksOvertime => 3;
        public static double ExpectedOvertime => Workday * PsWeeksOvertime;
        public static int PsAarsverkDays => AarsverkDays - PsWeeksOvertime * 5;
        public static int LunchTicks => (int) new TimeSpan(0,30,0).Ticks;
        public static int PsAarsverkTotalLunchTicks => PsAarsverkDays * LunchTicks;
    }
}