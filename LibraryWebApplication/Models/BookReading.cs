using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryWebApplication
{
    public partial class BookReading
    {
        [Display(Name = "Назва книги")]
        public int BookId { get; set; }
        [Display(Name = "Ім'я читача")]
        public int ReaderId { get; set; }
        public int Id { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата видачі")]
        public DateTime? DateOfIssue { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата повернення")]
        public DateTime? ReturnDate { get; set; }
        public virtual Books Book { get; set; }
        public virtual Readers Reader { get; set; }
    }
}
