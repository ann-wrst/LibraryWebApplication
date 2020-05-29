using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication
{
    public partial class Books
    {
        public Books()
        {
            Authorship = new HashSet<Authorship>();
            BookCategory = new HashSet<BookCategory>();
            BookReading = new HashSet<BookReading>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Назва книги")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Рік публікації")]
        [Range(1300, 2020, ErrorMessage = "Введіть рік від 1300 до поточного")]
        public int YearOfPublication { get; set; }
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Кількість сторінок")]
        [Range(1, 25000, ErrorMessage = "Введіть коректну кількість сторінок")]
        public int NumberOfPages { get; set; }

        public virtual ICollection<Authorship> Authorship { get; set; }
        public virtual ICollection<BookCategory> BookCategory { get; set; }
        public virtual ICollection<BookReading> BookReading { get; set; }
    }
}
