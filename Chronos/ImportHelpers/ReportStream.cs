using System.Data;
using System.IO;
using System.Linq;

namespace Chronos.ImportHelpers
{
    public class ReportStream
    {
        public static DataTable ParseToDataTable(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
                return ParseToDataTable(streamReader);
        }

        private static DataTable ParseToDataTable(StreamReader streamReader)
        {
            if(streamReader.EndOfStream) return new DataTable();

            var dataTable = InitializeDataTableWithColumns(streamReader);

            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();

                var values = line?.Split(',');

                if (values?.Length != dataTable.Columns.Count)
                    continue;

                var row = dataTable.NewRow();
                row.ItemArray = values;
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        private static DataTable InitializeDataTableWithColumns(StreamReader streamReader)
        {
            var firstLine = streamReader.ReadLine();
            if (firstLine == null) return new DataTable();

            var columnNames = firstLine.Split(",").ToList();

            var dataTable = new DataTable();
            columnNames.ForEach(columnName => dataTable.Columns.Add(columnName));

            return dataTable;
        }
    }
}