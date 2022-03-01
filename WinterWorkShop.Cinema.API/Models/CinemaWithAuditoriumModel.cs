using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Common;

namespace WinterWorkShop.Cinema.API.Models
{
    public class CinemaWithAuditoriumModel
    {
        [Required]
        [StringLength(50, ErrorMessage = Messages.CINEMA_PROPERTY_NAME_NOT_VALID)]
        public string Name { get; set; }

        [StringLength(50, ErrorMessage = Messages.AUDITORIUM_PROPERTIE_NAME_NOT_VALID)]
        public string auditName { get; set; }

        [Range(0, 20, ErrorMessage = Messages.AUDITORIUM_PROPERTIE_SEATROWSNUMBER_NOT_VALID)]
        public int seatRows { get; set; }

        [Range(0, 20, ErrorMessage = Messages.AUDITORIUM_PROPERTIE_SEATNUMBER_NOT_VALID)]
        public int numberOfSeats { get; set; }

    }
}
