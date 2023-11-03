namespace GQIDS___JSON_reader_1
{
    using Skyline.DataMiner.Analytics.GenericInterface;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Net.Caching;
    using System.IO;

    /// <summary>
    /// Represents the GQI data source.
    /// </summary>
    [GQIMetaData(Name = "JSON Reader")]
    public class JSONReaderDataSource : IGQIDataSource, IGQIInputArguments, IGQIOnInit
    {
        private GQIStringDropdownArgument _fileArg;
        private Reader _reader;

        public GQIArgument[] GetInputArguments()
        {
            var files = Scanner.Scan();
            if (files.Length == 0)
                throw new GenIfException("No JSON files found.");
            _fileArg = new GQIStringDropdownArgument("File", files) { IsRequired = true };
            return new GQIArgument[] { _fileArg };
        }

        public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
        {
            _reader = new Reader(args.GetArgumentValue(_fileArg));
            return new OnArgumentsProcessedOutputArgs();
        }

        public OnInitOutputArgs OnInit(OnInitInputArgs args)
        {
            return new OnInitOutputArgs();
        }

        public GQIColumn[] GetColumns()
        {
            return _reader.GetColumns();
        }

        public GQIPage GetNextPage(GetNextPageInputArgs args)
        {
            return _reader.GetData();
        }
    }
}