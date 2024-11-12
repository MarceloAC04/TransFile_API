using Aspose.Words;
using Microsoft.AspNetCore.Mvc;

namespace TesteAspore7.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TxtController : ControllerBase
    {
        [HttpPost("txtToPdf")]
        public async Task<IActionResult> ConvertTxtToPdf(IFormFile file)
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

                string tempFilePath = Path.Combine(downloadsFolder, "tempDocument.txt");

                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                Document doc = new Document(tempFilePath);

                doc.Save(outputFilePath, SaveFormat.Pdf);

                System.IO.File.Delete(tempFilePath);

                return File(System.IO.File.ReadAllBytes(outputFilePath), "application/pdf", Path.GetFileName(outputFilePath));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro: {ex.Message}");
            }
        }

        [HttpPost("TxtToDocx")]
        public async Task<IActionResult> ConvertTxtToDocx(IFormFile file)
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

                string outputFilePath = Path.Combine(downloadsFolder, fileNameWithoutExtension + ".docx");

                string tempFilePath = Path.Combine(downloadsFolder, "tempDocument.txt");

                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                Document doc = new Document(tempFilePath);

                doc.Save(outputFilePath, SaveFormat.Docx);

                System.IO.File.Delete(tempFilePath);

                return File(System.IO.File.ReadAllBytes(outputFilePath), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", Path.GetFileName(outputFilePath));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro: {ex.Message}");
            }
        }
    }
}
