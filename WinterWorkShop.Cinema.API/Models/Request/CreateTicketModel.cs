using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.API.Models.Request
{
    public class CreateTicketModel
    {
        public Guid SeatId { get; set; }
        public Guid ProjectionId { get; set; }
        public double Price { get; set; }
    }
}
