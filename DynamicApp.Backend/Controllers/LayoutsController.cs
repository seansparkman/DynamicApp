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
    public class LayoutsController : ControllerBase
    {
        private readonly PagesContext _context;

        public LayoutsController(PagesContext context)
        {
            _context = context;
        }

        // GET: api/Layouts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Layout>>> GetLayouts()
        {
            return await _context.Layouts.ToListAsync();
        }

        // GET: api/Layouts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Layout>> GetLayout(Guid id)
        {
            var layout = await _context.Layouts.FirstOrDefaultAsync(l => l.Guid == id);

            if (layout == null)
            {
                return NotFound();
            }

            return layout;
        }

        // PUT: api/Layouts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLayout(Guid id, Layout layout)
        {
            if (id != layout.Guid)
            {
                return BadRequest();
            }

            _context.Entry(layout).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LayoutExists(id))
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

        // POST: api/Layouts
        [HttpPost]
        public async Task<ActionResult<Layout>> PostLayout(Layout layout)
        {
            _context.Layouts.Add(layout);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLayout", new { id = layout.LayoutId }, layout);
        }

        // DELETE: api/Layouts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Layout>> DeleteLayout(Guid id)
        {
            var layout = await _context.Layouts.FirstOrDefaultAsync(l => l.Guid == id);
            if (layout == null)
            {
                return NotFound();
            }

            _context.Layouts.Remove(layout);
            await _context.SaveChangesAsync();

            return layout;
        }

        private bool LayoutExists(Guid id)
        {
            return _context.Layouts.Any(e => e.Guid == id);
        }
    }
}
