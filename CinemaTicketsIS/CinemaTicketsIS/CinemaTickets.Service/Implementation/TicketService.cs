using CinemaTickets.Domain.DTO;
using CinemaTickets.Domain.DomainModels;
using CinemaTickets.Repository.Interface;
using CinemaTickets.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CinemaTickets.Domain.DomainModels.Enumerations;

namespace CinemaTickets.Service.Implementation
{
   public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<TicketsInShoppingCart> _ticketsInShoppingCartRepository;

        public TicketService(ITicketRepository ticketRepository,
            IUserRepository userRepository,
            IRepository<TicketsInShoppingCart> ticketsInShoppingCartRepository)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _ticketsInShoppingCartRepository = ticketsInShoppingCartRepository;
        }

        public bool AddToShoppingCart(AddToShoppingCartDto item, string userID)
        {
            var user = _userRepository.Get(userID);
            var userShoppingCart = user.ShoppingCart;
            if (item.TicketId != null && userShoppingCart != null)
            {
                var ticket = this.GetDetailsForTicket(item.TicketId);
                if (ticket != null && ticket.Available >= (ticket.Available -= item.Quantity))
                {
                    TicketsInShoppingCart itemToAdd = new TicketsInShoppingCart
                    {                       
                        Ticket = ticket,
                        TicketId = ticket.Id,
                        ShoppingCart = userShoppingCart,
                        ShoppingCartId = userShoppingCart.Id,
                        Quantity = item.Quantity
                    };
                    _ticketsInShoppingCartRepository.Insert(itemToAdd);
                    return true;
                }
                return false;
            }
            return false;
        }

        public void CreateNewTicket(Ticket t)
        {
             _ticketRepository.Insert(t);
        }

        public void DeleteTicket(Guid id)
        {
            var ticket = this.GetDetailsForTicket(id);
            _ticketRepository.Delete(ticket);
        }

        public List<Ticket> GetAllTickets()
        {
            return _ticketRepository.GetAll().ToList();
        }

        public List<Ticket> GetAllTicketsByGenre(Genre genre)
        {
            return _ticketRepository.GetAllByGenre(genre).ToList();
        }

        public Ticket GetDetailsForTicket(Guid? id)
        {
            var ticket = _ticketRepository.Get(id);
            
            return _ticketRepository.Get(id);
        }

        public AddToShoppingCartDto GetShoppingCartInfo(Guid? id)
        {
            var ticket = this.GetDetailsForTicket(id);
            AddToShoppingCartDto model = new AddToShoppingCartDto
            {
                SelectedTicket = ticket,
                TicketId = ticket.Id,
                Quantity = 1,
                //Available = ticket.Available
            };
            return model;
        }

        public void UpdateExistingTicket(Ticket t)
        {
            _ticketRepository.Update(t);
        }
    }
}
