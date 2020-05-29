using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication
{
    public partial class Authors
    {
        public Authors()
        {
            Authorship = new HashSet<Authorship>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Ім'я автора")]
        [RegularExpression(@" ^([A-Z][a-z]+)\ ([A-Z][a-z]+)(\ ?([A-Z][a-z]+)?)|([А-ЯІЇЄЩ][а-яіїщє]+)\ ([А-ЯІЇЄЩ][а-яіїщє]+)(\ ?([А-ЯІЇЄЩ][а-яіїщє]+)?)$", ErrorMessage = "Неправильний формат імені")]
        public string FullName { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата народження")]
        public DateTime? DateOfBirth { get; set; }

        public virtual ICollection<Authorship> Authorship { get; set; }
    }
}
