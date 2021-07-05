using CinemaTickets.Domain.DomainModels;
using CinemaTickets.Domain.DomainModels.Enumerations;
using CinemaTickets.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CinemaTickets.Repository.Implementation
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext context;
        private readonly DbSet<Ticket> entities;

        public TicketRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Ticket>();
        }
        public IEnumerable<Ticket> GetAll()
        {
            return entities
               // .Include(z => z.Movie)
                //.Include("Movie.MovieName")
                .AsEnumerable();
        }

        public Ticket Get(Guid? id)
        {
            return entities
               // .Include(t => t.Movie)
               // .Include("Movie.MovieName")
                .SingleOrDefault(s => s.Id == id);
        }
        public void Insert(Ticket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }

        public void Update(Ticket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            context.SaveChanges();
        }

        public void Delete(Ticket entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            context.SaveChanges();
        }

        public IEnumerable<Ticket> GetAllByGenre(Genre genre)
        {
            List<Ticket> ticketList = new List<Ticket>();
            foreach(var e in entities)
            {
                foreach(var m in context.Movies)
                {
                    if(m.Id.Equals(e.MovieId))
                    {
                        if(m.Genre.Equals(genre))
                        {
                            ticketList.Add(e);
                        }
                    }
                }
            }
            return ticketList;
        }
    }
}
