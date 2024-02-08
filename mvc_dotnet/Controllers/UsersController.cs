using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc_dotnet.Models;
using X.PagedList;
using static NuGet.Packaging.PackagingConstants;

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
        public async Task<IActionResult> Index(int? page, string? search)
        {
            ViewData["CurrentFilter"] = search;
            var users = from u in _context.Users
                        select u;
            if (!String.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.Username.Contains(search));
            }

            //users = users.OrderBy(c => c.Username);

            //users = await _context.Users.ToListAsync();
            // Pagination for users
            if (page != null && page < 1)
            {
                page = 1;
            }

            int PageSize = 10;
            var usersData = await users.ToPagedListAsync(page ?? 1, PageSize);
            return View(usersData);
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
                user.Salt = Convert.ToBase64String(GenerateSalt());

                // Hash the password using the generated salt
                user.Password = HashPassword(user.Password, user.Salt);

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
                if(ValidatePassword(password, user.Password, user.Salt))
                {
                    // Passwords match, login successful
                    // Redirect based on user role (assuming you have a Role property in your User model)
                    switch (user.Role?.ToLower()?.Trim())
                    {
                        case "admin":
                            return RedirectToAction("Home", "Admins");
                        case "customer":
                            return RedirectToAction("Home", "Customers");
                        case "content":
                            return RedirectToAction("Home", "ContentAdmins");
                        default:
                            return RedirectToAction("Login", "Users");
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
        public async Task<IActionResult> Edit(string id, [Bind("Username,Email,Password,CreateTime")] User userModel)
        {
            if (id != userModel.Username)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // If you want to generate a new salt for each password change, uncomment the following line
            byte[] newSaltBytes = GenerateSalt();

            // Convert the new salt to base64 for storage
            userModel.Salt = Convert.ToBase64String(newSaltBytes);

            // Combine new password and salt, then hash
            userModel.Password = HashPassword(userModel.Password, userModel.Salt);

            // Update other properties
            user.Email = userModel.Email;
            user.CreateTime = userModel.CreateTime;
            user.Password = userModel.Password;

            user.Salt = userModel.Salt;

            _context.Update(user);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(userModel.Username))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private string HashPassword(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);

            using (var sha256 = new SHA256Managed())
            {
                byte[] combinedBytes = Encoding.UTF8.GetBytes(password).Concat(saltBytes).ToArray();
                byte[] hashedPasswordBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashedPasswordBytes);
            }
        }

        private bool ValidatePassword(string enteredPassword, string storedHashedPassword, string salt)
        {
            string hashedEnteredPassword = HashPassword(enteredPassword, salt);
            return string.Equals(hashedEnteredPassword, storedHashedPassword, StringComparison.Ordinal);
        }

        
        private byte[] GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return saltBytes;
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
