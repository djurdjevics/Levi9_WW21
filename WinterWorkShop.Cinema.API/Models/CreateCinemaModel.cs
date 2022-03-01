using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Common;

namespace WinterWorkShop.Cinema.API.Models
{
    public class CreateCinemaModel
    {
        [Required]
        [StringLength(50, ErrorMessage = Messages.CINEMA_PROPERTY_NAME_NOT_VALID)]
        public string Name { get; set; }
    }
}
