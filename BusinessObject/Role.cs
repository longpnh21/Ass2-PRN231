using System.ComponentModel.DataAnnotations;

namespace BusinessObject
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        [Required]
        [MaxLength(200)]
        public string RoleDesc { get; set; }
    }
}
