namespace DemoMLNetObjectDetectionWebApp.Controllers
{
    using DemoMLNetObjectDetectionConsoleApp.Services;
    using DemoMLNetObjectDetectionWebApp.Models;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class ObjectDetectionController : Controller
    {
        private readonly IObjectDetectService objectDetectService;

        public ObjectDetectionController(IObjectDetectService objectDetectService)
        {
            this.objectDetectService = objectDetectService;
        }

        [HttpGet]
        public IActionResult Detect()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Detect(ImageViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(); 
            }

            string fileRandomGuidName = Guid.NewGuid().ToString();
            string pathProject = @"C:/Users/USER/source/repos/PT_Demo_ML.NET/Solution/Tasks/Tasks.ObjectDetection/";
            string pathLocal = pathProject + @"DemoMLNet.Task.ObjectDetection.Console/assets/images/" + fileRandomGuidName + ".jpg";

            var fileToUploadToDisk = model.File;
            model.FileGuidName = fileRandomGuidName;
            model.FilePathInput = pathLocal;

            var stream = new FileStream(pathLocal, FileMode.Create);

            using (stream)
            {
                await fileToUploadToDisk.CopyToAsync(stream);
            }

            model.TextAnalysis = objectDetectService.GenerateAnalysis(fileRandomGuidName);

            objectDetectService.CopyImageFromLocalToWebDirectory(fileRandomGuidName);

            return this.RedirectToAction("Result", model);
        }

        public IActionResult Result(ImageViewModel model)
        {
            return this.View(model);
        }
    }
}
