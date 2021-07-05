using CinemaTickets.Domain.DomainModels.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTickets.Domain.DomainModels
{
    public class Movie : BaseEntity
    {
        [Display(Name = "Movie Name")]
        public string MovieName { get; set; }
        public string Description { get; set; }
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }
        public int Rating { get; set; }
        public Genre Genre { get; set; }
        [Display(Name ="Duration")]
        public int DurationMinutes { get; set; }
        public String Cast { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }

    }
}
