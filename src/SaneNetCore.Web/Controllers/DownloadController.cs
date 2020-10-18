using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using scanner.Services;

namespace scanner.Controllers
{
    public class DownloadController : Controller
    {
        private readonly IFileService _FileService;

        public DownloadController(IFileService fileService)
        {
            _FileService = fileService;
        }
        public IActionResult Index(string folder,string file)
        {
            var mediaType = new MediaTypeHeaderValue($"application/{new FileInfo(file).Extension}");
            var result = new PhysicalFileResult(_FileService.GetPath(folder, file),mediaType);
            result.FileDownloadName = file;
            return result;
        }
    }
}
