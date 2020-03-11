using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TrainingAzure.WebApp.Models
{
    public class UploadImageModel
    {
        [Required]
        public IFormFile File { get; set; }
    }
}