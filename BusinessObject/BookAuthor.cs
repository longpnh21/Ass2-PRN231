using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject
{
    public class BookAuthor
    {
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public int BookId { get; set; }
        [Range(0, 1000)]
        public int AuthorOrder { get; set; }
        [DefaultValue(0)]
        public float RoyalityPercentage { get; set; } = 0;

        [ForeignKey("AuthorId")]
        public virtual Author Author { get; set; }
        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
    }
}
