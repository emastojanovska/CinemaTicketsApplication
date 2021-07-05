using CinemaTickets.Domain.DomainModels.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTickets.Domain.DomainModels
{
    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public Role Role { get; set; }
    }
}
