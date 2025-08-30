using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sarw_rp.DTOs;
using sarw_rp.Models;
using sarw_rp.Models.DbModels;
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
        private SarwrpdbContext _sarwrpdbContext { get; }

        public BannersController(SarwrpdbContext sarwrpdbContext)
        {
            _sarwrpdbContext = sarwrpdbContext;
        }


        [HttpPost("CreateArticle")]
        public async Task<string?> CreateArticle([FromForm] IFormFile article_image, [FromForm] string json_payload)
        {
            var art = JsonSerializer.Deserialize<ArticleDTO>(json_payload);
            var article = new Article()
            {
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Title = art.Title,
                Url = art.Url,
                Summary = art.Summary
            };
            await _sarwrpdbContext.AddAsync<Article>(article);
            await _sarwrpdbContext.SaveChangesAsync();
            return await SendMultipartAsync(article_image, json_payload, @"incident-reports/articles/create","article");
        }

        [HttpPost("CreateBanner")]
        public async Task<string?> CreateBanner([FromForm] IFormFile banner_image, [FromForm] string json_payload)
        {
            //var art = JsonSerializer.Deserialize<Article>(json_payload);
            return await SendMultipartAsync(banner_image, json_payload, @"incident-reports/banners/create","banner");
        }


        public  async Task<string?> SendMultipartAsync(IFormFile article_image, string json_payload,string resourceUri,string imageType)
        {
            var url = "https://api-dev.sarwatch.co.za/api/v1/"+@resourceUri; // replace with real endpoint
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
            form.Add(fileContent, imageType+"_image", article_image.FileName);

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
}
