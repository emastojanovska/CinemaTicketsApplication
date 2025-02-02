﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTickets.Domain.DomainModels
{
    public class TicketsInShoppingCart : BaseEntity
    {
        public Guid TicketId { get; set; }
        public Guid ShoppingCartId { get; set; }
        public Ticket Ticket { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public int Quantity { get; set; }

    }
}
