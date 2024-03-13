using AuctionService.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data
{
    public class AuctionDbContext : DbContext
    {
        public AuctionDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Auction> Auctions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //below will add 3 tables into our database, these will be responsible for our outbox functionality
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
