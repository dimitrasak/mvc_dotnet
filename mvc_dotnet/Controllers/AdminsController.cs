using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using mvc_dotnet.Models;
using X.PagedList;


namespace mvc_dotnet.Controllers
{
    public class AdminsController : Controller
    {
        private readonly TicketServiceContext _context;

        public AdminsController(TicketServiceContext context)
        {
            _context = context;
        }

        public IActionResult Home()
        {
            // Add any logic or data retrieval specific to the admin home page
            return View();
        }

        public IActionResult AssignRole()
        {  
            // Retrieve a list of users from the database
            var users = _context.Users.ToList();

            // You may pass the list of users to the view
            return View(users);
        }

        
        [HttpPost]
        public IActionResult AssignRole(string username, string newRole)
        {
            try
            {
                // Retrieve the user from the database based on the provided username
                var user = _context.Users.SingleOrDefault(u => u.Username == username);

                if (user != null)
                {
                    // Assign the new role
                    user.Role = newRole;

                    // Update the database
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Role assigned successfully";

                    // Optionally, you may return a response indicating success or failure
                    return Ok(new { Message = "Role assigned successfully" });
                }
                TempData["ErrorMessage"] = "User not found";
                // User not found
                return NotFound(new { Message = "User not found" });
            }
            catch (DbUpdateException ex)
            {
                // Check if the exception is due to a duplicate key violation
                if (ex.InnerException is SqlException sqlException && sqlException.Number == 2627)
                {
                    // Handle the duplicate key violation here
                    // For example, show a message to the user
                    TempData["ErrorMessage"] = "Error: Duplicate record already exists in the table.";
                    return BadRequest(new { Message = "Error: Duplicate record already exists in the table." });
                }
                else
                {
                    TempData["ErrorMessage"] = "Error: Internal Server Error while processing the request.";
                    // Handle other DbUpdateException scenarios or rethrow the exception
                    return StatusCode(500, new { Message = "Internal Server Error" });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";

                // Handle other exceptions as needed
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }           


        // GET: Admins
        public async Task<IActionResult> Index(int? page, string? search)
        {
            ViewData["CurrentFilter"] = search;

            var admins = from a in _context.Admins.Include(a => a.UserUsernameNavigation)
                         select a;

            if (!String.IsNullOrEmpty(search))
            {
                admins = admins.Where(a => a.UserUsernameNavigation.Username.Contains(search));
                // Add additional search conditions as needed
            }

            admins = admins.OrderBy(a => a.UserUsernameNavigation.Username);

            // Pagination
            if (page != null && page < 1)
            {
                page = 1;
            }

            int PageSize = 10;
            var adminsData = await admins.ToPagedListAsync(page ?? 1, PageSize);

            return View(adminsData);
            //var ticketServiceContext = _context.Admins.Include(a => a.UserUsernameNavigation);
            //return View(await ticketServiceContext.ToListAsync());
        }


        // GET: Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Admins == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .Include(a => a.UserUsernameNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            var admins = _context.Users.Where(u => u.Role == "Admin");
            ViewData["UserUsername"] = new SelectList(admins, "Username", "Username");
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,UserUsername")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                admin.Id = GetNextId();
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserUsername"] = new SelectList(_context.Users, "Username", "Username", admin.UserUsername);
            return View(admin);
        }

        private int GetNextId()
        {
            // Logic to get the next available ID, for example, querying the database or using a counter
            int nextId = _context.Admins.Max(a => (int?)a.Id) ?? 0;
            return nextId + 1;
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Admins == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            ViewData["UserUsername"] = new SelectList(_context.Users, "Username", "Username", admin.UserUsername);
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UserUsername")] Admin admin)
        {
            if (id != admin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.Id))
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
            ViewData["UserUsername"] = new SelectList(_context.Users, "Username", "Username", admin.UserUsername);
            return View(admin);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Admins == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .Include(a => a.UserUsernameNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Admins == null)
            {
                return Problem("Entity set 'TicketServiceContext.Admins'  is null.");
            }
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
          return (_context.Admins?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
