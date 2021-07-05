
using CinemaTickets.Domain.DomainModels;
using CinemaTickets.Domain.DomainModels.Enumerations;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTickets.Domain.Identity
{
    public class CinemaApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }
        public virtual ShoppingCart ShoppingCart { get; set; }

    }
}
