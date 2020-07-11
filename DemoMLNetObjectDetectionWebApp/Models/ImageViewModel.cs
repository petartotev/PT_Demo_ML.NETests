namespace DemoMLNetObjectDetectionWebApp.Models
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ImageViewModel
    {
        public IFormFile File { get; set; }

        public string FileGuidName { get; set; }

        public string FilePathInput { get; set; }

        public string FilePathOutput { get; set; }

        public List<string> TextAnalysis { get; set; }
    }

}
