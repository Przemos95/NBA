using NBAMetrics.DataAccess.Interfaces;
using System.Data.Entity;

namespace NBAMetrics.DataAccess.Repositories
{
    public class PositionRepository : RepositoryBase<Positions>, IPositionsRepository
    {
        public PositionRepository(DbContext context) : base(context)
        {
        }
    }
}