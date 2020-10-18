using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace scanner.Hubs
{
    public class ScanResults
    {
        public string WorkingDirectory { get; set; }
        public string FolderName { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmss");
        public string[] FileNames { get; set; }
    }
}
