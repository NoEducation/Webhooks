using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelAgentWeb.Models;

namespace TravelAgentWeb.Data
{
    public class TravelAgentDbContext : DbContext
    {
        public TravelAgentDbContext(DbContextOptions<TravelAgentDbContext> options)
            : base(options)
        {}


        public DbSet<WebhookSecret> SubscriptionSecrets { get; set; }

    }
}
