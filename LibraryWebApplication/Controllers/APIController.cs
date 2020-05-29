using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebApplication.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly LibraryContext _context;

        public APIController(LibraryContext context)
        {
            _context = context;
        }
        [HttpGet("Categories")]
        public JsonResult Categories()
        {
            var categories = _context.Categories.Include(o=>o.BookCategory).ToList();
            List<object> result = new List<object>();
            result.Add(new[] { "Категорія", "Кількість книжок" });
            foreach (var o in categories)
            {
                result.Add(new object[] { o.CategoryName, o.BookCategory.Count() });
            }

            return new JsonResult(result);
        }


        [HttpGet("BooksDate")]
        public JsonResult BooksDate()
        {
            var books = _context.Books.OrderBy(o => o.YearOfPublication).ToList();
            List<object> result = new List<object>();
            result.Add(new[] { "Рік", "Кількість книжок" });

           Dictionary<int,int> years = new Dictionary<int, int>();
            foreach (var b in books)
            {
                try
                {
                    years[b.YearOfPublication]++;
                }
                catch
                {
                    years.Add(b.YearOfPublication, 1);
                }
            }
            foreach (var b in years)
            {
                result.Add(new object[] { b.Key.ToString(), b.Value });
            }
            return new JsonResult(result);
        }
    }
}