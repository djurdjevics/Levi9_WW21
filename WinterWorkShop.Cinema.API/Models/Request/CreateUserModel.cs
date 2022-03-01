using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.API.Models.Request
{
    public class CreateUserModel
    {
        [Required]
        public string firstName { get; set; }

        [Required]
        //[StringLength(255, ErrorMessage = Messages.AUDITORIUM_PROPERTIE_NAME_NOT_VALID)]
        public string lastName { get; set; }

        [Required]
        //[Range(1, 20, ErrorMessage = Messages.AUDITORIUM_PROPERTIE_SEATROWSNUMBER_NOT_VALID)]
        public string userName { get; set; }

        [Required]
        //[Range(1, 20, ErrorMessage = Messages.AUDITORIUM_PROPERTIE_SEATNUMBER_NOT_VALID)]
        public string role { get; set; }
    }
}
