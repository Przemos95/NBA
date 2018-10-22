using NBAMetrics.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEVEKO.DataAccess.App_Start
{
    public class EntityContext : DbContext
    {
        public EntityContext()
            : base("NBAConnection")
        {

        }

        public DbSet<Activity> Activity { get; set; }
        public DbSet<EstimatedRankings> EstimatedRankings { get; set; }
        public DbSet<Positions> Positions { get; set; }
        public DbSet<Rankings> Rankings { get; set; }
        public DbSet<Teams> Teams { get; set; }
    }
}
