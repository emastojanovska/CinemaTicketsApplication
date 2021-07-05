using CinemaTickets.Domain.DomainModels;
using CinemaTickets.Domain.DomainModels.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaTickets.Repository.Interface
{
    public interface ITicketRepository
    {
        IEnumerable<Ticket> GetAll();
        IEnumerable<Ticket> GetAllByGenre(Genre genre);
        Ticket Get(Guid? id);
        void Insert(Ticket entity);
        void Update(Ticket entity);
        void Delete(Ticket entity);
    }
}
