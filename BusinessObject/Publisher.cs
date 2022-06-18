using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject
{
    public class Publisher
    {
        [Key]
        public int PubId { get; set; }
        [Required]
        [MaxLength(50)]
        public string PublisherName { get; set; }
        [MaxLength(100)]
        public string City { get; set; }
        [MaxLength(100)]
        public string State { get; set; }
        [MaxLength(100)]
        public string Country { get; set; }

        public virtual ICollection<Book> Books { get; set; }
    }
}
