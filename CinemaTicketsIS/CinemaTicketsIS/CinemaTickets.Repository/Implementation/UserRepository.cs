﻿using CinemaTickets.Domain.Identity;
using CinemaTickets.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CinemaTickets.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private readonly DbSet<CinemaApplicationUser> entities;       

        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<CinemaApplicationUser>();
        }
        public IEnumerable<CinemaApplicationUser> GetAll()
        {
            return entities.AsEnumerable();
        }

        public CinemaApplicationUser Get(string id)
        {
            return entities.Include(z => z.ShoppingCart)
                .Include(z => z.ShoppingCart.TicketsInShoppingCarts)
                .ThenInclude(z => z.Ticket)
                .SingleOrDefault(s => s.Id == id);
        }
        public void Insert(CinemaApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }

        public void Update(CinemaApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            context.SaveChanges();
        }

        public void Delete(CinemaApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            context.SaveChanges();
        }

        public CinemaApplicationUser Get(Guid? id)
        {
            throw new NotImplementedException();
        }
    }
}
