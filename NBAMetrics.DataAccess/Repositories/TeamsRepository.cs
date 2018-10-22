using NBAMetrics.DataAccess.Interfaces;
using System.Data.Entity;

namespace NBAMetrics.DataAccess.Repositories
{
    public class TeamsRepository : RepositoryBase<Teams>, ITeamsRepository
    {
        public TeamsRepository(DbContext context) : base(context)
        {
        }
    }
}
