using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication
{
    public partial class Authorship
    {
        [Display(Name = "Назва книги")]
        public int BookId { get; set; }
        [Display(Name = "Ім'я автора")]
        public int AuthorId { get; set; }
        public int Id { get; set; }

        public virtual Authors Author { get; set; }
        public virtual Books Book { get; set; }
    }
}
