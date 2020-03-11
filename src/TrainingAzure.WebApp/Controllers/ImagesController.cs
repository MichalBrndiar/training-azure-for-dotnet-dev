using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingAzure.WebApp.Models;
using TrainingAzure.WebApp.Services;

namespace TrainingAzure.WebApp.Controllers
{
    public class ImagesController : Controller
    {
        private readonly ImageService imageService;
        private readonly TelemetryClient telemetryClient;

        public ImagesController(ImageService imageService, TelemetryClient client)
        {
            this.imageService = imageService;
            this.telemetryClient = client;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadImageModel model)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    using var stream = model.File.OpenReadStream();
                    await this.imageService.UploadAsync(model.File.FileName, stream);

                    telemetryClient.TrackEvent($"Image {model.File.FileName} uploaded successfully");

                    TempData["message"] = "Image uploaded successfully";
                }
                catch (Exception ex)
                {
                    telemetryClient.TrackException(ex);
                    throw;
                }
            }

            return RedirectToAction(nameof(Upload));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string imageName)
        {
            await this.imageService.DeleteAsync(imageName);

            return RedirectToAction(nameof(Upload));
        }

        [HttpPost]
        public async Task<IActionResult> ThrowError()
        {
            throw new InvalidOperationException("Making errors is my intention");
        }
    }
}
