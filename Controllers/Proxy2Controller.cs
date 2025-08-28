using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class Proxy2Controller : ControllerBase
{
    private readonly HttpClient _httpClient;
    private  string _targetBaseUrl;
    private readonly ILogger<ProxyController> _logger;
    IConfiguration configuration;

    private static readonly HashSet<string> CorsHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Access-Control-Allow-Origin",
        "Access-Control-Allow-Methods",
        "Access-Control-Allow-Headers",
        "Access-Control-Allow-Credentials",
        "Access-Control-Max-Age",
        "Access-Control-Expose-Headers"
    };

    public Proxy2Controller(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ProxyController> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _targetBaseUrl = configuration["TargetBaseUrl2"] ?? throw new ArgumentNullException("TargetBaseUrl configuration is missing");
        _logger = logger;
        this.configuration = configuration;
    }

    [HttpGet("{**path}")]
    [HttpPost("{**path}")]
    [HttpPut("{**path}")]
    [HttpDelete("{**path}")]
    [HttpPatch("{**path}")]
    public async Task<IActionResult> ForwardRequest(string path,[FromQuery] int? urltype)
    {
        try
        {
            var targetUrl = "";
            if (urltype == null || urltype != 1 )
            {

                targetUrl = $"{_targetBaseUrl.TrimEnd('/')}/{path}{HttpContext.Request.QueryString.Value}";
            }
            else
            {
                _targetBaseUrl = configuration["TargetBaseUrl3"] ?? throw new ArgumentNullException("TargetBaseUrl configuration is missing");
                targetUrl = $"{_targetBaseUrl.TrimEnd('/')}/{path}".Replace("/v1","");
            }

            // Create the request message
            var requestMessage = new HttpRequestMessage(new HttpMethod(Request.Method), targetUrl);

            // Copy request headers (excluding CORS headers)
            foreach (var header in Request.Headers)
            {
                if (!header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase) &&
                    !CorsHeaders.Contains(header.Key))
                {
                    requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            // Copy request body for POST/PUT/PATCH
            if (Request.Body != null && (Request.Method == "POST" || Request.Method == "PUT" || Request.Method == "PATCH"))
            {
                var bodyContent = await new StreamReader(Request.Body).ReadToEndAsync();
                requestMessage.Content = new StringContent(bodyContent, System.Text.Encoding.UTF8, Request.ContentType);
            }

            // Copy query string
            var queryString = Request.QueryString.Value;
            if (!string.IsNullOrEmpty(queryString))
            {
                targetUrl += queryString;
            }

            // Send the request
            var response = await _httpClient.SendAsync(requestMessage);

            // Copy response headers (excluding CORS headers)
            foreach (var header in response.Headers)
            {
                if (!CorsHeaders.Contains(header.Key))
                {
                    Response.Headers[header.Key] = header.Value.ToArray();
                }
            }

            // Read response content
            var content = await response.Content.ReadAsByteArrayAsync();

            // Copy content headers (excluding CORS headers)
            foreach (var header in response.Content.Headers)
            {
                if (!CorsHeaders.Contains(header.Key))
                {
                    Response.Headers[header.Key] = header.Value.ToArray();
                }
            }

            // Return the response with original status code
            return new FileContentResult(content, response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forwarding request to {TargetUrl}", $"{_targetBaseUrl}/{path}");
            return StatusCode(500, new { error = "Proxy Error", message = ex.Message });
        }
    }
}