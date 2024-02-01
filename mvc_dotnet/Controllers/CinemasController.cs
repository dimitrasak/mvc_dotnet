using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc_dotnet.Models;
using X.PagedList;

namespace mvc_dotnet.Controllers
{
    public class CinemasController : Controller
    {
        private readonly TicketServiceContext _context;

        public CinemasController(TicketServiceContext context)
        {
            _context = context;
        }

        // GET: Cinemas
        public async Task<IActionResult> Index(int? page, string? search)
        {
            ViewData["CurrentFilter"] = search;
            var cinemas = from c in _context.Cinemas
                            select c;
            if (!String.IsNullOrEmpty(search))
            {
                cinemas = cinemas.Where(c => c.Name.Contains(search));
            }
            // Pagination for users
            if (page != null && page < 1)
            {
                page = 1;
            }

            int PageSize = 10;
            var cinemasData = await cinemas.ToPagedListAsync(page ?? 1, PageSize);
            return View(cinemasData);
        }

        // GET: Cinemas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cinemas == null)
            {
                return NotFound();
            }

            var cinema = await _context.Cinemas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cinema == null)
            {
                return NotFound();
            }

            return View(cinema);
        }

        // GET: Cinemas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cinemas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Seats,_3d")] Cinema cinema)
        {
            if (ModelState.IsValid)
            {
                cinema.Id = GetNextId();
                _context.Add(cinema);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cinema added successfully.";
                return RedirectToAction("Home", "ContentAdmins");
            }
            return View(cinema);
        }

        private int GetNextId()
        {
            // Logic to get the next available ID, for example, querying the database or using a counter
            // Replace this with your actual logic
            int nextId = _context.Cinemas.Max(c => (int?)c.Id) ?? 0;
            return nextId + 1;
        }

        // GET: Cinemas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cinemas == null)
            {
                return NotFound();
            }

            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null)
            {
                return NotFound();
            }
            return View(cinema);
        }

        // POST: Cinemas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Seats,_3d")] Cinema cinema)
        {
            if (id != cinema.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cinema);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CinemaExists(cinema.Id))
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
            return View(cinema);
        }

        // GET: Cinemas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cinemas == null)
            {
                return NotFound();
            }

            var cinema = await _context.Cinemas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cinema == null)
            {
                return NotFound();
            }

            return View(cinema);
        }

        // POST: Cinemas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cinemas == null)
            {
                return Problem("Entity set 'TicketServiceContext.Cinemas'  is null.");
            }
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema != null)
            {
                _context.Cinemas.Remove(cinema);
            }
            
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cinema deleted successfully.";
            return RedirectToAction("Home", "ContentAdmins");
        }

        private bool CinemaExists(int id)
        {
          return (_context.Cinemas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
