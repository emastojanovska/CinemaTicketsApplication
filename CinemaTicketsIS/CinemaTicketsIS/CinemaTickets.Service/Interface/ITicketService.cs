using CinemaTickets.Domain.DTO;
using CinemaTickets.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Text;
using CinemaTickets.Domain.DomainModels.Enumerations;

namespace CinemaTickets.Service.Interface
{
    public interface ITicketService
    {
        List<Ticket> GetAllTickets();
        List<Ticket> GetAllTicketsByGenre(Genre genre);
        Ticket GetDetailsForTicket(Guid? id);
        void CreateNewTicket(Ticket t);
        void UpdateExistingTicket(Ticket t);
        AddToShoppingCartDto GetShoppingCartInfo(Guid? id);
        void DeleteTicket(Guid id);
        bool AddToShoppingCart(AddToShoppingCartDto item, string userID);
    }
}
