// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	InfrastructureLayer.cs
// 
// 	Created by: ykorshev 
// 	 at 15.05.2013 15:36
// 
// ============================================================

using Autofac;
using Kartel.Domain.Infrastructure.Search;
using Kartel.Domain.Interfaces.Search;
using Kartel.Domain.IoC;

namespace Kartel.Domain.Infrastructure
{
    /// <summary>
    /// Инфраструктурный модуль
    /// </summary>
    public class InfrastructureLayer: Autofac.Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        /// <param name="builder">The builder through which components can be
        ///             registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SearchManager>().As<ISearchManager>().SingleInstance();
        }
    }
}