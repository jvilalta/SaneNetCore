using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using scanner.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace scanner.Hubs
{
    public class ScannerHub : Hub
    {
        private readonly ScannerOptions _Options;
        private readonly ILogger<ScannerHub> _Logger;
        private readonly IScanner _Scanner;
        private readonly IFileService _FileService;

        public ScannerHub(IOptions<ScannerOptions> options, ILogger<ScannerHub> logger, IScanner scanner, IFileService fileService)
        {
            _Options = options.Value;
            _Logger = logger;
            _Scanner = scanner;
            _FileService = fileService;
        }
        public async Task Scan(bool duplex, bool merge, string scannerMode)
        {
            _Logger.LogInformation($"Duplex: {duplex}");
            _Logger.LogInformation($"Merge: {merge}");
            try
            {
                _Logger.LogInformation("Scanning");
                await Clients.All.SendAsync("Scanning", true);
                await Clients.All.SendAsync("Progress",$"Scanning as {scannerMode}.");
                _Scanner.ProgressReceived += _Scanner_ProgressReceived;
                _Scanner.ErrorReceived += _Scanner_ErrorReceived;
                _Options.ScannerMode = scannerMode;
                _Options.Duplex = duplex;
                if (scannerMode != "Lineart")
                {
                    _Options.FileFormat = "jpeg";
                }
                var results = _Scanner.Scan(_Options);
                if (merge)
                {
                    _Logger.LogInformation("Start merge");
                    await Clients.All.SendAsync("Progress", $"Starting PDF merge. Please wait, this will take a few seconds. You can continue scanning.");

                    Task.Run(() => _Scanner.MergeResults(_Options, results));
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _Scanner.ProgressReceived -= _Scanner_ProgressReceived;
                _Scanner.ErrorReceived -= _Scanner_ErrorReceived;
                await Clients.All.SendAsync("Scanning", false);
            }
        }

        private void _Scanner_ErrorReceived(object sender, DataEventArgs e)
        {
            var error = e.Data;
            Clients.All.SendAsync("Error", error).GetAwaiter().GetResult();
        }

        private void _Scanner_ProgressReceived(object sender, DataEventArgs e)
        {
            var progress = e.Data;
            Clients.All.SendAsync("Progress", progress).GetAwaiter().GetResult();
        }

        public async Task GetDirectories()
        {
            var files = _FileService.GetDirectories().Select(s => new { Name = new DirectoryInfo(s).Name, CreationTime = new DirectoryInfo(s).CreationTime }).OrderByDescending(s => s.CreationTime)
                .Select(s => new { id = s.Name, text = $"{s.CreationTime.ToLongDateString()} {s.CreationTime.ToLongTimeString()}", files = _FileService.GetFiles(s.Name) });
            await Clients.Caller.SendAsync("UpdateDirectories", files);
        }
    }
}
