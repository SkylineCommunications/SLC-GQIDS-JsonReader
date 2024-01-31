namespace GQIDS___JSON_reader_1
{
    using GQI___JSON_creator_1;
    using Skyline.DataMiner.Analytics.GenericInterface;

    /// <summary>
    /// Represents the GQI data source.
    /// </summary>
    [GQIMetaData(Name = "JSON Reader")]
    public class JSONReaderDataSource : IGQIDataSource, IGQIInputArguments
    {
        internal const string DIRECTORY = @"C:\Skyline DataMiner\Documents\DMA_COMMON_DOCUMENTS\GQI data sources";
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
            var path = args.GetArgumentValue(_fileArg);
            _reader = ReaderCache.GetReader(path);
            return new OnArgumentsProcessedOutputArgs();
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