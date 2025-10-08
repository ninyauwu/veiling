﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Data;
using Veiling.Server.Models;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KavelsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KavelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/kavels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kavel>>> GetKavels()
        {
            return await _context.Kavels
                .Include(k => k.Gebruiker)
                .Include(k => k.Veiling)
                .Include(k => k.Bedrijf)
                .Include(k => k.Leverancier)
                .Include(k => k.Boden)
                .ToListAsync();
        }

        // GET: api/kavels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Kavel>> GetKavel(int id)
        {
            var kavel = await _context.Kavels
                .Include(k => k.Gebruiker)
                .Include(k => k.Veiling)
                .Include(k => k.Bedrijf)
                .Include(k => k.Leverancier)
                .Include(k => k.Boden)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kavel == null)
            {
                return NotFound();
            }

            return kavel;
        }

        // POST: api/kavels
        [HttpPost]
        public async Task<ActionResult<Kavel>> CreateKavel(Kavel kavel)
        {
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
    }
}