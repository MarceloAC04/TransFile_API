using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace TranslateAPI.Domains
{
    public class HistoryProps
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("docs")]
        public string? Docs { get; set; }

        [BsonElement("type")]
        public string? Type { get; set; }

        [BsonElement("dateModify")]
        public DateTime Date { get; set; }

        //Referencia tabela de usuario
        [BsonElement("idUser")]
        public string? idUser { get; set; }
    }
}
