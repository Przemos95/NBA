using DEVEKO.DataAccess.App_Start;
using NBAMetrics.DataAccess.Interfaces;
using NBAMetrics.DataAccess.Repositories;
using Ninject;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Mvc;

namespace NBAMetrics.Web.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            kernel.Bind(typeof(IRepository<>)).To(typeof(RepositoryBase<>));
            kernel.Bind<DbContext>().To<EntityContext>();
            kernel.Bind<IActivityRepository>().To<ActivityRepository>();
            kernel.Bind<IEstimatedRankingRepository>().To<EstimatedRankingRepository>();
            kernel.Bind<IPositionsRepository>().To<PositionRepository>();
            kernel.Bind<IRankingsRepository>().To<RankingsRepository>();
            kernel.Bind<ITeamsRepository>().To<TeamsRepository>();
        }
    }
}