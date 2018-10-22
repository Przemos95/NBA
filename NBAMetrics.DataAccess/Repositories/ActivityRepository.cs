using NBAMetrics.DataAccess.Interfaces;
using System.Data.Entity;

namespace NBAMetrics.DataAccess.Repositories
{
    public class ActivityRepository : RepositoryBase<Activity>, IActivityRepository
    {
        public ActivityRepository(DbContext context) : base(context)
        {
        }
    }
}