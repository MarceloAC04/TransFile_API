using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Text;

namespace TranslateAPI.Services.AzureTranslator
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureTranslatorController : ControllerBase
    {

        private readonly IAzureTranslatorInterface _AzureRepository;

        public AzureTranslatorController()
        {
            _AzureRepository = new AzureTranslatorRepository();
        }

        [HttpGet("UploadedFiles")]
        public async Task<string> GetBlobs()

        {
            try
            {
                return (await _AzureRepository.GetFiles());


            }
            catch (Exception)
            {

                throw;
            }


        }

        [HttpPost("TranslateDocuments")]
        public IActionResult Translation(string language)
        {
            try
            {
                _AzureRepository.Translate(language);
                return Ok();
            }
            catch (Exception)
            {

                throw;
            }


        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadDocuments(IList<IFormFile> file)
        {
            try
            {
                await _AzureRepository.UploadDocuments(file);
                return Ok();
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }



        }

        
        [HttpDelete("DeleteByName")]
        public async Task<IActionResult> Delete(string file)
        {
            try
            {
                await _AzureRepository.DeleteFile(file);
                return Ok();
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }



        }

        [HttpDelete("DeleteFiles")]
        public async Task<IActionResult> DeleteAll()
        {
            try
            {
                await _AzureRepository.DeleteAllFiles();
                return Ok();
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }



        }

        [HttpGet("DownloadFiles")]
        public async Task<IActionResult> DowloadFile()
        {
            string _connectionString = "";

            BlobContainerClient blobContainerClient = new BlobContainerClient(_connectionString, "traductions");

            var blobs = blobContainerClient.GetBlobs();

            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var blob in blobs)
                    {

                        var fileDowloadZip = zipArchive.CreateEntry(blob.Name);
                        var blobClient = blobContainerClient.GetBlobClient(blob.Name);

                        using (var fileStream = fileDowloadZip.Open())
                        {

                            await blobClient.DownloadToAsync(fileStream);


                        }


                    }

                }
                memoryStream.Position = 0;
                var file = File(memoryStream.ToArray(), "application/zip", "TemplateFiles.zip");  
                await _AzureRepository.DeleteAllFilesTraductions();
                return file;


            }







        }

    }
}
