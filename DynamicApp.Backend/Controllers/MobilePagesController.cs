using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DynamicApp.Backend.Models;

namespace DynamicApp.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly PagesContext _context;

        public PagesController(PagesContext context)
        {
            _context = context;
        }

        // GET: api/Pages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MobilePage>>> GetPages()
        {
            return await _context.Pages.Include(p => p.Sections).ToListAsync();
        }

        // GET: api/Pages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MobilePage>> GetMobilePage(Guid id)
        {
            var mobilePage = await _context.Pages.FirstOrDefaultAsync(p => p.Guid == id);

            if (mobilePage == null)
            {
                return NotFound();
            }

            return mobilePage;
        }

        // PUT: api/Pages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMobilePage(Guid id, MobilePage mobilePage)
        {
            if (id != mobilePage.Guid)
            {
                return BadRequest();
            }

            _context.Entry(mobilePage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MobilePageExists(id))
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

        // POST: api/Pages
        [HttpPost]
        public async Task<ActionResult<MobilePage>> PostMobilePage(MobilePage mobilePage)
        {
            _context.Pages.Add(mobilePage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMobilePage", new { id = mobilePage.MobilePageId }, mobilePage);
        }

        // DELETE: api/Pages/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MobilePage>> DeleteMobilePage(Guid id)
        {
            var mobilePage = await _context.Pages.FirstOrDefaultAsync(p => p.Guid == id);
            if (mobilePage == null)
            {
                return NotFound();
            }

            _context.Pages.Remove(mobilePage);
            await _context.SaveChangesAsync();

            return mobilePage;
        }

        private bool MobilePageExists(Guid id)
        {
            return _context.Pages.Any(e => e.Guid == id);
        }
    }
}
