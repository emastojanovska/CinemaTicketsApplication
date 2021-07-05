using CinemaTickets.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTickets.Domain.DTO
{
    public class AddToShoppingCartDto 
    {
        public Ticket SelectedTicket { get; set; }
        public Guid TicketId { get; set; }        
        public int Quantity { get; set; }
/*        public int Available { get; set; }
*/
                       
/*
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            if (Quantity > Available)
            {
                errors.Add(new ValidationResult("There aren't that many tickets available"));
            }
            return errors;
        }*/


    }
}
