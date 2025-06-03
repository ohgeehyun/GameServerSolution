using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiServer.DB.Model
{
    [Table("users_oauth")]
    public class UsersOauth
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [MaxLength(255)]
        public string KakaoId { get; set; }

        [MaxLength(255)]
        public string Nickname { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        public DateTime? Birth { get; set; }

        [MaxLength(1)]
        public string Gender { get; set; }
    }
}
