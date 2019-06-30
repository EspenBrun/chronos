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
}