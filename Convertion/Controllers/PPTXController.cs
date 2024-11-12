using Aspose.Slides;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TesteAspore7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PPTXController : ControllerBase
    {
        [HttpPost("PptxToPdf")]
        public async Task<IActionResult> ConvertPptxToPdf(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Nenhum arquivo foi enviado.");
                }
                string downloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";

                if (!Directory.Exists(downloadsFolder))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "A pasta de Downloads não foi encontrada.");
                }

                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);

                string outputFilePath = Path.Combine(downloadsFolder, fileNameWithoutExtension + ".pdf");

                string tempFilePath = Path.Combine(downloadsFolder, "tempPresentation.pptx");

                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                using (Presentation presentation = new Presentation(tempFilePath))
                {
                    presentation.Save(outputFilePath, Aspose.Slides.Export.SaveFormat.Pdf);
                }

                System.IO.File.Delete(tempFilePath);

                return File(System.IO.File.ReadAllBytes(outputFilePath), "application/pdf", Path.GetFileName(outputFilePath));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro: {ex.Message}");
            }
        }
    }
}
