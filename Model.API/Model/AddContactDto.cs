using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.API.Model
{
    public class AddContactDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [MaxLength(20)]
        public string LastName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string WebSiteUrl { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;
        public string Emails { get; set; } = string.Empty;
    }
}
