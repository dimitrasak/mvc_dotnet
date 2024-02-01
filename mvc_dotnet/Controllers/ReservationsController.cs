using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using mvc_dotnet.Models;
using X.PagedList;


namespace mvc_dotnet.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly TicketServiceContext _context;

        public ReservationsController(TicketServiceContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index(int? page, string? search)
        {
            ViewData["CurrentFilter"] = search;
            var reservations = from r in _context.Reservations.Include(r => r.Customers).Include(r => r.Provoles).ThenInclude(r => r.Cinemas)
                               select r;
            //var reservations = await ticketServiceContext.ToListAsync();
            //Search
            if (!String.IsNullOrEmpty(search))
            {
                reservations = reservations.Where(r => r.ProvolesMoviesName.Contains(search));
                                       ;
            }
            // Pagination for users
            if (page != null && page < 1)
            {
                page = 1;
            }

            int PageSize = 10;
            var reservationsData = await reservations.ToPagedListAsync(page ?? 1, PageSize);
            return View(reservationsData);
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Customers)
                .Include(r => r.Provoles)
                .ThenInclude(r => r.Cinemas)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        
        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["CustomersId"] = new SelectList(_context.Customers, "Id", "Name");
            ViewData["ProvolesCinemasId"] = new SelectList(_context.Provoles.Include(p => p.Cinemas), "CinemasId", "Cinemas.Name");
            ViewData["ProvolesMoviesName"] = new SelectList(_context.Provoles.Select(p => p.MoviesName).Distinct().ToList());
            ViewData["AvailableSeats"] = 0; //initialize with 0

            return View();
        }

        [HttpPost]
        public JsonResult GetCinemasByMovie(string movieName)
        {
            var cinemas = _context.Provoles
                .Where(p => p.MoviesName == movieName)
                .Select(p => new { p.CinemasId, CinemaName = p.Cinemas.Name })
                .Distinct()
                .ToList();

            return Json(cinemas);
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumberOfSeats,ProvolesCinemasId,ProvolesMoviesName,CustomersId,Id")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //fetch the selected cinema and movie details
                    var selectedCinemasId = reservation.ProvolesCinemasId;
                    var selectedMoviesId = _context.Provoles
                        .Where(p => p.CinemasId == selectedCinemasId && p.MoviesName == reservation.ProvolesMoviesName)
                        .Select(p => p.MoviesId)
                        .FirstOrDefault();

                    //set correct CinemasId and MoviesId
                    reservation.ProvolesCinemasId = selectedCinemasId;
                    reservation.ProvolesMoviesId = selectedMoviesId;

                    //check availability
                    var availableSeatsResult = await GetAvailableSeats(selectedCinemasId, selectedMoviesId, reservation.ProvolesMoviesName);

                    if (availableSeatsResult is ActionResult<int> actionResult)
                    {
                        if (actionResult.Result is OkObjectResult objectResult)
                        {
                            if (objectResult.Value is int availableSeats)
                            {
                                // Check if there are enough available seats for the reservation
                                if (reservation.NumberOfSeats <= availableSeats)
                                {
                                    // Continue with reservation creation
                                    reservation.Id = GetNextReservationId();
                                    _context.Add(reservation);
                                    await _context.SaveChangesAsync();
                                    return RedirectToAction(nameof(Details), new { id = reservation.Id });
                                }
                                else
                                {
                                    // Not enough available seats, add a validation error
                                    ModelState.AddModelError("NumberOfSeats", $"There are only {availableSeats} available seats.");
                                }
                            }
                            else if (objectResult.Value is string notFoundMessage)
                            {
                                // Provoles not found
                                ModelState.AddModelError("ProvolesCinemasId", notFoundMessage);
                            }
                        }
                        else if (actionResult.Result is BadRequestObjectResult badRequestResult)
                        {
                            // Handle the case where fetching available seats fails
                            ModelState.AddModelError("ProvolesCinemasId", badRequestResult.Value.ToString());
                        }
                        else
                        {
                            // Handle unexpected result type
                            ModelState.AddModelError("ProvolesCinemasId", "Unexpected result type from GetAvailableSeats.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle other exceptions that may occur
                    ModelState.AddModelError("ProvolesCinemasId", $"Error: {ex.Message}");
                }
            }

            // If the model is not valid or an error occurred, reload the ViewData and return to the Create view
            ViewData["CustomersId"] = new SelectList(_context.Customers, "Id", "Name", reservation.CustomersId);
            ViewData["ProvolesMoviesName"] = new SelectList(_context.Provoles.Select(p => p.MoviesName).Distinct().ToList(), reservation.ProvolesMoviesName);
            ViewData["ProvolesCinemasId"] = new SelectList(_context.Provoles.Include(p => p.Cinemas), "CinemasId", "Cinemas.Name", reservation.ProvolesCinemasId);

            // Additional ViewData for available seats
            ViewData["AvailableSeats"] = 0;

            return View(reservation);
        }



        private int GetNextReservationId()
        {
            int nextId = _context.Reservations.Max(p => (int?)p.Id) ?? 0;
            return nextId + 1;
        }

        private async Task<ActionResult<int>> GetAvailableSeats(int? cinemasId, int? moviesId, string moviesName)
        {
            try
            {
                var response = await _context.Provoles
                    .Where(p => p.CinemasId == cinemasId && p.MoviesId == moviesId && p.MoviesName == moviesName)
                    .Select(p => new { TotalSeats = int.Parse(p.Cinemas.Seats), ReservedSeats = p.Reservations.Sum(r => r.NumberOfSeats) })
                    .FirstOrDefaultAsync();

                if (response != null)
                {
                    int availableSeats = (int)(response.TotalSeats - response.ReservedSeats);
                    return Ok(availableSeats);  // Use Ok() to return a 200 OK response
                }
                else
                {
                    // Provoles not found
                    return NotFound("Provoles not found.");  // Use NotFound() to return a 404 Not Found response
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the fetching of available seats
                return BadRequest($"Error fetching available seats: {ex.Message}");  // Use BadRequest() to return a 400 Bad Request response
            }
        }

        [HttpPost]
        public JsonResult GetCinemaNameById(int cinemasId)
        {
            var cinemaName = _context.Provoles
                .Where(p => p.CinemasId == cinemasId)
                .Select(p => p.Cinemas.Name)
                .FirstOrDefault();

            return Json(cinemaName);
        }

        // GET: Reservations/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = _context.Reservations.Find(id);

            if (reservation == null)
            {
                return NotFound();
            }

            // Fetch cinemas' names based on the ProvolesCinemasId of the reservation
            var cinemasNames = _context.Provoles
                .Where(p => p.CinemasId == reservation.ProvolesCinemasId)
                .Select(p => p.Cinemas.Name)
                .Distinct()
                .ToList();

            ViewData["CustomersId"] = new SelectList(_context.Customers, "Id", "Name", reservation.CustomersId);
            ViewData["ProvolesMoviesName"] = new SelectList(_context.Provoles.Select(p => p.MoviesName).Distinct().ToList(), reservation.ProvolesMoviesName);
            ViewData["ProvolesCinemasId"] = new SelectList(_context.Provoles.Include(p => p.Cinemas), "CinemasId", "Cinemas.Name", reservation.ProvolesCinemasId);

            return View(reservation);
        }

        // POST: Reservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NumberOfSeats,ProvolesCinemasId,ProvolesMoviesName,CustomersId,Id")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the selected cinema and movie details
                    var selectedCinemasId = reservation.ProvolesCinemasId;
                    var selectedMoviesId = _context.Provoles
                        .Where(p => p.CinemasId == selectedCinemasId && p.MoviesName == reservation.ProvolesMoviesName)
                        .Select(p => p.MoviesId)
                        .FirstOrDefault();

                    // Set correct CinemasId and MoviesId
                    reservation.ProvolesCinemasId = selectedCinemasId;
                    reservation.ProvolesMoviesId = selectedMoviesId;
                    reservation.CustomersId = reservation.CustomersId;

                    // Check availability
                    var availableSeatsResult = await GetAvailableSeats(selectedCinemasId, selectedMoviesId, reservation.ProvolesMoviesName);

                    if (availableSeatsResult is ActionResult<int> actionResult)
                    {
                        if (actionResult.Result is OkObjectResult objectResult)
                        {
                            if (objectResult.Value is int availableSeats)
                            {
                                // Check if the user is reducing the number of seats or adding more seats
                                int currentlyBookedSeats = (int)_context.Reservations.Where(r => r.Id == reservation.Id).Select(r => r.NumberOfSeats).FirstOrDefault();
                                int seatsDifference = (int)(reservation.NumberOfSeats - currentlyBookedSeats);

                                // Check if there are enough available seats for the reservation
                                if (seatsDifference <= availableSeats)
                                {
                                    // Continue with reservation update
                                    _context.Update(reservation);
                                    await _context.SaveChangesAsync();
                                    return RedirectToAction(nameof(Details), new { id = reservation.Id });
                                }
                                else
                                {
                                    // Not enough available seats, add a validation error
                                    ModelState.AddModelError("NumberOfSeats", $"There are only {availableSeats} available seats.");
                                }
                            }
                            else if (objectResult.Value is string notFoundMessage)
                            {
                                // Provoles not found
                                ModelState.AddModelError("ProvolesCinemasId", notFoundMessage);
                            }
                        }
                        else if (actionResult.Result is BadRequestObjectResult badRequestResult)
                        {
                            // Handle the case where fetching available seats fails
                            ModelState.AddModelError("ProvolesCinemasId", badRequestResult.Value.ToString());
                        }
                        else
                        {
                            // Handle unexpected result type
                            ModelState.AddModelError("ProvolesCinemasId", "Unexpected result type from GetAvailableSeats.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle other exceptions that may occur
                    ModelState.AddModelError("ProvolesCinemasId", $"Error: {ex.Message}");
                }
            }

            // If the model is not valid or an error occurred, reload the ViewData and return to the Edit view
            ViewData["CustomersId"] = new SelectList(_context.Customers, "Id", "Name", reservation.CustomersId);
            ViewData["ProvolesMoviesName"] = new SelectList(_context.Provoles.Select(p => p.MoviesName).Distinct().ToList(), reservation.ProvolesMoviesName);
            ViewData["ProvolesCinemasId"] = new SelectList(_context.Provoles.Include(p => p.Cinemas), "CinemasId", "Cinemas.Name", reservation.ProvolesCinemasId);

            // Additional ViewData for available seats
            ViewData["AvailableSeats"] = 0;

            return View(reservation);
        }



        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Customers)
                .Include(r => r.Provoles)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservations == null)
            {
                return Problem("Entity set 'TicketServiceContext.Reservations'  is null.");
            }
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
          return (_context.Reservations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
    

}
