using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc_dotnet.Models;
using X.PagedList;

namespace mvc_dotnet.Controllers
{
    public class MoviesController : Controller
    {
        private readonly TicketServiceContext _context;

        public MoviesController(TicketServiceContext context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index(int? page, string? search)
        {
            ViewData["CurrentFilter"] = search;
            var movies = from m in _context.Movies.Include(m => m.ContentAdmin)
                         select m;
            //var movies = await ticketServiceContext.ToListAsync();
            //search
            if (!String.IsNullOrEmpty(search))
            {
                movies = movies.Where(m => m.Name.Contains(search)
                                       || m.Director.Contains(search));
            }
            movies = movies.OrderBy(m => m.Name).ThenBy(m => m.Director);

            // Pagination for users
            if (page != null && page < 1)
            {
                page = 1;
            }

            int PageSize = 10;
            var moviesData = await movies.ToPagedListAsync(page ?? 1, PageSize);
            return View(moviesData);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id, string name)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.ContentAdmin)
                .FirstOrDefaultAsync(m => m.Id == id && m.Name == name);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewData["ContentAdminId"] = new SelectList(_context.ContentAdmins, "Id", "UserUsername");
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Content,Length,Type,Summary,Director,ContentAdminId")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                movie.Id = GetNextId(); // Assuming GetNextId is a method to get the next available ID
                _context.Add(movie);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Movie added successfully.";

                return RedirectToAction("Home", "ContentAdmins");
            }
            ViewData["ContentAdminId"] = new SelectList(_context.ContentAdmins, "Id", "UserUsername", movie.ContentAdminId);
            return View(movie);
        }

        private int GetNextId()
        {
            // Logic to get the next available ID, for example, querying the database or using a counter
            // Replace this with your actual logic
            int nextId = _context.Movies.Max(m => (int?)m.Id) ?? 0;
            return nextId + 1;
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id, string name)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id, name);
            if (movie == null)
            {
                return NotFound();
            }
            ViewData["ContentAdminId"] = new SelectList(_context.ContentAdmins, "Id", "UserUsername", movie.ContentAdminId);
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string name, [Bind("Id,Name,Content,Length,Type,Summary,Director,ContentAdminId")] Movie movie)
        {
            if (id != movie.Id || name != movie.Name)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
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
            ViewData["ContentAdminId"] = new SelectList(_context.ContentAdmins, "Id", "UserUsername", movie.ContentAdminId);
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id, string name)
        {
            if (id == null || name == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.ContentAdmin)
                .FirstOrDefaultAsync(m => m.Id == id && m.Name == name);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string name)
        {
            if (_context.Movies == null)
            {
                return Problem("Entity set 'TicketServiceContext.Movies'  is null.");
            }
            var movie = await _context.Movies.FindAsync(id, name);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }
            
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Movie deleted successfully.";
            return RedirectToAction("Home", "ContentAdmins");
        }

        private bool MovieExists(int id)
        {
          return (_context.Movies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
