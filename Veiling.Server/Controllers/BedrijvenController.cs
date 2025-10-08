﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Data;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BedrijvenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BedrijvenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/bedrijven
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bedrijf>>> GetBedrijven()
        {
            return await _context.Bedrijven
                .Include(b => b.Kavels)
                .ToListAsync();
        }

        // GET: api/bedrijven/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bedrijf>> GetBedrijf(int id)
        {
            var bedrijf = await _context.Bedrijven
                .Include(b => b.Kavels)
                .FirstOrDefaultAsync(b => b.Bedrijfscode == id);

            if (bedrijf == null)
            {
                return NotFound();
            }

            return bedrijf;
        }

        // POST: api/bedrijven
        [HttpPost]
        public async Task<ActionResult<Bedrijf>> CreateBedrijf(Bedrijf bedrijf)
        {
            _context.Bedrijven.Add(bedrijf);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBedrijf), new { id = bedrijf.Bedrijfscode }, bedrijf);
        }

        // PUT: api/bedrijven/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBedrijf(int id, Bedrijf bedrijf)
        {
            if (id != bedrijf.Bedrijfscode)
            {
                return BadRequest();
            }

            _context.Entry(bedrijf).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BedrijfExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/bedrijven/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBedrijf(int id)
        {
            var bedrijf = await _context.Bedrijven.FindAsync(id);
            if (bedrijf == null)
            {
                return NotFound();
            }

            _context.Bedrijven.Remove(bedrijf);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BedrijfExists(int id)
        {
            return _context.Bedrijven.Any(b => b.Bedrijfscode == id);
        }
    }
}