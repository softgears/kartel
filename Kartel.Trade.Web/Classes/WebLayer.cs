// ============================================================
// 
// 	Kartel
// 	Kartel.Trade.Web 
// 	WebLayer.cs
// 
// 	Created by: ykorshev 
// 	 at 07.04.2013 17:57
// 
// ============================================================

using Autofac;
using Autofac.Integration.Mvc;
using Kartel.Trade.Web.Classes.Cache;

namespace Kartel.Trade.Web.Classes
{
    /// <summary>
    /// Контейнер зависимостей слоя веб интерфейса
    /// </summary>
    public class WebLayer: Autofac.Module
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
            builder.RegisterType<CategoriesIndexManager>().AsSelf().SingleInstance();
            builder.RegisterControllers(this.ThisAssembly);
        }
    }
}