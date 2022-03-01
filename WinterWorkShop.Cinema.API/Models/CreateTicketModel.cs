using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;

namespace WinterWorkShop.Cinema.API.Models
{
    public class CreateTicketModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public Guid ProjectionId { get; set; }

        [Required]
        public List<Seat> SeatIds { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}
