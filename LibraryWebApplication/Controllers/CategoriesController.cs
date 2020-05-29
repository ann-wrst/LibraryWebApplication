using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryWebApplication;
using Microsoft.AspNetCore.Http;
using System.IO;
using ClosedXML.Excel;
using System.Text.RegularExpressions;

namespace LibraryWebApplication.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly LibraryContext _context;

        public CategoriesController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return View(await _context.Categories.ToListAsync());
            }
            var categories_data = _context.BookCategory.Where(obj => obj.BookId == id).Select(i => i.Category);
            if (categories_data == null)
            {
                return NotFound();
            }
            return View(categories_data);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                        if (Path.GetExtension(stream.Name) == ".xlsx" || Path.GetExtension(stream.Name) == ".xls")
                        {
                            await fileExcel.CopyToAsync(stream);
                            using (XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled))
                            {                              
                                foreach (IXLWorksheet worksheet in workBook.Worksheets)
                                {
                                    //worksheet.Name - назва категорії. Пробуємо знайти в БД, якщо відсутня, то створюємо нову
                                    Categories newcat;
                                    var c = (from cat in _context.Categories
                                             where cat.CategoryName.Contains(worksheet.Name)
                                             select cat).ToList();
                                    if (c.Count > 0)
                                    {
                                        newcat = c[0];
                                    }
                                    else
                                    {
                                        newcat = new Categories();
                                        newcat.CategoryName = worksheet.Name;
                                        _context.Categories.Add(newcat);
                                    }
                                    //перегляд усіх рядків                    
                                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                    {
                                        try
                                        {
                                            Books book = new Books();
                                            book.Name = row.Cell("1").Value.ToString();
                                            if (Int16.Parse(row.Cell("2").Value.ToString()) < 1 || Int16.Parse(row.Cell("2").Value.ToString()) > 25000)
                                                throw new Exception();
                                            book.NumberOfPages = Int16.Parse(row.Cell("2").Value.ToString());

                                            if (Int16.Parse(row.Cell("3").Value.ToString()) < 1300 || Int16.Parse(row.Cell("3").Value.ToString()) > 2020)
                                                throw new Exception();

                                            book.YearOfPublication = Int16.Parse(row.Cell("3").Value.ToString());
                                            BookCategory bookCategory = new BookCategory();
                                            bookCategory.Category = newcat;
                                            bookCategory.Book = book;
                                            _context.Books.Add(book);
                                            _context.BookCategory.Add(bookCategory);
                                            for (int i = 4; i <= 6; i++)
                                            {                                  
                                               /* if(!Regex.IsMatch(row.Cell(i).Value.ToString(), @"^([A-Z][a-z]+)\ ([A-Z][a-z]+)(\ ?([A-Z][a-z]+)?)|([А-ЯІЇЄЩ][а-яіїщє]+)\ ([А-ЯІЇЄЩ][а-яіїщє]+)(\ ?([А-ЯІЇЄЩ][а-яіїщє]+)?)$")) {
                                                    throw new Exception();
                                                }                     */                         
                                                if (row.Cell(i).Value.ToString().Length > 0)
                                                {
                                                    Authors author;
                                                    var a = (from aut in _context.Authors
                                                             where aut.FullName.Contains(row.Cell(i).Value.ToString())
                                                             select aut).ToList();
                                                    if (a.Count > 0)
                                                    {
                                                        author = a[0];
                                                    }
                                                    else
                                                    {
                                                        author = new Authors();
                                                        author.FullName = row.Cell(i).Value.ToString();
                                                        _context.Add(author);
                                                    }
                                                    Authorship ab = new Authorship();
                                                    ab.Book = book;
                                                    ab.Author = author;
                                                    _context.Authorship.Add(ab);
                                                }                                              
                                            }
                                        }

                                        catch (Exception e)
                                        {
                                            TempData["msgDATA"] = "<script>alert('Некоректний зміст файлу');</script>";
                                        }
                                    }

                                }                               
                            }                            
                        }
                        else TempData["msgDATA"] = "<script>alert('Некоректний формат файлу');</script>";                   
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        public ActionResult Example()
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                    var worksheet = workbook.Worksheets.Add("Example");
                    worksheet.Cell("A1").Value = "Назва книги";
                    worksheet.Cell("B1").Value = "Кількість сторінок";
                    worksheet.Cell("C1").Value = "Рік публікації";
                    worksheet.Cell("D1").Value = "Автор 1";
                    worksheet.Cell("E1").Value = "Автор 2";
                    worksheet.Cell("F1").Value = "Автор 3";                 
                    worksheet.Row(1).Style.Font.Bold = true;                  
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"example_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Books = await _context.BookCategory.Where(m => m.CategoryId == id).Select(o => o.Book).ToListAsync();
            if (Books == null)
            {
                return NotFound();
            }

            return View(Books);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryName")] Categories categories)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categories);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categories);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookCategory = await _context.BookCategory.Where(o => o.CategoryId == id).Select(o => o.Book).ToListAsync();
            if (bookCategory == null)
            {
                return NotFound();
            }
            @ViewBag.categoryName = await _context.Categories.Where(o => o.Id == id).FirstOrDefaultAsync();
            return View(bookCategory);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryName")] Categories categories)
        {
            if (id != categories.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categories);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriesExists(categories.Id))
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
            return View(categories);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categories == null)
            {
                return NotFound();
            }

            return View(categories);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categories = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(categories);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriesExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
