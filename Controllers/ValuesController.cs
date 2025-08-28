using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace sarw_rp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        public ValuesController(HttpClient client)
        {
            Client = client;
        }

        public HttpClient Client { get; }

        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<IActionResult> Get(string query)
        {
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={query.Replace(" ", "%20")}&key=AIzaSyAOnwQQ2lhVc0f8WAGHFFzcvwH4tRwtw84";
            var data = await Client.GetAsync(url);
            var content = await data.Content.ReadAsByteArrayAsync();
            return new FileContentResult(content, data.Content.Headers.ContentType?.ToString() ?? "application/octet-stream");
        }
    }
}
