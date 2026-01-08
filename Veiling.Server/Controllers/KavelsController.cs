using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KavelsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<KavelsController> _logger;

        public KavelsController(AppDbContext context, IWebHostEnvironment environment, ILogger<KavelsController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        [HttpPost("upload-image")]
        public async Task<ActionResult<object>> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest(new { error = "Geen afbeelding opgegeven" });

            const int maxFileSize = 5 * 1024 * 1024;
            if (image.Length > maxFileSize)
                return BadRequest(new { error = "Afbeelding mag maximaal 5MB zijn" });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            
            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                return BadRequest(new { error = "Ongeldig bestandstype" });

            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedContentTypes.Contains(image.ContentType.ToLowerInvariant()))
                return BadRequest(new { error = "Ongeldig content-type" });

            try
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "kavels");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    await image.CopyToAsync(fileStream);

                var imageUrl = $"/uploads/kavels/{uniqueFileName}";
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij uploaden afbeelding");
                return StatusCode(500, new { error = "Uploaden mislukt" });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kavel>>> GetKavels()
        {
            try
            {
                return await _context.Kavels
                    .Include(k => k.Veiling)
                    .Include(k => k.Leverancier)
                    .Include(k => k.Boden)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij ophalen kavels");
                return StatusCode(500, new { error = "Fout bij ophalen kavels" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Kavel>> GetKavel(int id)
        {
            if (id <= 0)
                return BadRequest(new { error = "Ongeldig kavel ID" });

            var kavel = await _context.Kavels
                .Include(k => k.Veiling)
                .Include(k => k.Leverancier)
                .Include(k => k.Boden)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kavel == null)
                return NotFound(new { error = $"Kavel met ID {id} niet gevonden" });

            return kavel;
        }

        [HttpGet("veiling/{veilingId}")]
        public async Task<ActionResult<IEnumerable<Kavel>>> GetKavelsByVeiling(int veilingId)
        {
            return await _context.Kavels
                .Where(k => k.VeilingId == veilingId)
                .Include(k => k.Leverancier)
                .Include(k => k.Boden)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Kavel>> CreateKavel([FromBody] CreateKavelDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { error = "Validatie mislukt", details = errors });
            }

            var veilingExists = await _context.Veilingen.AnyAsync(v => v.Id == dto.VeilingId);
            if (!veilingExists)
                return BadRequest(new { error = $"Veiling met ID {dto.VeilingId} bestaat niet" });

            try
            {
                var kavel = new Kavel
                {
                    Naam = dto.Naam,
                    Beschrijving = dto.Description,
                    Foto = dto.ImageUrl ?? string.Empty,
                    MinimumPrijs = dto.MinimumPrijs,
                    HoeveelheidContainers = dto.Aantal,
                    Keurcode = dto.Ql,
                    VeilingId = dto.VeilingId,
                    StageOfMaturity = dto.Stadium,
                    LengteVanBloemen = dto.Lengte,
                    Kavelkleur = dto.Kleur,
                    Fustcode = dto.Fustcode,
                    AantalProductenPerContainer = dto.AantalProductenPerContainer,
                    GewichtVanBloemen = dto.GewichtVanBloemen,
                    ArtikelKenmerken = string.Empty,
                    MaximumPrijs = dto.MinimumPrijs * 5f,
                    GekochtPrijs = 0,
                    GekochteContainers = 0,
                    Minimumhoeveelheid = 1,
                    Karnummer = 0,
                    Rijnummer = 0,
                    NgsCode = 'A',
                    GeldPerTickCode = string.Empty
                };

                _context.Kavels.Add(kavel);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetKavel), new { id = kavel.Id }, kavel);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database fout bij aanmaken kavel");
                return StatusCode(500, new { error = "Database fout" });
            }
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKavel(int id, Kavel kavel)
        {
            if (id != kavel.Id)
                return BadRequest(new { error = "ID mismatch" });

            if (kavel.MinimumPrijs <= 0)
                return BadRequest(new { error = "Minimum prijs moet groter dan 0 zijn" });

            if (kavel.HoeveelheidContainers <= 0)
                return BadRequest(new { error = "Hoeveelheid containers moet groter dan 0 zijn" });

            if (kavel.AantalProductenPerContainer <= 0)
                return BadRequest(new { error = "Aantal producten per container moet groter dan 0 zijn" });

            if (kavel.GewichtVanBloemen <= 0)
                return BadRequest(new { error = "Gewicht moet groter dan 0 zijn" });

            if (kavel.LengteVanBloemen <= 0)
                return BadRequest(new { error = "Lengte moet groter dan 0 zijn" });

            _context.Entry(kavel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await KavelExists(id))
                    return NotFound();
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKavel(int id)
        {
            var kavel = await _context.Kavels.FindAsync(id);
            if (kavel == null)
                return NotFound();

            _context.Kavels.Remove(kavel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> KavelExists(int id)
        {
            return await _context.Kavels.AnyAsync(k => k.Id == id);
        }
    }

    public class CreateKavelDto
    {
        [Required(ErrorMessage = "Naam is verplicht")]
        public string Naam { get; set; } = string.Empty;

        [Required(ErrorMessage = "Beschrijving is verplicht")]
        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Prijs is verplicht")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Prijs moet groter dan 0 zijn")]
        public float MinimumPrijs { get; set; }

        [Required(ErrorMessage = "Aantal is verplicht")]
        [Range(1, int.MaxValue, ErrorMessage = "Aantal moet groter dan 0 zijn")]
        public int Aantal { get; set; }

        [Required(ErrorMessage = "Ql is verplicht")]
        public string Ql { get; set; } = string.Empty;

        [Required(ErrorMessage = "Veiling ID is verplicht")]
        public int VeilingId { get; set; }

        [Required(ErrorMessage = "Stadium is verplicht")]
        public string Stadium { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lengte is verplicht")]
        [Range(0.01, float.MaxValue, ErrorMessage = "Lengte moet groter dan 0 zijn")]
        public float Lengte { get; set; }

        [Required(ErrorMessage = "Kleur is verplicht")]
        public string Kleur { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fustcode is verplicht")]
        public int Fustcode { get; set; }

        [Required(ErrorMessage = "Aantal per container is verplicht")]
        [Range(1, int.MaxValue, ErrorMessage = "Aantal per container moet groter dan 0 zijn")]
        public int AantalProductenPerContainer { get; set; }

        [Required(ErrorMessage = "Gewicht is verplicht")]
        [Range(0.01, float.MaxValue, ErrorMessage = "Gewicht moet groter dan 0 zijn")]
        public float GewichtVanBloemen { get; set; }
    }
}