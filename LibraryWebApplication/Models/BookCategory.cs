using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication
{
    public partial class BookCategory
    {
        [Display(Name = "Назва книги")]
        public int BookId { get; set; }
        [Display(Name = "Назва категорії")]
        public int CategoryId { get; set; }
        public int Id { get; set; }

        public virtual Books Book { get; set; }
        public virtual Categories Category { get; set; }
    }
}
