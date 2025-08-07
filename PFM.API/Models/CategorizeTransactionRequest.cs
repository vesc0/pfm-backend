using System.Text.Json.Serialization;

namespace PFM.API.Models
{
    public class CategorizeTransactionRequest
    {
        [JsonPropertyName("catcode")]
        public string CatCode { get; set; } = default!;
    }
}