using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryWebApplication;
using Microsoft.CodeAnalysis.Operations;

namespace LibraryWebApplication.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category_data = await _context.BookCategory.Where(o => o.BookId == id).Select(o => o.Category).ToListAsync();
            if (category_data == null)
            {
                return NotFound();
            }
            var book_data = await _context.Books.Where(o => o.Id == id).FirstOrDefaultAsync();
            if (book_data == null)
            {
                return NotFound();
            }
            ViewBag.BookId = book_data.Id;

            ViewBag.BookName = book_data.Name;


            return View(category_data);
        }

        public IActionResult CategoryAdd(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.CategoryData = new SelectList(_context.Categories, "Id", "CategoryName");
            ViewBag.BookData = new SelectList(_context.Books, "Id", "Name", id);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryAdd(int? id, [Bind("BookId,CategoryId")]BookCategory AddObj)
        {
            var search = await _context.BookCategory.Where(o => o.BookId == AddObj.BookId && o.CategoryId == AddObj.CategoryId).FirstOrDefaultAsync();
            if (search != null)
            {
                ModelState.AddModelError("CategoryId", "У книги вже є ця категорія");
            }
            if (ModelState.IsValid)
            {
                _context.Add(AddObj);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Books", new { id = AddObj.BookId });
            }
            ViewBag.CategoryData = new SelectList(_context.Categories, "Id", "CategoryName", AddObj.CategoryId);
            ViewBag.BookData = new SelectList(_context.Books, "Id", "Name", AddObj.BookId);
            return View(AddObj);
        }

        public async Task<IActionResult> CategoryRemove([Bind("bookid")]int? bookid, [Bind("catergoryid")]int? categoryid)
        {
            if (bookid == null || categoryid == null)
            {
                return NotFound();
            }
            var bookCat = await _context.BookCategory.Where(bc => bc.BookId == bookid && bc.CategoryId == categoryid).Include(o => o.Book).Include(o => o.Category).FirstOrDefaultAsync();
            if (bookCat == null)
            {
                return NotFound();
            }
            ViewBag.BookId = bookCat.BookId;
            ViewBag.CategoryId = bookCat.CategoryId;
            return View(bookCat);
        }

        [HttpPost, ActionName("CategoryRemove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryRemoveConfirmed([Bind("BookId")]int? bookid, [Bind("CategoryId")]int? categoryid)
        {
            var bookCat = await _context.BookCategory.Where(bc => bc.BookId == bookid && bc.CategoryId == categoryid).FirstOrDefaultAsync();
            if (bookCat == null)
            {
                return NotFound();
            }
            _context.BookCategory.Remove(bookCat);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Books", new { id = bookCat.BookId });
        }



        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,YearOfPublication,NumberOfPages")] Books books)
        {
            if (ModelState.IsValid)
            {
                _context.Add(books);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(books);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var books = await _context.Books.FindAsync(id);
            if (books == null)
            {
                return NotFound();
            }
            return View(books);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,YearOfPublication,NumberOfPages")] Books books)
        {
            if (id != books.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(books);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BooksExists(books.Id))
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
            return View(books);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var books = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (books == null)
            {
                return NotFound();
            }

            return View(books);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var books = await _context.Books.FindAsync(id);
            _context.Books.Remove(books);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BooksExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
