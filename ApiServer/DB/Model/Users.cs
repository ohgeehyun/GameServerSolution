using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiServer.DB.Model
{
    [Table("users")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }

        [Required]
        [MaxLength(255)]
        public string UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        [MaxLength(255)]
        public string Nickname { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; }

        public DateTime? Birth { get; set; }

        [MaxLength(1)]
        public string? Gender { get; set; }
    }
}
