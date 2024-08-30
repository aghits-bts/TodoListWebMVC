using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ToDoListMVC.Context;
using ToDoListMVC.Models;

namespace ToDoListMVC.Controllers
{
    public class ToDoController : Controller
    {
        private readonly ToDoDbContext _context;

        public ToDoController(ToDoDbContext context)
        {
            _context = context;
        }

        // GET: ToDo
        public async Task<IActionResult> Index(string importance, bool? completed, string searchString, string category, DateTime? dueDate)
        {
            if (_context.ToDoModel is null)
            {
                return Problem("Entity set 'ToDoDbContext.ToDoModel'  is null.");
            }

            var items = from t in _context.ToDoModel
                        select t;

            if (!string.IsNullOrEmpty(importance))
            {
                items = items.Where(s => s.Importance == importance);
            }

            if (completed.HasValue)
            {
                items = items.Where(s =>s.Completed == completed.Value);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                items = items.Where(s => s.Description.ToLower().Contains(searchString.ToLower()));
            }

            if (!string.IsNullOrEmpty(category))
            {
                items = items.Where(s => s.Category == category);
            }

            if (dueDate.HasValue)
            {
                items = items.Where(s => s.DueDate.Date == dueDate);
            }



            //sort by priority
            items = items.OrderBy(s => s.Completed)
                         .ThenByDescending(s => s.Importance == "High")
                         .ThenByDescending(s => s.Importance == "Medium")
                         .ThenByDescending(s => s.Importance == "Low")
                         .ThenBy(s => s.DueDate);

           

            return View(await items.ToListAsync());
         
        }

        // GET: ToDo/Details/id
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ToDoModel == null)
            {
                return NotFound();
            }

            var toDoModel = await _context.ToDoModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoModel == null)
            {
                return NotFound();
            }

            return View(toDoModel);
        }

        // GET: ToDo/Create
        public IActionResult Create()
        {
            ViewData["PriorityList"] = new SelectList(new[] {"High", "Medium", "Low"});
            ViewData["CategoryList"] = new SelectList(new[] { "Home", "School", "Work", "Misc" });
            return View();
        }

        // POST: ToDo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Importance,Id,Description,Completed,Category,DueDate")] ToDoModel toDoModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(toDoModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(toDoModel);
        }

        // GET: ToDo/Edit/id
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ToDoModel == null)
            {
                return NotFound();
            }

            var toDoModel = await _context.ToDoModel.FindAsync(id);
            if (toDoModel == null)
            {
                return NotFound();
            }
            //populate priority list for drop-down
            ViewData["PriorityList"] = new SelectList(new[] { "High", "Medium", "Low" }, toDoModel.Importance);

            //populate category list for drop-down
            ViewData["CategoryList"] = new SelectList(new[] { "Home", "School", "Work", "Misc" }, toDoModel.Category);

            return View(toDoModel);
        }

        // POST: ToDo/Edit/id
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Importance,Id,Description,Completed,Category,DueDate")] ToDoModel toDoModel)
        {
            if (id != toDoModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toDoModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoModelExists(toDoModel.Id))
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
            //populate priority list for drop-down
            ViewData["PriorityList"] = new SelectList(new[] { "High", "Medium", "Low" }, toDoModel.Importance);
            //populate category list for drop-down
            ViewData["CategoryList"] = new SelectList(new[] { "Home", "School", "Work", "Misc" }, toDoModel.Category);

            return View(toDoModel);
        }

        // GET: ToDo/Delete/id
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ToDoModel == null)
            {
                return NotFound();
            }

            var toDoModel = await _context.ToDoModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoModel == null)
            {
                return NotFound();
            }

            return View(toDoModel);
        }

        // POST: ToDo/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ToDoModel == null)
            {
                return Problem("Entity set 'ToDoDbContext.ToDoModel'  is null.");
            }
            var toDoModel = await _context.ToDoModel.FindAsync(id);
            if (toDoModel != null)
            {
                _context.ToDoModel.Remove(toDoModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoModelExists(int id)
        {
          return (_context.ToDoModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
