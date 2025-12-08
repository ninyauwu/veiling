using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KavelsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public KavelsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost("upload-image")]
        public async Task<ActionResult<object>> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest(new { error = "No image provided" });
            }

            try
            {
                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "kavels");
                Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                // Return the URL
                var imageUrl = $"/uploads/kavels/{uniqueFileName}";
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Image upload failed: {ex.Message}" });
            }
        }

        // GET: api/kavels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kavel>>> GetKavels()
        {
            return await _context.Kavels
                .Include(k => k.Veiling)
                .Include(k => k.Leverancier)
                .Include(k => k.Boden)
                .ToListAsync();
        }

        // GET: api/kavels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Kavel>> GetKavel(int id)
        {
            var kavel = await _context.Kavels
                .Include(k => k.Veiling)
                .Include(k => k.Leverancier)
                .Include(k => k.Boden)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kavel == null)
            {
                return NotFound();
            }

            return kavel;
        }

        // GET: api/kavels/veiling/5
        [HttpGet("veiling/{veilingId}")]
        public async Task<ActionResult<IEnumerable<Kavel>>> GetKavelsByVeiling(int veilingId)
        {
            return await _context.Kavels
                .Where(k => k.VeilingId == veilingId)
                .Include(k => k.Leverancier)
                .Include(k => k.Boden)
                .ToListAsync();
        }

        // POST: api/kavels
        [HttpPost]
        public async Task<ActionResult<Kavel>> CreateKavel([FromBody] CreateKavelDto dto)
        {
            if (!IsValidHexColor(dto.Kleur))
            {
                return BadRequest(new { error = "Invalid hex color format. Use #RGB or #RRGGBB." });
            }
            var kavel = new Kavel
            {
                Naam = dto.Naam,
                Beschrijving = dto.Description,
                Foto = dto.ImageUrl, 
                MinimumPrijs = float.Parse(dto.MinimumPrijs), 
                HoeveelheidContainers = int.Parse(dto.Aantal),
                Keurcode = dto.Ql,
                VeilingId = int.Parse(dto.Plaats),
                StageOfMaturity = dto.Stadium,
                LengteVanBloemen = float.Parse(dto.Lengte),
                Kavelkleur = dto.Kleur,
                Fustcode = int.Parse(dto.Fustcode),
                AantalProductenPerContainer = int.Parse(dto.AantalProductenPerContainer),
                GewichtVanBloemen = float.Parse(dto.GewichtVanBloemen)
            };

            _context.Kavels.Add(kavel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetKavel), new { id = kavel.Id }, kavel);
        }

        // PUT: api/kavels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKavel(int id, Kavel kavel)
        {
            if (id != kavel.Id)
            {
                return BadRequest();
            }

            _context.Entry(kavel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KavelExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/kavels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKavel(int id)
        {
            var kavel = await _context.Kavels.FindAsync(id);
            if (kavel == null)
            {
                return NotFound();
            }

            _context.Kavels.Remove(kavel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KavelExists(int id)
        {
            return _context.Kavels.Any(k => k.Id == id);
        }

        private bool IsValidHexColor(string? color)
        {
            if (string.IsNullOrWhiteSpace(color))
                return false;

            // Regex: #RGB, #RRGGBB, #ARGB, #AARRGGBB
            return System.Text.RegularExpressions.Regex.IsMatch(
                color,
                @"^#([A-Fa-f0-9]{3}|[A-Fa-f0-9]{4}|[A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$"
            );
        }

    }
}
