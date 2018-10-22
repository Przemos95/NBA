using NBAMetrics.DataAccess.Interfaces;
using System.Data.Entity;

namespace NBAMetrics.DataAccess.Repositories
{
    public class EstimatedRankingRepository : RepositoryBase<EstimatedRankings>, IEstimatedRankingRepository
    {
        public EstimatedRankingRepository(DbContext context) : base(context)
        {
        }
    }
}
