using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using mvc_dotnet.Models;
using X.PagedList;

namespace mvc_dotnet.Controllers
{
    public class ProvolesController : Controller
    {
        private readonly TicketServiceContext _context;

        public ProvolesController(TicketServiceContext context)
        {
            _context = context;
        }

        // GET: Provoles
        public async Task<IActionResult> Index(int? page)
        {
            var ticketServiceContext = _context.Provoles.Include(p => p.Cinemas).Include(p => p.ContentAdmin).Include(p => p.Movies);
            var provoles = await ticketServiceContext.ToListAsync();
            // Pagination for users
            if (page != null && page < 1)
            {
                page = 1;
            }

            int PageSize = 10;
            var provolesData = await provoles.ToPagedListAsync(page ?? 1, PageSize);
            return View(provolesData);
        }
        
        // GET: Provoles/Details/5
        public async Task<IActionResult> Details(int? cinemasId, int? moviesId, string moviesName)
        {
            if (cinemasId == null || moviesId == null || moviesName == null|| _context.Provoles == null)
            {
                return NotFound();
            }

            var provole = await _context.Provoles
                .Include(p => p.Cinemas)
                .Include(p => p.ContentAdmin)
                .Include(p => p.Movies)
                .Include(p => p.Reservations)
                .FirstOrDefaultAsync(m => m.CinemasId == cinemasId && m.MoviesId == moviesId && m.MoviesName == moviesName);
            if (provole == null)
            {
                return NotFound();
            }

            string seatsAsString = provole.Cinemas.Seats;
            int totalSeats;
            // Calculate available seats
            totalSeats = int.Parse(provole.Cinemas.Seats);
            int reservedSeats = (int)provole.Reservations.Sum(r => r.NumberOfSeats);
            int availableSeats = totalSeats - reservedSeats;

            // Pass the calculated value to the view
            ViewData["AvailableSeats"] = availableSeats;


            return View(provole);
        }

        // GET: Provoles/Create
        public IActionResult Create()
        {
            CreateProvoleModel provoleModel = new CreateProvoleModel();
            ViewData["CinemasId"] = new SelectList(_context.Cinemas, "Id", "Name");
            ViewData["ContentAdminId"] = new SelectList(_context.ContentAdmins, "Id", "UserUsername");
            ViewData["MoviesId"] = new SelectList(_context.Movies, "Id", "Id");
            ViewData["MoviesName"] = new SelectList(_context.Movies, "Id", "Name");

            return View(provoleModel);
        }

        // POST: Provoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProvoleModel createProvole)
        {
            try
            {
                //var selectedMovie = await _context.Movies.FirstOrDefaultAsync(m => m.Name == createProvole.MoviesName);
                //int selectedMoviesId = createProvole.MoviesId;
                string selectedMoviesName = createProvole.MoviesName;
                int selectedMovieId = int.Parse(selectedMoviesName);


                Console.WriteLine("Selected MoviesId: " + selectedMovieId);
                Console.WriteLine("Selected MoviesName: " + selectedMoviesName);

                
                var selectedMovie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == selectedMovieId);
                                

                // Assign MovieId to the Provole entity
                createProvole.MoviesId = selectedMovie.Id;
                createProvole.MoviesName = selectedMovie.Name;
                createProvole.Id = GetNextProvoleId();
                //createProvole.DatetimeColumn = createProvole.DatetimeColumn; 

                var provoleEntity = new Provole(createProvole);
                provoleEntity.DatetimeColumn = createProvole.DatetimeColumn;

                if (_context.Provoles.Any(p => p.MoviesId == createProvole.MoviesId && p.CinemasId == createProvole.CinemasId))
                {
                    ModelState.AddModelError(string.Empty, "There already exists a screening of this Movie to this Cinema!");
                    //reload the dropdowns or set other ViewData as needed
                    ViewData["CinemasId"] = new SelectList(_context.Cinemas, "Id", "Name", createProvole.CinemasId);
                    ViewData["ContentAdminId"] = new SelectList(_context.ContentAdmins, "Id", "UserUsername", createProvole.ContentAdminId);
                    ViewData["MoviesName"] = new SelectList(_context.Movies, "Id", "Name", createProvole.MoviesName);
                    return View(createProvole);
                }

                if (selectedMovie == null)
                {
                    //case where the selected movie is not found (optional)
                    ModelState.AddModelError(string.Empty, "Selected movie not found.");
                    ViewData["CinemasId"] = new SelectList(_context.Cinemas, "Id", "Name", createProvole.CinemasId);
                    ViewData["ContentAdminId"] = new SelectList(_context.ContentAdmins, "Id", "UserUsername", createProvole.ContentAdminId);
                    ViewData["MoviesName"] = new SelectList(_context.Movies, "Id", "Name", createProvole.MoviesName);
                    return View(createProvole);
                }
                if (ModelState.IsValid)
                {                    
                    
                    Console.WriteLine("inside if: " + provoleEntity.Id + ", " + provoleEntity.CinemasId + ", " + provoleEntity.MoviesId + ", " + provoleEntity.MoviesName + ", " + provoleEntity.ContentAdminId+ ", " +provoleEntity.DatetimeColumn);

                    _context.Provoles.Add(provoleEntity);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                } 

                Console.WriteLine("outside if: " + provoleEntity.Id + ", " + provoleEntity.CinemasId + ", " + provoleEntity.MoviesId + ", " + provoleEntity.MoviesName + ", " + provoleEntity.ContentAdminId);
                ViewData["CinemasId"] = new SelectList(_context.Cinemas, "Id", "Name", createProvole.CinemasId);
                ViewData["ContentAdminId"] = new SelectList(_context.ContentAdmins, "Id", "UserUsername", createProvole.ContentAdminId);
                ViewData["MoviesName"] = new SelectList(_context.Movies, "Id", "Name", createProvole.MoviesName);
                return View(createProvole);
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                //handle other exceptions if needed
                return View(createProvole);
            }
        }

        //method to fetch the next available value for id column
        private int GetNextProvoleId()
        {
            int nextId = _context.Provoles.Max(p => (int?)p.Id) ?? 0;
            return nextId + 1;
        }

        // GET: Provoles/Edit/5
        public async Task<IActionResult> Edit(int? cinemasId, int? moviesId, string moviesName)
        {
            if (cinemasId == null || moviesId == null || string.IsNullOrEmpty(moviesName))
            {
                return NotFound();
            }

            var provole = await _context.Provoles.FindAsync(cinemasId, moviesId, moviesName);
            if (provole == null)
            {
                return NotFound();
            }

            //ViewData["CinemasId"] = new SelectList(_context.Cinemas, "Id", "Name", provole.CinemasId);
            ViewData["ContentAdminId"] = new SelectList(_context.ContentAdmins, "Id", "UserUsername", provole.ContentAdminId);
            ViewData["DateTimeColumn"] = provole.DatetimeColumn;
            // Set MoviesName to the selected movie's name for display in the Edit form
            ViewData["MoviesName"] = provole.MoviesName;

            var editProvoleModel = new CreateProvoleModel
            {
                Id = provole.Id,
                CinemasId = provole.CinemasId,
                MoviesId = provole.MoviesId,
                MoviesName = provole.MoviesName,
                ContentAdminId = provole.ContentAdminId,
                DatetimeColumn = provole.DatetimeColumn
                // Populate other properties as needed
            };

            return View(editProvoleModel);
        }

        // POST: Provoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int cinemasId, int moviesId, string moviesName, CreateProvoleModel editProvole)
        {
            if (cinemasId != editProvole.CinemasId || moviesId != editProvole.MoviesId || moviesName != editProvole.MoviesName)
            {
                return NotFound();
            }

            try
            {
                var existingProvole = await _context.Provoles
                    .FindAsync(cinemasId, moviesId, moviesName);

                if (existingProvole == null)
                {
                    return NotFound();
                }

                // Update existingProvole properties with the values from editProvole
                existingProvole.CinemasId = editProvole.CinemasId;
                existingProvole.MoviesId = editProvole.MoviesId;
                existingProvole.MoviesName = editProvole.MoviesName.ToString();
                existingProvole.ContentAdminId = editProvole.ContentAdminId;
                existingProvole.DatetimeColumn = editProvole.DatetimeColumn;

                // Validate the model state
                if (ModelState.IsValid)
                {
                    _context.Entry(existingProvole).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                // If ModelState is not valid, reload the necessary ViewData and return to the view
                ViewData["CinemasId"] = new SelectList(_context.Cinemas, "Id", "Name", editProvole.CinemasId);
                ViewData["ContentAdminId"] = new SelectList(_context.ContentAdmins, "Id", "UserUsername", editProvole.ContentAdminId);
                ViewData["MoviesName"] = new SelectList(_context.Movies, "Id", "Name", editProvole.MoviesName);
                ViewData["DateTimeColumn"] = editProvole.DatetimeColumn;

                return View(editProvole);
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                // Handle other exceptions if needed
                return View(editProvole);
            }
        } 


        // POST: Provoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.        
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int cinemasId, int moviesId, string moviesName,CreateProvoleModel editProvole)
        {
            try
            {
                var existingProvoleModel = new CreateProvoleModel
                {
                    Id = editProvole.Id,
                    CinemasId = editProvole.CinemasId,
                    MoviesId = editProvole.MoviesId,
                    MoviesName = editProvole.MoviesName,
                    ContentAdminId = editProvole.ContentAdminId
                    // Populate other properties as needed
                };
                // Find the existing Provole based on the provided key values
                //var existingProvole = await _context.Provoles
                    //.FindAsync(cinemasId, moviesId, moviesName);

                // If the existing Provole is not found, return NotFound
                if (existingProvoleModel == null)
                {
                    Console.WriteLine("Non Existing Provole");

                }
                // Validate the model state
                if (ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is valid.");

                    // Debug output to check the values OI ALLAGES 
                    Console.WriteLine($"CinemasId: {editProvole.CinemasId}, MoviesId: {editProvole.MoviesId}, MoviesName: {editProvole.MoviesName}");

                    // Check if there is another record with the same values for CinemasId, MoviesId, and MoviesName
                    var existingDuplicate = await _context.Provoles
                        .AnyAsync(p => p.CinemasId == editProvole.CinemasId &&
                                       p.MoviesId == editProvole.MoviesId &&
                                       p.MoviesName == editProvole.MoviesName &&
                                       p.Id != editProvole.Id);

                    // If another record with the same values exists, return an error
                    if (existingDuplicate)
                    {
                        ModelState.AddModelError(string.Empty, "A record with the same values already exists.");
                        Console.WriteLine("Duplicate record found.");
                        return View(editProvole);
                    }

                    

                    // Create a new entity with updated values from the model
                    var newProvole = new Provole();

                    // Remove the existing entity
                    _context.Provoles.Remove(existingProvoleModel);

                    // Add the new entity to the context
                    _context.Provoles.Add(newProvole);

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    Console.WriteLine("Changes saved successfully.");

                    // Redirect to the Index action
                    return RedirectToAction(nameof(Index));
                }

                // If ModelState is not valid, reload the necessary ViewData and return to the view
                ViewData["CinemasId"] = new SelectList(_context.Cinemas, "Id", "Name", editProvole.CinemasId);
                ViewData["ContentAdminId"] = new SelectList(_context.ContentAdmins, "Id", "UserUsername", editProvole.ContentAdminId);

                Console.WriteLine("ModelState is not valid.");

                return View(editProvole);
            }
            catch (DbUpdateException ex)
            {
                // Log the exception and handle other exceptions if needed
                Console.WriteLine("Exception: " + ex.Message);
                return View(editProvole);
            }
        }*/



        // GET: Provoles/Delete/5
        public async Task<IActionResult> Delete(int? cinemasId, int? moviesId, string moviesName)
        {
            if (cinemasId == null || moviesId == null || moviesName == null || _context.Provoles == null)
            {
                return NotFound();
            }

            var provole = await _context.Provoles
                .Include(p => p.Cinemas)
                .Include(p => p.ContentAdmin)
                .Include(p => p.Movies)
                .FirstOrDefaultAsync(m => m.CinemasId == cinemasId && m.MoviesId == moviesId && m.MoviesName == moviesName) ;
            if (provole == null)
            {
                return NotFound();
            }

            return View(provole);
        }

        // POST: Provoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? cinemasId, int? moviesId, string moviesName)
        {
            if (_context.Provoles == null)
            {
                return Problem("Entity set 'TicketServiceContext.Provoles'  is null.");
            }
            var provole = await _context.Provoles.FindAsync(cinemasId, moviesId, moviesName);
            if (provole != null)
            {
                _context.Provoles.Remove(provole);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProvoleExists(int id)
        {
          return (_context.Provoles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
