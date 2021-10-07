using System.ComponentModel.DataAnnotations;

namespace MessagingApp.API.Dtos{
    public class UserForRegisterDto{

        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(32, MinimumLength =8, ErrorMessage ="Password Length must be within 8-32 characters.")]
        public string Password { get; set; }
    }
}