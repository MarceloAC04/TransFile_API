using Azure;
using Azure.AI.Translation.Document;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace TranslateAPI.Services.AzureTranslator
{
    public class AzureTranslatorRepository : IAzureTranslatorInterface
    {

        private string _connectionString = "DefaultEndpointsProtocol=https;AccountName=transfilestoragetest;AccountKey=IsbcuPou48XNu9CG3c1Bxp9ruMLBtGzUJFUcszbR/EaYxrNt2uirawgLM9nZJOn9Do6HtiwFilGT+ASt+YCm0g==;EndpointSuffix=core.windows.net";
        public async Task Translate(string language)
        {




            string endpoint = "https://transfiletest.cognitiveservices.azure.com/";
            string apiKey = "Wvx4nkCTyF7ZbnHdSEpektRj51pdjlWIQKXHnFKRYHniXrOROKr7JQQJ99AKACZoyfiXJ3w3AAAbACOGdu0o";

            var client = new DocumentTranslationClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

            Uri sourceUri = new Uri("https://transfilestoragetest.blob.core.windows.net/filetest?sp=racwdl&st=2024-11-11T13:00:51Z&se=2024-12-20T21:00:51Z&sv=2022-11-02&sr=c&sig=5ZapAfneZ0TYzyF4p2CQ8cOXsPZcE0n5OtGId%2F%2BsPnA%3D");
            Uri targetUri = new Uri("https://transfilestoragetest.blob.core.windows.net/traductions?sp=racwdl&st=2024-11-11T12:46:59Z&se=2024-12-20T20:46:59Z&sv=2022-11-02&sr=c&sig=ia8r%2B5N%2B1gw%2BBXa93xBkqPuL8bayJpz6PSzh3f9Duig%3D");
            var input = new DocumentTranslationInput(sourceUri, targetUri, $"{language}");



            DocumentTranslationOperation operation = await client.StartTranslationAsync(input);

            await operation.WaitForCompletionAsync();

            Console.WriteLine($"  Status: {operation.Status}");
            Console.WriteLine($"  Created on: {operation.CreatedOn}");
            Console.WriteLine($"  Last modified: {operation.LastModified}");
            Console.WriteLine($"  Total documents: {operation.DocumentsTotal}");
            Console.WriteLine($"    Succeeded: {operation.DocumentsSucceeded}");
            Console.WriteLine($"    Failed: {operation.DocumentsFailed}");
            Console.WriteLine($"    In Progress: {operation.DocumentsInProgress}");
            Console.WriteLine($"    Not started: {operation.DocumentsNotStarted}");

            await foreach (DocumentStatusResult document in operation.Value)
            {
                Console.WriteLine($"Document with Id: {document.Id}");
                Console.WriteLine($"  Status:{document.Status}");
                if (document.Status == DocumentTranslationStatus.Succeeded)
                {
                    Console.WriteLine($"  Translated Document Uri: {document.TranslatedDocumentUri}");
                    Console.WriteLine($"  Translated to language code: {document.TranslatedToLanguageCode}.");
                    Console.WriteLine($"  Document source Uri: {document.SourceDocumentUri}");

                   //deleta todos os arquivos após a tradução 
                   await DeleteAllFiles();
                   

                }
                else
                {
                    Console.WriteLine($"  Error Code: {document.Error.Code}");
                    Console.WriteLine($"  Message: {document.Error.Message}");


                }
            }


        }

        public Task DeleteAllFiles()
        {
            BlobContainerClient blobContainerClient = new BlobContainerClient(_connectionString, "filetest");

            var allFiles = blobContainerClient.GetBlobs();

            foreach (var file in allFiles)
            {

                var blobClient = blobContainerClient.GetBlobClient(file.Name);
                blobClient.Delete();


            }

            return Task.CompletedTask;
        }

        public async Task DeleteFile(string fileName)
        {

            BlobContainerClient blobContainerClient = new BlobContainerClient(_connectionString, "filetest");

            BlobClient file = blobContainerClient.GetBlobClient(fileName);

            try
            {

                await file.DeleteAsync();

            }
            catch (RequestFailedException ex)
            {

                if (ex.ErrorCode == BlobErrorCode.BlobNotFound)

                {


                }
            }



        }

        public async Task<string> GetFiles()
        {


            string containerName = "filetest";

            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            StringBuilder blobList = new StringBuilder();


            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                blobList.AppendLine(blobItem.Name);
                blobList.AppendLine(blobItem.Properties.LastModified.ToString());
            }


            return (blobList.ToString());
        }

        public async Task<IActionResult> UploadDocuments(IList<IFormFile> files)
        {
            BlobContainerClient blobContainerClient = new BlobContainerClient(_connectionString, "filetest");

            foreach (IFormFile file in files)
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;
                    await blobContainerClient.UploadBlobAsync(file.FileName, stream);
                }

            }
            return null;
        }


        public Task DeleteAllFilesTraductions()
        {
            BlobContainerClient blobContainerClient = new BlobContainerClient(_connectionString, "traductions");

            var allFiles = blobContainerClient.GetBlobs();

            foreach (var file in allFiles)
            {

                var blobClient = blobContainerClient.GetBlobClient(file.Name);
                blobClient.Delete();


            }

            return Task.CompletedTask;
        }

    }
}
