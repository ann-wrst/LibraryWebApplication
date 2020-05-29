using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication
{
    public partial class Readers
    {
        public Readers()
        {
            BookReading = new HashSet<BookReading>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Ім'я читача")]
        [RegularExpression(@" ^([A-Z][a-z]+)\ ([A-Z][a-z]+)(\ ?([A-Z][a-z]+)?)|([А-ЯІЇЄЩ][а-яіїщє]+)\ ([А-ЯІЇЄЩ][а-яіїщє]+)(\ ?([А-ЯІЇЄЩ][а-яіїщє]+)?)$", ErrorMessage = "Неправильний формат імені")]
        public string FullName { get; set; }

        public virtual ICollection<BookReading> BookReading { get; set; }
    }
}
