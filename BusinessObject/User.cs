using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BusinessObject
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [MaxLength(320)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required]
        [MaxLength(100)]
        [DataType(DataType.Password)]
        [JsonIgnore]
        public string Password { get; set; }
        [MaxLength(1000)]
        public string Source { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string MiddleName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        public int RoleId { get; set; }
        public int? PubId { get; set; }
        public DateTime? HireDate { get; set; }

        [ForeignKey("PubId")]
        public virtual Publisher Publisher { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}
