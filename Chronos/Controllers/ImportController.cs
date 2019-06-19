using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Chronos.ImportHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            ValidateFile(files);
            var dataTable = ParseToDataTable(files);

            return Ok(new { rows = dataTable.Rows.Count, columns = dataTable.Columns.Count});
        }

        private static DataTable ParseToDataTable(List<IFormFile> files)
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