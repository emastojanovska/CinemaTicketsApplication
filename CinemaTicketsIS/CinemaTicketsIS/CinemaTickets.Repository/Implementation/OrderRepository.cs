using CinemaTickets.Domain.DomainModels;
using CinemaTickets.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTickets.Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {

        private readonly ApplicationDbContext context;
        private DbSet<Order> entities;
        string errorMessage = string.Empty;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Order>();
        }


        public List<Order> getAllOrders()
        {
            return entities
                .Include(z => z.User)
                .Include(z => z.TicketsInOrders)
                .Include("TicketsInOrders.SelectedTicket")
                .ToListAsync().Result;
        }

        public List<Order> getAllOrdersForUser(string userId)
        {
            var user = context.Users.Find(userId);
            var orders = new List<Order>();
            foreach(var o in entities.Include(z=>z.TicketsInOrders)
                .ThenInclude(t => t.SelectedTicket))
            {
                if (o.UserId.Equals(user.Id))
                {
                    orders.Add(o);
                }
            }
            return orders;
        }

        public Order getOrderDetails(Guid id)
        {
            return entities
               .Include(z => z.User)
               .Include(z => z.TicketsInOrders)
               .Include("TicketsInOrders.SelectedTicket")
               .SingleOrDefaultAsync(z => z.Id == id).Result;
        }
    }
}
