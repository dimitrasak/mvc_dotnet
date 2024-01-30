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
    public class ReservationsController : Controller
    {
        private readonly TicketServiceContext _context;

        public ReservationsController(TicketServiceContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index(int? page)
        {
            var ticketServiceContext = _context.Reservations.Include(r => r.Customers).Include(r => r.Provoles);
            var reservations = await ticketServiceContext.ToListAsync();
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
            ViewData["ProvolesCinemasId"] = new SelectList(_context.Provoles, "CinemasId", "CinemasId");
            ViewData["ProvolesMoviesName"] = new SelectList(_context.Provoles, "CinemasId", "MoviesName");

            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumberOfSeats,ProvolesMoviesId,ProvolesMoviesName,CustomersId,Id")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                reservation.Id = GetNextReservationId();
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = reservation.Id });
            }
            ViewData["CustomersId"] = new SelectList(_context.Customers, "Id", "Name", reservation.CustomersId);
            ViewData["ProvolesCinemasId"] = new SelectList(_context.Provoles, "CinemasId", "CinemasId", reservation.ProvolesCinemasId);
            ViewData["ProvolesMoviesName"] = new SelectList(_context.Provoles, "CinemasId", "MoviesName", reservation.ProvolesMoviesName);


            return View(reservation);
        }

        private int GetNextReservationId()
        {
            int nextId = _context.Reservations.Max(p => (int?)p.Id) ?? 0;
            return nextId + 1;
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservations == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["CustomersId"] = new SelectList(_context.Customers, "Id", "Id", reservation.CustomersId);
            ViewData["ProvolesCinemasId"] = new SelectList(_context.Provoles, "CinemasId", "MoviesName", reservation.ProvolesCinemasId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NumberOfSeats,ProvolesCinemasId,ProvolesMoviesId,ProvolesMoviesName,CustomersId,Id")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
            ViewData["CustomersId"] = new SelectList(_context.Customers, "Id", "Id", reservation.CustomersId);
            ViewData["ProvolesCinemasId"] = new SelectList(_context.Provoles, "CinemasId", "MoviesName", reservation.ProvolesCinemasId);
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
