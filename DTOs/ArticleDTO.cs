using System.Text.Json.Serialization;

namespace sarw_rp.DTOs
{
    public class ArticleDTO
    {
        public Guid? Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("article_url")]
        public string Url { get; set; }

        [JsonPropertyName("content")]
        public string Summary { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
