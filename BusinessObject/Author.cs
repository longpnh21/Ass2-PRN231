using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        [MaxLength(50)]
        [Required]
        public string LastName { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(15)]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [MaxLength(200)]
        public string Address { get; set; }
        [MaxLength(100)]
        public string City { get; set; }
        [MaxLength(100)]
        public string State { get; set; }
        [MaxLength(5)]
        [DataType(DataType.PostalCode)]
        public int Zip { get; set; }
        [Required]
        [MaxLength(320)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        public virtual ICollection<BookAuthor> BookAuthors { get; set; }
    }
}
