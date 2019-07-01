using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

            var numberOfEntries = timeBlocks.Count;
            var weekends = timeBlocks.Where(t => t.In.DayOfWeek == DayOfWeek.Saturday || t.In.DayOfWeek == DayOfWeek.Sunday).ToList();
            var weekDays = timeBlocks.Where(t => t.In.DayOfWeek != DayOfWeek.Saturday && t.In.DayOfWeek != DayOfWeek.Saturday).ToList();
            var numberOfWeekendEntries = weekends.Count;
            var numberOfWeekdayEntries = weekDays.Count;

            var lunchTicks = new TimeSpan(0,30,0).Ticks;
            var weekdaysGrouped = weekDays.GroupBy(x => x.In.Date, x => x,
                (key, y) =>
                {
                    var timeSpan = new TimeSpan(y.Sum(t => t.Worked.Ticks) - lunchTicks);
                    return new {Date = key, Worked = timeSpan};
                }).ToList();

            var weekendsGrouped = weekDays.GroupBy(x => x.In.Date, x => x,
                (key, y) =>
                {
                    var timeSpan = new TimeSpan(y.Sum(t => t.Worked.Ticks));
                    return new {Date = key, Worked = timeSpan};
                }).ToList();

            var totalWorked = new TimeSpan(weekdaysGrouped.Sum(r => r.Worked.Ticks) + weekends.Sum(r => r.Worked.Ticks));
            return Ok( new
            {
                numberOfEntries,
                numberOfWeekdayEntries,
                numberOfWeekendEntries,
                totalWorked,
                AverageWorkDay = new TimeSpan(totalWorked.Ticks/numberOfWeekdayEntries),
                weekdaysGrouped,
                weekendsGrouped
            });
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
}