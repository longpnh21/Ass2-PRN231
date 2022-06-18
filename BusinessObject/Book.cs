using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }
        [Required]
        [MaxLength(100)]
        public string Type { get; set; }
        [Required]
        public int PubId { get; set; }
        [Required]
        public double Price { get; set; }
        public double Advance { get; set; }
        [DefaultValue(0)]
        public double Royalty { get; set; } = 0;
        [DefaultValue(0)]
        public double YtdSales { get; set; } = 0;
        [MaxLength(200)]
        public string Notes { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime? PublishedDate { get; set; }

        [ForeignKey("PubId")]
        public virtual Publisher Publisher { get; set; }
        public virtual ICollection<BookAuthor> BookAuthors { get; set; }
    }
}
