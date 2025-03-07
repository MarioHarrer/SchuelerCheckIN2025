using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchuelerCheckIN2025.Data;
using SchuelerCheckIN2025.Models;

namespace SchuelerCheckIN2025.Controllers
{
    public class SchuelerdatensController : Controller
    {
        private readonly SchuelerCheckIN2025Context _context;

        public SchuelerdatensController(SchuelerCheckIN2025Context context)
        {
            _context = context;
        }

        // GET: Schuelerdatens
        public async Task<IActionResult> Index()
        {
            return View(await _context.Schuelerdaten.ToListAsync());
        }

        // GET: Schuelerdatens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schuelerdaten = await _context.Schuelerdaten
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schuelerdaten == null)
            {
                return NotFound();
            }

            return View(schuelerdaten);
        }

        // GET: Schuelerdatens/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Schuelerdatens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,email,schluessel,klasse")] Schuelerdaten schuelerdaten)
        {
            if (ModelState.IsValid)
            {
                _context.Add(schuelerdaten);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(schuelerdaten);
        }

        // GET: Schuelerdatens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schuelerdaten = await _context.Schuelerdaten.FindAsync(id);
            if (schuelerdaten == null)
            {
                return NotFound();
            }
            return View(schuelerdaten);
        }

        // POST: Schuelerdatens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,email,schluessel,klasse")] Schuelerdaten schuelerdaten)
        {
            if (id != schuelerdaten.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schuelerdaten);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SchuelerdatenExists(schuelerdaten.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(schuelerdaten);
        }

        // GET: Schuelerdatens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schuelerdaten = await _context.Schuelerdaten
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schuelerdaten == null)
            {
                return NotFound();
            }

            return View(schuelerdaten);
        }

        // POST: Schuelerdatens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schuelerdaten = await _context.Schuelerdaten.FindAsync(id);
            if (schuelerdaten != null)
            {
                _context.Schuelerdaten.Remove(schuelerdaten);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SchuelerdatenExists(int id)
        {
            return _context.Schuelerdaten.Any(e => e.Id == id);
        }
    }
}
