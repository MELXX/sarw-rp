using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sarw_rp.Models;
using sarw_rp.Models.DbModels;

[Route("api/[controller]")]
[ApiController]
public class HomePageBannerController : ControllerBase
{
    private readonly SarwrpdbContext _sarwrpdbContext;
    private readonly string _uploadsFolder = "uploads/banners";

    public HomePageBannerController(SarwrpdbContext sarwrpdbContext)
    {
        _sarwrpdbContext = sarwrpdbContext;

        // Ensure uploads directory exists
        if (!Directory.Exists(_uploadsFolder))
        {
            Directory.CreateDirectory(_uploadsFolder);
        }
    }

    // GET: api/Banners
    [HttpGet]
    public async Task<IActionResult> GetBanners()
    {
        var banners = await _sarwrpdbContext.Banners.ToListAsync();
        return Ok(banners);
    }

    // GET: api/Banners/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBanner(int id)
    {
        var banner = await _sarwrpdbContext.Banners.FindAsync(id);

        if (banner == null)
        {
            return NotFound();
        }

        return Ok(banner);
    }

    // POST: api/Banners
    [HttpPost]
    public async Task<IActionResult> CreateBanner([FromForm] string title, [FromForm] string place, IFormFile bannerImage)
    {
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(place))
        {
            return BadRequest("Title and place are required");
        }

        var banner = new Banner
        {
            Title = title,
            Place = place,
            ArticleUrl = ""
        };

        if (bannerImage != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bannerImage.FileName);
            var filePath = Path.Combine(_uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await bannerImage.CopyToAsync(stream);
            }

            banner.ArticleUrl = $"/{_uploadsFolder}/{fileName}";
        }

        _sarwrpdbContext.Banners.Add(banner);
        await _sarwrpdbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBanner), new { id = banner.Id }, banner);
    }

    // PUT: api/Banners/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBanner(Guid id, [FromForm] string title, [FromForm] string place, IFormFile bannerImage)
    {
        var banner = await _sarwrpdbContext.Banners.FindAsync(id);
        if (banner == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(title))
        {
            banner.Title = title;
        }

        if (!string.IsNullOrEmpty(place))
        {
            banner.Place = place;
        }

        if (bannerImage != null)
        {
            // Delete old file if exists
            if (!string.IsNullOrEmpty(banner.ArticleUrl) && System.IO.File.Exists(banner.ArticleUrl.TrimStart('/')))
            {
                System.IO.File.Delete(banner.ArticleUrl.TrimStart('/'));
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bannerImage.FileName);
            var filePath = Path.Combine(_uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await bannerImage.CopyToAsync(stream);
            }

            banner.ArticleUrl = $"/{_uploadsFolder}/{fileName}";
        }

        _sarwrpdbContext.Entry(banner).State = EntityState.Modified;

        try
        {
            await _sarwrpdbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BannerExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Banners/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBanner(int id)
    {
        var banner = await _sarwrpdbContext.Banners.FindAsync(id);
        if (banner == null)
        {
            return NotFound();
        }

        // Delete the file if it exists
        if (!string.IsNullOrEmpty(banner.ArticleUrl) && System.IO.File.Exists(banner.ArticleUrl.TrimStart('/')))
        {
            System.IO.File.Delete(banner.ArticleUrl.TrimStart('/'));
        }

        _sarwrpdbContext.Banners.Remove(banner);
        await _sarwrpdbContext.SaveChangesAsync();

        return NoContent();
    }

    private bool BannerExists(Guid id)
    {
        return _sarwrpdbContext.Banners.Any(e => e.Id == id);
    }
}