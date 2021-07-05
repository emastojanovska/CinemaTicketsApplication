using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTickets.Domain.DomainModels
{
    public class Ticket : BaseEntity, IValidatableObject
    {
      
        [Required]
        [Display(Name = "Date and Time")]
    
        public DateTime DateTime { get; set; }
        [Required]
        public int Available { get; set; }  
        public Movie Movie { get; set; }
        public Guid MovieId { get; set; }
        [Required]
        public int Price { get; set; }
        public virtual ICollection<TicketsInOrder> TicketsInOrders { get; set; }
        public virtual ICollection<TicketsInShoppingCart> TicketsInShoppingCarts { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            if (DateTime <= DateTime.Now)
            {
                errors.Add(new ValidationResult($"{nameof(DateTime)} needs to be greater than now."));
            }
            return errors;
        }

    }
}
