using CinemaTickets.Domain.DomainModels;
using CinemaTickets.Domain.DTO;
using CinemaTickets.Repository.Interface;
using CinemaTickets.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CinemaTickets.Service.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<TicketsInOrder> _ticketsInOrderRepositorty;
        private readonly IRepository<Order> _orderRepositorty;
        private readonly IRepository<EmailMessage> _mailRepository;
        private readonly Repository.ApplicationDbContext _context;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository,
            IUserRepository userRepository,
            IRepository<TicketsInOrder> ticketsInOrderRepositorty,
            IRepository<Order> orderRepositorty,
            IRepository<EmailMessage> mailRepository,
            Repository.ApplicationDbContext context)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _userRepository = userRepository;
            _ticketsInOrderRepositorty = ticketsInOrderRepositorty;
            _orderRepositorty = orderRepositorty;
            _mailRepository = mailRepository;
            _context = context;
        }
        public bool deleteProductFromShoppingCart(string userId, Guid id)
        {
            if (!string.IsNullOrEmpty(userId) && id != null)
            {              

                var loggedInUser = _userRepository.Get(userId);

                var userShoppingCart = loggedInUser.ShoppingCart;

                var itemToDelete = userShoppingCart.TicketsInShoppingCarts
                    .Where(z => z.TicketId.Equals(id)).FirstOrDefault();

                userShoppingCart.TicketsInShoppingCarts.Remove(itemToDelete);

                _shoppingCartRepository.Update(userShoppingCart);

                return true;
            }

            return false;
        }

        public ShoppingCartDto getShoppingCartInfo(string userId)
        {
            var loggedInUser = this._userRepository.Get(userId);

            var userShoppingCart = loggedInUser.ShoppingCart;

            var allTickets = userShoppingCart.TicketsInShoppingCarts.ToList();

            var allTicketPrice = allTickets.Select(z => new
            {
                TicketPrice = z.Ticket.Price,
                TicketQuantity = z.Quantity
            }).ToList();

            var total = 0;
            foreach (var item in allTicketPrice)
            {
                total += item.TicketQuantity * item.TicketPrice;
            }
            ShoppingCartDto scDto = new ShoppingCartDto
            {
                Tickets = allTickets,
                TotalPrice = total
            };    

            return scDto;
        }

        public bool orderNow(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {              
                var loggedInUser = this._userRepository.Get(userId);

                var userShoppingCart = loggedInUser.ShoppingCart;

                EmailMessage mail = new EmailMessage();
                mail.MailTo = loggedInUser.Email;
                mail.Subject = "Successfully created order";
                mail.Status = false;

                Order order = new Order
                {
                    Id = Guid.NewGuid(),
                    User = loggedInUser,
                    UserId = userId
                };

                this._orderRepositorty.Insert(order);

                List<TicketsInOrder> ticketsInOrders = new List<TicketsInOrder>();

                var result = userShoppingCart.TicketsInShoppingCarts
                    .Select(z => new TicketsInOrder
                {
                    Id = Guid.NewGuid(),
                    TicketId = z.Ticket.Id,
                    SelectedTicket = z.Ticket,
                    OrderId = order.Id,
                    UserOrder = order,
                    Quantity = z.Quantity
                }).ToList();

                StringBuilder sb = new StringBuilder();

                var totalPrice = 0;

                sb.AppendLine("Your order is completed. The order conains: ");

                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];

                    totalPrice += item.Quantity * item.SelectedTicket.Price;

                    var movieName = "";
                    foreach(var m in _context.Movies)
                    {
                        if(item.SelectedTicket.MovieId.Equals(m.Id))
                        {
                            movieName = m.MovieName;
                        }
                    }

                    sb.AppendLine(i.ToString() + ". " + movieName + " with price of: " + item.SelectedTicket.Price + " and quantity of: " + item.Quantity);
                }

                sb.AppendLine("Total price: " + totalPrice.ToString());


                mail.Content = sb.ToString();


                ticketsInOrders.AddRange(result);

                foreach (var item in ticketsInOrders)
                {
                    this._ticketsInOrderRepositorty.Insert(item);
                }

                loggedInUser.ShoppingCart.TicketsInShoppingCarts.Clear();

                this._userRepository.Update(loggedInUser);
                //this._mailRepository.Insert(mail);

                return true;
            }
            return false;
        }
    }
}
