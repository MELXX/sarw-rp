using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace sarw_rp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController : ControllerBase
    {
        private readonly HttpClient http = new HttpClient();

        public BannersController()
        {
                
        }

        [HttpPost("CreateArticle")]
        public async Task<string?> CreateArticle([FromForm] IFormFile article_image, [FromForm] string json_payload)
        {
            //var art = JsonSerializer.Deserialize<Article>(json_payload);
            return await SendMultipartAsync(article_image,json_payload);
        }


        public  async Task<string?> SendMultipartAsync(IFormFile article_image, string json_payload)
        {
            var url = "https://api-dev.sarwatch.co.za/api/v1/incident-reports/articles/create"; // replace with real endpoint
            await using var stream = article_image.OpenReadStream();
            var fileContent = new StreamContent(stream);
            // Optional: auth header

            var tkn = this.Request.Headers["authorization"];

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",tkn.ToString().Split(" ")[1]);

            // Build multipart content
            using var form = new MultipartFormDataContent();


            // 2) Add simple text fields
            var jsonContent = new StringContent(@json_payload, Encoding.UTF8, "application/json");
            form.Add(jsonContent, "json_payload");
            form.Add(fileContent, "article_image", article_image.FileName);

            // Send request
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = form
            };

            using var response = await http.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Upload failed ({(int)response.StatusCode}): {responseBody}");
            }
            return responseBody;
        }
    }

    public class Article
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("article_url")]
        public string ArticleUrl { get; set; }
    }
}
