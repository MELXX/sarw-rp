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

    public class ArticleCreateDto
    {
        //[FromForm] IFormFile article_image, [FromForm] string json_payload
        [JsonPropertyName("json_payload")]
        public string payload { get; set; }
        [JsonPropertyName("content")]
        public IFormFile article_image { get; set; }
    }

}
