using NBAMetrics.DataAccess.Interfaces;
using System.Data.Entity;

namespace NBAMetrics.DataAccess.Repositories
{
    public class RankingsRepository : RepositoryBase<Rankings>, IRankingsRepository
    {
        public RankingsRepository(DbContext context) : base(context)
        {
        }
    }
}
