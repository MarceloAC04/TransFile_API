using Microsoft.AspNetCore.Mvc;

namespace TranslateAPI.Services.AzureTranslator
{
    public interface IAzureTranslatorInterface
    {
        //Funcao de retorno de documentos e data de upload/modificacao
        public Task<string> GetFiles();
        
        //Funcao de upload de documentos no blob container
        public Task<IActionResult> UploadDocuments(IList<IFormFile> file);

        //Funcao de deletar arquivo pelo nome

        public Task DeleteFile(string fileName);

        //Funcao de deletar todos os arquivos
        public Task DeleteAllFiles();

        //Funcao de deletar todos os arquivos das traducoes
        public Task DeleteAllFilesTraductions();


        //Funcao de Traduzir todos os documentos carregados
        public Task Translate(string language);


    }
}
