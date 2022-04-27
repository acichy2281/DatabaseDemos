// Author: Andrew Cichy
// Class: CIS237
// Date: 4/27/22
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using cis237_assignment_6.Models;

namespace cis237_assignment_6.Controllers
{
    [Authorize]
    public class BeveragesController : Controller
    {
        private readonly BeverageContext _context;

        public BeveragesController(BeverageContext context)
        {
            _context = context;
        }

        // GET: Beverages
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Beverages.ToListAsync());
            // Setup a variable to hold the beverages data
            DbSet<Beverage> BeveragesToFilter = _context.Beverages;

            // Setup some strings to hold the data that might be in
            // the session. If there is nothing in the session
            // we can still use these variables as a default.
            string filterName = "";
            string filterPack = "";
            string filterMin = "";
            string filterMax = "";
            // Define a min and max for the cylinders
            int min = -10000;
            int max = 16000;

            // Check to see if there is a vaule in the session and if there is, assing it to the variable that we setup to hold the value
            if (!String.IsNullOrWhiteSpace(
                HttpContext.Session.GetString("session_name")
                ))
            {
                filterName = HttpContext.Session.GetString("session_name");
            }
            if (!String.IsNullOrWhiteSpace(
                HttpContext.Session.GetString("session_pack")
                ))
            {
                filterPack = HttpContext.Session.GetString("session_pack");
            }
            if (!String.IsNullOrWhiteSpace(
                HttpContext.Session.GetString("session_min")
                ))
            {
                filterMin = HttpContext.Session.GetString("session_min");
                min = Int32.Parse(filterMin);
            }
            if (!String.IsNullOrWhiteSpace(
                HttpContext.Session.GetString("session_max")
                ))
            {
                filterMax = HttpContext.Session.GetString("session_max");
                max = Int32.Parse(filterMax);
            }

            IList<Beverage> finalFiltered = await BeveragesToFilter.Where(
                beverage => beverage.Price >= min &&
                beverage.Price <= max &&
                beverage.Name.Contains(filterName) &&
                beverage.Pack.Contains(filterPack)
            ).ToListAsync();

            ViewData["filteredName"] = filterName;
            ViewData["filteredpack"] = filterPack;
            ViewData["filteredMin"] = filterMin;
            ViewData["filteredMax"] = filterMax;

            return View(finalFiltered);
        }

        // GET: Beverages/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beverage = await _context.Beverages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (beverage == null)
            {
                return NotFound();
            }

            return View(beverage);
        }

        // GET: Beverages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Beverages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Pack,Price,Active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(beverage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(beverage);
        }

        // GET: Beverages/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beverage = await _context.Beverages.FindAsync(id);
            if (beverage == null)
            {
                return NotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Pack,Price,Active")] Beverage beverage)
        {
            if (id != beverage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(beverage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeverageExists(beverage.Id))
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
            return View(beverage);
        }

        // GET: Beverages/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beverage = await _context.Beverages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (beverage == null)
            {
                return NotFound();
            }

            return View(beverage);
        }

        // POST: Beverages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var beverage = await _context.Beverages.FindAsync(id);
            _context.Beverages.Remove(beverage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Filter method to filter the beverage list based off a given parameter
        public IActionResult Filter()
        {
            // Get the form data that we sent out of the request object
            // The string that is used as a key to get the data matches 
            // the name propert of the form control
            string name = HttpContext.Request.Form["name"];
            string pack = HttpContext.Request.Form["pack"];
            string min = HttpContext.Request.Form["min"];
            string max = HttpContext.Request.Form["max"];

            // Now that we have the data pulled out from the the
            // Request object, let's put it into the session 
            // so that other methods can have access to it
            HttpContext.Session.SetString("session_name", name);
            HttpContext.Session.SetString("session_pack", pack);
            HttpContext.Session.SetString("session_min", min);
            HttpContext.Session.SetString("session_max", max);

            return RedirectToAction(nameof(Index));
        }

        private bool BeverageExists(string id)
        {
            return _context.Beverages.Any(e => e.Id == id);
        }
    }
}
