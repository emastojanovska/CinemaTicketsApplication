using CinemaTickets.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTickets.Service.Interface
{
    public interface IOrderService
    {
        List<Order> getAllOrders();
        List<Order> getAllOrdersForUser(string userId);

        Order GetOrderDetails(Guid id);
    }
}
