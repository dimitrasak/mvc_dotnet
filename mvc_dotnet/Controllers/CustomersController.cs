﻿using System;
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
    public class CustomersController : Controller
    {
        private readonly TicketServiceContext _context;

        public CustomersController(TicketServiceContext context)
        {
            _context = context;
        }

        public IActionResult Home()
        {
            // Add any logic or data retrieval specific to the admin home page
            return View();
        }

        public async Task<IActionResult> History(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.Include(x => x.Reservations).ThenInclude(x => x.Provoles)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
        // GET: Customers
        public async Task<IActionResult> Index(int? page, string? search)
        {
            ViewData["CurrentFilter"] = search;
            var customers = from c in _context.Customers.Include(c => c.UserUsernameNavigation)
                            select c;

            if (!String.IsNullOrEmpty(search))
            {
                customers = customers.Where(c => c.UserUsernameNavigation.Username.Contains(search));
            }

            //var customersList = await customers.ToListAsync();
            // Pagination for users
            if (page != null && page < 1)
            {
                page = 1;
            }

            int PageSize = 10;
            var customersData = await customers.ToPagedListAsync(page ?? 1, PageSize);
            return View(customersData);
        }

        

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.UserUsernameNavigation)
                .Include(c => c.Reservations)
                .ThenInclude(c => c.Provoles)
                .ThenInclude(c => c.Cinemas)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            var customerUsers = _context.Users.Where(u => u.Role == "Customer");
            ViewData["UserUsername"] = new SelectList(customerUsers, "Username", "Username");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,UserUsername")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.Id = GetNextId();
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var customerUsers = _context.Users.Where(u => u.Role == "Customer");
            ViewData["UserUsername"] = new SelectList(customerUsers, "Username", "Username", customer.UserUsername);
            return View(customer);
        }

        private int GetNextId()
        {
            // Logic to get the next available ID, for example, querying the database or using a counter
            int nextId = _context.Customers.Max(c => (int?)c.Id) ?? 0;
            return nextId + 1;
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["UserUsername"] = new SelectList(_context.Users, "Username", "Username", customer.UserUsername);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UserUsername")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
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
            ViewData["UserUsername"] = new SelectList(_context.Users, "Username", "Username", customer.UserUsername);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.UserUsernameNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'TicketServiceContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
          return (_context.Customers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
