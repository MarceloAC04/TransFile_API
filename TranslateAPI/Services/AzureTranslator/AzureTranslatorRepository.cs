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

        private string _connectionString = "";
        public async Task Translate(string language)
        {




            string endpoint = "";
            string apiKey = "";

            var client = new DocumentTranslationClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

            Uri sourceUri = new Uri("");
            Uri targetUri = new Uri("");
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
