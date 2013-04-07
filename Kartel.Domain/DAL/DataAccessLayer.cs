// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	DataAccessLayer.cs
// 
// 	Created by: ykorshev 
// 	 at 07.04.2013 17:49
// 
// ============================================================

using System.Reflection;
using Autofac;
using Autofac.Integration.Mvc;
using Kartel.Domain.DAL.Repositories;
using Kartel.Domain.Interfaces.Repositories;

namespace Kartel.Domain.DAL
{
    /// <summary>
    /// Модуль регистрации зависимостей DAL слоя
    /// </summary>
    public class DataAccessLayer: Autofac.Module
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
            builder.RegisterType<KartelDataContext>().AsSelf().InstancePerHttpRequest();
            builder.RegisterType<UsersRepository>().As<IUsersRepository>();
            builder.RegisterType<CategoriesRepository>().As<ICategoriesRepository>();
            builder.RegisterType<ProductsRepository>().As<IProductsRepository>();
        }
    }
}