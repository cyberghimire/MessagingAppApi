using System;
using System.ComponentModel.DataAnnotations;

namespace MessagingApp.API.Dtos{
    public class UserForRegisterDto{

        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(32, MinimumLength =8, ErrorMessage ="Password Length must be within 8-32 characters.")]
        public string Password { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string KnownAs { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }
        
        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }
        
        public UserForRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}