namespace GQIDS___JSON_reader_1
{
    using System;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using Skyline.DataMiner.Analytics.GenericInterface;
    using static GQIDS___JSON_reader_1.JSONDataSource;

    internal class Reader
    {
        private string _file;
        private JSONDataSource _source;
        private GQIColumn[] _columnCache;
        private GQIPage _pageCache;

        public Reader(string file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            _file = Path.Combine(JSONReaderDataSource.DIRECTORY, file);
        }

        internal GQIColumn[] GetColumns()
        {
            if (_columnCache != null)
                return _columnCache;

            EnsureRead();
            _columnCache = ParseColumns();
            return _columnCache;
        }

        internal GQIPage GetData()
        {
            if (_pageCache != null)
                return _pageCache;

            EnsureRead();
            var rows = ParseRows();
            _pageCache = new GQIPage(rows) { HasNextPage = false };
            return _pageCache;
        }

        private void EnsureRead()
        {
            if (_source != null)
                return;

            string jsonContent = File.ReadAllText(_file);
            _source = JsonConvert.DeserializeObject<JSONDataSource>(jsonContent);
        }

        private GQIColumn[] ParseColumns()
        {
            return _source.Columns?.Select(x => ParseColumn(x)).ToArray() ?? new GQIColumn[0];
        }

        private GQIColumn ParseColumn(JSONColumn column)
        {
            if (String.Equals(column.Type, "string", StringComparison.OrdinalIgnoreCase))
                return new GQIStringColumn(column.Name);
            else if (String.Equals(column.Type, "int", StringComparison.OrdinalIgnoreCase))
                return new GQIIntColumn(column.Name);
            else if (String.Equals(column.Type, "datetime", StringComparison.OrdinalIgnoreCase))
                return new GQIDateTimeColumn(column.Name);
            else if (String.Equals(column.Type, "double", StringComparison.OrdinalIgnoreCase))
                return new GQIDoubleColumn(column.Name);
            else if (String.Equals(column.Type, "boolean", StringComparison.OrdinalIgnoreCase))
                return new GQIBooleanColumn(column.Name);
            else
                throw new GenIfException($"Column of type '{column.Type}' not supported.");
        }

        private GQIRow[] ParseRows()
        {
            return _source.Rows?.Select(x => ParseRow(x)).ToArray() ?? new GQIRow[0];
        }

        private GQIRow ParseRow(JSONRow row)
        {
            return new GQIRow(
                row.Cells?.Select((x, i) => ParseCell(x, _columnCache[i])).ToArray() ?? new GQICell[0]
                );
        }

        private GQICell ParseCell(JSONCell cell, GQIColumn column)
        {
            return new GQICell() { Value = ParseValue(cell.Value, column), DisplayValue = cell.DisplayValue };
        }

        private object ParseValue(object value, GQIColumn column)
        {
            try
            {
                if (column is GQIIntColumn)
                {
                    return Convert.ToInt32(value);
                }
                else if (column is GQIStringColumn)
                {
                    return value?.ToString() ?? String.Empty;
                }
                else if (column is GQIDateTimeColumn)
                {
                    var offset = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(value));
                    return offset.DateTime.ToUniversalTime();
                }
                else if (column is GQIDoubleColumn)
                {
                    return Convert.ToDouble(value);
                }
                else if (column is GQIBooleanColumn)
                {
                    return Convert.ToBoolean(value);
                }
                else
                {
                    throw new GenIfException($"Unknown cell type '{column.GetType()}'");
                }
            }
            catch (GenIfException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new GenIfException($"Something went wrong trying to parse the value for column '{column.Name}'. Exception: {ex}");
            }
        }
    }

    public class JSONDataSource
    {
        public int Version { get; set; }

        public JSONColumn[] Columns { get; set; }

        public JSONRow[] Rows { get; set; }

        public class JSONColumn
        {
            public string Name { get; set; }

            public string Type { get; set; }
        }

        public class JSONRow
        {
            public JSONCell[] Cells { get; set; }
        }

        public class JSONCell
        {
            public object Value { get; set; }

            public string DisplayValue { get; set; }
        }
    }
}
