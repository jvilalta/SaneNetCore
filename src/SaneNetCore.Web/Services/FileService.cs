using Microsoft.Extensions.Options;
using scanner.Hubs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace scanner.Services
{
    public class FileService : IFileService
    {
        private readonly ScannerOptions _options;

        public FileService(IOptions<ScannerOptions> options)
        {
            _options = options.Value;
        }
        public string[] GetDirectories()
        {
            return Directory.GetDirectories(_options.SharePath);
        }
        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(Path.Combine(_options.SharePath, path)).Select(s => Path.GetFileName(s)).OrderBy(s => s).ToArray();
        }

        public string GetPath(string folder, string file)
        {
            return Path.Combine(_options.SharePath, folder, file);
        }
    }
}
