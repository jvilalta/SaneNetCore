using System;

namespace scanner.Hubs
{
    public interface IScanner
    {

        void MergeResults(ScannerOptions options, ScanResults results);
        ScanResults Scan(ScannerOptions options);

        event EventHandler<DataEventArgs> ErrorReceived;
        event EventHandler<DataEventArgs> ProgressReceived;
    }
}