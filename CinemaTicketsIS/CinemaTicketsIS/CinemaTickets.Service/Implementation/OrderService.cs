using CinemaTickets.Domain.DomainModels;
using CinemaTickets.Repository.Interface;
using CinemaTickets.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTickets.Service.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository)
        {
            this._orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        public List<Order> getAllOrders()
        {
            return this._orderRepository.getAllOrders();
        }

        public List<Order> getAllOrdersForUser(string userId)
        {
            return _orderRepository.getAllOrdersForUser(userId);
        }

        public Order GetOrderDetails(Guid model)
        {
            return this._orderRepository.getOrderDetails(model);
        }
      

        List<Order> IOrderService.getAllOrders()
        {
            return _orderRepository.getAllOrders();
        }
    }
}
