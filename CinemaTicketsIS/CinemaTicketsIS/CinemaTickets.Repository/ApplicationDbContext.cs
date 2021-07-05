using CinemaTickets.Domain.DomainModels;
using CinemaTickets.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace CinemaTickets.Repository
{
    public class ApplicationDbContext : IdentityDbContext<CinemaApplicationUser>
    {
     
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
       

        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<TicketsInShoppingCart> TicketsInShoppingCart { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<EmailMessage> EmailMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //Validation
            builder.Entity<Ticket>()
                .Property(z => z.DateTime).IsRequired();

            builder.Entity<Movie>()
                .Property(z => z.MovieName).IsRequired();
            builder.Entity<Movie>()
               .Property(z => z.Genre).IsRequired();
            builder.Entity<Movie>()
               .Property(z => z.DurationMinutes).IsRequired();

            //Automatic generation of id
            builder.Entity<Ticket>()
               .Property(z => z.Id)
               .ValueGeneratedOnAdd();
            builder.Entity<Movie>()
               .Property(z => z.Id)
               .ValueGeneratedOnAdd();
            builder.Entity<ShoppingCart>()
               .Property(z => z.Id)
               .ValueGeneratedOnAdd();
            builder.Entity<TicketsInShoppingCart>()
              .Property(z => z.Id)
              .ValueGeneratedOnAdd();

            //Composite key
            builder.Entity<TicketsInShoppingCart>()
                .HasKey(z => new { z.ShoppingCartId, z.Id });

            builder.Entity<TicketsInOrder>()
                .HasKey(z => new { z.OrderId, z.Id });

            //N-N relationships
            builder.Entity<TicketsInShoppingCart>()
                .HasOne(z => z.Ticket)
                .WithMany(z => z.TicketsInShoppingCarts)
                .HasForeignKey(z => z.ShoppingCartId);

            builder.Entity<TicketsInShoppingCart>()
                    .HasOne(z => z.ShoppingCart)
                    .WithMany(z => z.TicketsInShoppingCarts)
                    .HasForeignKey(z => z.TicketId);

            builder.Entity<TicketsInOrder>()
                .HasOne(z => z.SelectedTicket)
                .WithMany(z => z.TicketsInOrders)
                .HasForeignKey(z => z.OrderId);

            builder.Entity<TicketsInOrder>()
                .HasOne(z => z.UserOrder)
                .WithMany(z => z.TicketsInOrders)
                .HasForeignKey(z => z.TicketId);


            //1-1 relationships
            builder.Entity<ShoppingCart>()
                    .HasOne<CinemaApplicationUser>(z => z.Owner)
                    .WithOne(z => z.ShoppingCart)
                    .HasForeignKey<ShoppingCart>(z => z.OwnerId);

            //1-N relationships
            builder.Entity<Ticket>()
            .HasOne<Movie>(s => s.Movie)
            .WithMany(g => g.Tickets)
            .HasForeignKey(s => s.MovieId);

        }
    }
}
