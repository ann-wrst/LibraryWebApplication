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
    public class ReadersController : Controller
    {
        private readonly LibraryContext _context;

        public ReadersController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Readers
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null || id < 0)
                return View(await _context.Readers.ToListAsync());
            var readers = _context.BookReading.Where(o => o.BookId == id).Select(o => o.Reader);
            if (readers == null)
            {
                return NotFound();
            }
            return View(await readers.ToListAsync());
        }

        // GET: Readers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookreaders = await _context.BookReading.Where(m => m.ReaderId == id).Include(o => o.Book).ToListAsync();

            if (bookreaders == null)
            {
                return NotFound();
            }
            var reader_data = await _context.Readers.Where(o => o.Id == id).FirstOrDefaultAsync();
            if (reader_data == null)
            {
                return NotFound();
            }
            ViewBag.readerName = reader_data.FullName;
            ViewBag.ReaderId = reader_data.Id;

            return View(bookreaders);
        }
        public IActionResult BookAdd(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.ReaderData = new SelectList(_context.Readers, "Id", "FullName", id);
            ViewBag.BookData = new SelectList(_context.Books, "Id", "Name");
            var obj = new BookReading();
            obj.DateOfIssue = DateTime.Today;
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAdd(int? id, [Bind("BookId,ReaderId,DateOfIssue,ReturnDate")]BookReading AddObj)
        {
            var search = await _context.BookReading.Where(o => o.BookId == AddObj.BookId && o.ReaderId == AddObj.ReaderId).FirstOrDefaultAsync();
            if (search != null)
            {
                ModelState.AddModelError("ReaderId", "В читача вже є ця книга");
            }      
            if(AddObj.DateOfIssue!=null && AddObj.ReturnDate!=null && AddObj.DateOfIssue> AddObj.ReturnDate)
            {
                ModelState.AddModelError("ReturnDate", "Дата повернення не може бути раніше дати видачі");
            }
            if (AddObj.DateOfIssue != null && AddObj.ReturnDate != null && AddObj.DateOfIssue>DateTime.Today)
            {
                ModelState.AddModelError("DateOfIssue", "Дата видачі не може бути в майбутньому");
            }
            if (ModelState.IsValid)
            {
                _context.Add(AddObj);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = AddObj.ReaderId });
            }
            ViewBag.ReaderData = new SelectList(_context.Readers, "Id", "FullName", AddObj.ReaderId);
            ViewBag.BookData = new SelectList(_context.Books, "Id", "Name", AddObj.BookId);
            AddObj.DateOfIssue= DateTime.Today;
            return View(AddObj);
        }
        /*public IActionResult BookEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var search = _context.BookReading.Where(o => o.Id == id).FirstOrDefault();
            if (search == null)
            {
                ViewBag.ReaderData = new SelectList(_context.Readers, "Id", "FullName");
                ViewBag.BookData = new SelectList(_context.Books, "Id", "Name");
            }
             ViewBag.ReaderData = new SelectList(_context.Readers, "Id", "FullName", search.ReaderId);
            ViewBag.BookData = new SelectList(_context.Books, "Id", "Name", search.BookId);
            return View(search);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookEdit(int? id, [Bind("BookId,ReaderId,DateOfIssue,ReturnDate")]BookReading AddObj)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.BookReading.Update(AddObj);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReadersExists(AddObj.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = AddObj.ReaderId });
            }
            ViewBag.ReaderData = new SelectList(_context.Readers, "Id", "FullName", AddObj.ReaderId);
            ViewBag.BookData = new SelectList(_context.Books, "Id", "Name", AddObj.BookId);
            return View(AddObj);
        } */
        public async Task<IActionResult> HandBook(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var search = await _context.BookReading.Where(o => o.Id == id).FirstOrDefaultAsync();
            if (search == null) { 
                return NotFound(); 
            }
            search.ReturnDate = DateTime.Today;
           
                try
                {
                    _context.BookReading.Update(search);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReadersExists(search.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = search.ReaderId });
        }
        public async Task<IActionResult> BookRemove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var bookread = await _context.BookReading.Where(bc => bc.Id==id).Include(o => o.Book).Include(o => o.Reader).FirstOrDefaultAsync();
            if (bookread == null)
            {
                return NotFound();
            }

            ViewBag.BookId = bookread.BookId;
            ViewBag.ReaderId = bookread.ReaderId;
            return View(bookread);
        }

        [HttpPost, ActionName("BookRemove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookRemoveConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var bookread = await _context.BookReading.Where(bc => bc.Id == id).FirstOrDefaultAsync();
            if (bookread == null)
            {
                return NotFound();
            }
            _context.BookReading.Remove(bookread);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Readers", new { id = bookread.ReaderId });
        }


        // GET: Readers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Readers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName")] Readers readers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(readers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(readers);
        }

        // GET: Readers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readers = await _context.Readers.FindAsync(id);
            if (readers == null)
            {
                return NotFound();
            }
            return View(readers);
        }

        // POST: Readers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName")] Readers readers)
        {
            if (id != readers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(readers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReadersExists(readers.Id))
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
            return View(readers);
        }

        // GET: Readers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readers = await _context.Readers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (readers == null)
            {
                return NotFound();
            }

            return View(readers);
        }

        // POST: Readers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var readers = await _context.Readers.FindAsync(id);
            _context.Readers.Remove(readers);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReadersExists(int id)
        {
            return _context.Readers.Any(e => e.Id == id);
        }
    }
}
