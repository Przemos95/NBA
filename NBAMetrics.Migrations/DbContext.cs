using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBAMetrics.Core.Domain.Entities.Identity;
using System.Data.SqlClient;
using System.Configuration;

namespace NBAMetrics.Migrations
{
    class DbContext : System.Data.Entity.DbContext
    {
        public DbSet<Team> Teams { get; set; }

        //public List<Team> GetTeams()
        //{

        //}

        //private IEnumerable<Team> getTeams()
        //{
        //    SqlConnection connection;
        //    using (connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connectionStringName"].ConnectionString;)
        //    {

        //    }
        //}
        
    }
}
