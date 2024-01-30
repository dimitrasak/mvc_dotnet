using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc_dotnet.Models;

namespace mvc_dotnet.Controllers
{
    public class UsersController : Controller
    {
        private readonly TicketServiceContext _context;

        public UsersController(TicketServiceContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
              return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'TicketServiceContext.Users'  is null.");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Username == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                // Generate a random salt
                byte[] saltBytes = new byte[16];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(saltBytes);
                }

                // Convert the salt to base64 for storage
                user.Salt = Convert.ToBase64String(saltBytes);

                // Combine password and salt, then hash
                using (var sha256 = new SHA256Managed())
                {
                    byte[] combinedBytes = Encoding.UTF8.GetBytes(user.Password).Concat(saltBytes).ToArray();
                    byte[] hashedPasswordBytes = sha256.ComputeHash(combinedBytes);

                    // Convert the hashed password to base64 for storage
                    user.Password = Convert.ToBase64String(hashedPasswordBytes);
                }

                // Set other properties
                user.CreateTime = DateTime.Now;

                // Add the user to the context
                _context.Add(user);

                // Save changes to the database
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        //Login!
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            // Retrieve the user from the database based on the provided username
            var user = _context.Users.SingleOrDefault(u => u.Username == username);

            if (user != null)
            {
                // Convert the stored salt and entered password to byte arrays
                byte[] saltBytes = Convert.FromBase64String(user.Salt);
                byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(password);

                // Combine entered password and stored salt, then hash
                using (var sha256 = new SHA256Managed())
                {
                    byte[] combinedBytes = enteredPasswordBytes.Concat(saltBytes).ToArray();
                    byte[] hashedPasswordBytes = sha256.ComputeHash(combinedBytes);

                    // Compare the hashed password with the stored hashed password
                    if (Convert.ToBase64String(hashedPasswordBytes) == user.Password)
                    {
                        // Passwords match, login successful
                        // Pass the username to the view
                        ViewData["Username"] = user.Username;
                        string? normalizedRole = user.Role?.ToLower()?.Trim();

                        switch (normalizedRole)
                        {
                            case "admin":
                                return RedirectToAction("Home", "Admins");
                            case "customer":
                                return RedirectToAction("Index", "Customers");
                            case "content":
                                return RedirectToAction("Home", "ContentAdmins");
                            default:
                                ModelState.AddModelError(string.Empty, "Invalid role");
                                break;
                        }
                    }
                }
            }

            // Incorrect username or password, return to login view
            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return View();
        }



        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Username,Email,Password,CreateTime,Salt,Role")] User user)
        {
            if (id != user.Username)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Username))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Username == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'TicketServiceContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
          return (_context.Users?.Any(e => e.Username == id)).GetValueOrDefault();
        }
    }
}
