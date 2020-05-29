using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryWebApplication;

namespace LibraryWebApplication.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly LibraryContext _context;

        public AuthorsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Authors
        public async Task<IActionResult> Index(int? id)
        {

            if (id == null) return View(await _context.Authors.ToListAsync());

            var Authors_data = _context.Authorship.Where(o => o.BookId == id).Select(o => o.Author);
            if (Authors_data == null)
            {
                return NotFound();
            }
            return View(await Authors_data.ToListAsync());

        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var books = await _context.Authorship.Where(o => o.AuthorId == id).Select(o => o.Book).ToListAsync();
            if (books == null)
            {
                return NotFound();
            }
            var author = await _context.Authors.Where(o => o.Id == id).FirstOrDefaultAsync();
            if (author == null)
            {
                return NotFound();
            }
            ViewBag.authorid = author.Id;
            ViewBag.authorName = author.FullName;

            return View(books);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult AddBook(int? id)
        {

            ViewBag.AuthorData = new SelectList(_context.Authors, "Id", "FullName", id);
            ViewBag.BookData = new SelectList(_context.Books, "Id", "Name");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBook([Bind("AuthorId,BookId")] Authorship AddObj)
        {
            var bookData = _context.Books.Where(o => o.Id == AddObj.BookId).FirstOrDefault();
            var authorData = _context.Authors.Where(o => o.Id == AddObj.AuthorId).FirstOrDefault();
            var search = await _context.Authorship.Where(o => o.BookId == AddObj.BookId && o.AuthorId == AddObj.AuthorId).FirstOrDefaultAsync();
            if (search != null)
            {
                ModelState.AddModelError("BookId", "В автора вже є ця книга");
            }
            if (authorData.DateOfBirth != null && authorData.DateOfBirth.Value.Year > bookData.YearOfPublication) 
            {
                ModelState.AddModelError("BookId", "Книжка не може бути написана раніше за дату народження автора");
            }
            if (ModelState.IsValid)
            {
                _context.Add(AddObj);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = AddObj.AuthorId });
            }
            ViewBag.AuthorData = new SelectList(_context.Authors, "Id", "FullName", AddObj.AuthorId);
            ViewBag.BookData = new SelectList(_context.Books, "Id", "Name", AddObj.BookId);
            return View(AddObj);
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,DateOfBirth")] Authors authors)
        {
            if (authors.DateOfBirth!=null && DateTime.Today < authors.DateOfBirth)
            {
                ModelState.AddModelError("DateOfBirth", "Дата народження не може бути в майбутньому");
            }
            if(authors.DateOfBirth != null && authors.DateOfBirth.Value.Year<1300)
            {
                ModelState.AddModelError("DateOfBirth", "Дата народження надто мала");
            }
            if (ModelState.IsValid)
            {
                _context.Add(authors);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(authors);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authors = await _context.Authors.FindAsync(id);
            if (authors == null)
            {
                return NotFound();
            }
            return View(authors);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,DateOfBirth")] Authors authors)
        {
            if (id != authors.Id)
            {
                return NotFound();
            }
            if (authors.DateOfBirth != null && DateTime.Today <= authors.DateOfBirth)
            {
                ModelState.AddModelError("DateOfBirth", "Дата народження не може бути в майбутньому");
            }
            if (authors.DateOfBirth != null && authors.DateOfBirth.Value.Year < 1300)
            {
                ModelState.AddModelError("DateOfBirth", "Дата народження надто мала");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(authors);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorsExists(authors.Id))
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
            return View(authors);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authors = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (authors == null)
            {
                return NotFound();
            }

            return View(authors);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var authors = await _context.Authors.FindAsync(id);
            _context.Authors.Remove(authors);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorsExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
