using Aspose.Cells;
using Aspose.Words.Tables;
using Aspose.Words;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TesteAspore7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XlmsController : ControllerBase
    {
            [HttpPost("XlsxToPdf")]
            public async Task<IActionResult> ConvertXlsxToPdf(IFormFile file)
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

                    string tempFilePath = Path.Combine(downloadsFolder, "tempDocument.xlsx");

                    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    Workbook workbook = new Workbook(tempFilePath);

                    foreach (Worksheet sheet in workbook.Worksheets)
                    {
                        sheet.PageSetup.Orientation = PageOrientationType.Landscape;

                        sheet.PageSetup.FitToPagesWide = 1;
                        sheet.PageSetup.FitToPagesTall = 1;

                        sheet.PageSetup.LeftMargin = 0.5;
                        sheet.PageSetup.RightMargin = 0.5;
                        sheet.PageSetup.TopMargin = 0.5;
                        sheet.PageSetup.BottomMargin = 0.5;
                    }

                    PdfSaveOptions pdfOptions = new PdfSaveOptions
                    {
                        AllColumnsInOnePagePerSheet = true,
                        OnePagePerSheet = true,
                    };

                    workbook.Save(outputFilePath, pdfOptions);

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


