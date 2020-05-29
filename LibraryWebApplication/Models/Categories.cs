using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace LibraryWebApplication
{
    public partial class Categories
    {
        public Categories()
        {
            BookCategory = new HashSet<BookCategory>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Категорія")]
        public string CategoryName { get; set; }

        public virtual ICollection<BookCategory> BookCategory { get; set; }
    }
}
