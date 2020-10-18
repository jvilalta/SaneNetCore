using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace scanner.Hubs
{
    public class Scanner : IScanner
    {
        private readonly ILogger<Scanner> _logger;
        public event EventHandler<DataEventArgs> ErrorReceived;
        public event EventHandler<DataEventArgs> ProgressReceived;

        protected virtual void OnProgressReceived(DataEventArgs e)
        {
            EventHandler<DataEventArgs> handler = ProgressReceived;
            handler?.Invoke(this, e);
        }
        protected virtual void OnErrorReceived(DataEventArgs e)
        {
            EventHandler<DataEventArgs> handler = ErrorReceived;
            handler?.Invoke(this, e);
        }
        public Scanner(ILogger<Scanner> logger)
        {
            _logger = logger;
        }

        public ScanResults Scan(ScannerOptions options)
        {
            var results = new ScanResults();
            results.WorkingDirectory = Path.Combine(options.SharePath, results.FolderName);
            Directory.CreateDirectory(results.WorkingDirectory);
            var source = options.Duplex ? "ADF Duplex" : "ADF Front";
            var info = new ProcessStartInfo("scanimage", $"--batch=page-%03d.{options.FileFormat} --format={options.FileFormat} --batch-print -d \"fujitsu:ScanSnap S1500:326926\" --mode={options.ScannerMode} --source=\"{source}\"");
            //var info = new ProcessStartInfo("scanimage", "--batch=page-%03d.png --format=png --batch-print -p -d 'fujitsu:ScanSnap S1500:326926'");
            info.WorkingDirectory = results.WorkingDirectory;
            _logger.LogInformation($"Directory: {info.WorkingDirectory}");
            _logger.LogInformation($"Args: {info.Arguments}");
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.UseShellExecute = false;
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo = info;
                    process.Start();
                    OnProgressReceived(new DataEventArgs("Starting scan"));
                    _logger.LogInformation("Scan process started");
                    process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            return;
                        }
                        OnProgressReceived(new DataEventArgs(e));
                        _logger.LogInformation($"STDOUT: {e.Data}");
                    });
                    process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                     {
                         if (e.Data == null)
                         {
                             return;
                         }
                         OnErrorReceived(new DataEventArgs(e));
                         _logger.LogInformation($"STDERR: {e.Data}");
                     });
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    _logger.LogInformation($"Exit code: {process.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                OnErrorReceived(new DataEventArgs($"Error starting scan process:{ex.Message}"));
            }
            return results;
        }

        public void MergeResults(ScannerOptions options, ScanResults results)
        {
            _logger.LogInformation("Merge process started");
            var timer = Stopwatch.StartNew();
            var workingDirectory = results.WorkingDirectory;
            var files = Directory.GetFiles(workingDirectory).OrderBy(f => f);
            var fileArg = string.Join(" ", files);
            var info = new ProcessStartInfo("convert", $"{fileArg} {results.FolderName}.pdf");
            info.WorkingDirectory = workingDirectory;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.UseShellExecute = false;
            using (var process = new Process())
            {
                process.StartInfo = info;
                process.Start();
                process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (e.Data == null)
                    {
                        return;
                    }
                    _logger.LogInformation($"STDOUT: {e.Data}");
                });
                process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (e.Data == null)
                    {
                        return;
                    }
                    _logger.LogInformation($"STDERR: {e.Data}");
                });
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                _logger.LogInformation($"Exit code: {process.ExitCode}");
                _logger.LogInformation($"Merge time: {timer.Elapsed.TotalSeconds} seconds");
            }
        }
    }
}