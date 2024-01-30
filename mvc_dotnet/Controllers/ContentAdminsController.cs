﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc_dotnet.Models;

namespace mvc_dotnet.Controllers
{
    public class ContentAdminsController : Controller
    {
        private readonly TicketServiceContext _context;

        public ContentAdminsController(TicketServiceContext context)
        {
            _context = context;
        }

        public IActionResult Home()
        {
            // Add any logic or data retrieval specific to the admin home page
            return View();
        }

        // GET: ContentAdmins
        public async Task<IActionResult> Index()
        {
            var ticketServiceContext = _context.ContentAdmins.Include(c => c.UserUsernameNavigation);
            return View(await ticketServiceContext.ToListAsync());
        }

        // GET: ContentAdmins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ContentAdmins == null)
            {
                return NotFound();
            }

            var contentAdmin = await _context.ContentAdmins
                .Include(c => c.UserUsernameNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contentAdmin == null)
            {
                return NotFound();
            }

            return View(contentAdmin);
        }

        // GET: ContentAdmins/Create
        public IActionResult Create()
        {
            var contentUsers = _context.Users.Where(u => u.Role == "Content");
            ViewData["UserUsername"] = new SelectList(contentUsers, "Username", "Username");
            return View();
        }

        // POST: ContentAdmins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UserUsername")] ContentAdmin contentAdmin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contentAdmin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var contentUsers = _context.Users.Where(u => u.Role == "Content");
            ViewData["UserUsername"] = new SelectList(contentUsers, "Username", "Username", contentAdmin.UserUsername);
            return View(contentAdmin);
        }

        // GET: ContentAdmins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ContentAdmins == null)
            {
                return NotFound();
            }

            var contentAdmin = await _context.ContentAdmins.FindAsync(id);
            if (contentAdmin == null)
            {
                return NotFound();
            }
            ViewData["UserUsername"] = new SelectList(_context.Users, "Username", "Username", contentAdmin.UserUsername);
            return View(contentAdmin);
        }

        // POST: ContentAdmins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UserUsername")] ContentAdmin contentAdmin)
        {
            if (id != contentAdmin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contentAdmin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContentAdminExists(contentAdmin.Id))
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
            ViewData["UserUsername"] = new SelectList(_context.Users, "Username", "Username", contentAdmin.UserUsername);
            return View(contentAdmin);
        }

        // GET: ContentAdmins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ContentAdmins == null)
            {
                return NotFound();
            }

            var contentAdmin = await _context.ContentAdmins
                .Include(c => c.UserUsernameNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contentAdmin == null)
            {
                return NotFound();
            }

            return View(contentAdmin);
        }

        // POST: ContentAdmins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ContentAdmins == null)
            {
                return Problem("Entity set 'TicketServiceContext.ContentAdmins'  is null.");
            }
            var contentAdmin = await _context.ContentAdmins.FindAsync(id);
            if (contentAdmin != null)
            {
                _context.ContentAdmins.Remove(contentAdmin);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContentAdminExists(int id)
        {
          return (_context.ContentAdmins?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
