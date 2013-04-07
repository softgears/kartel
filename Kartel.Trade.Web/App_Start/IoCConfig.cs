// ============================================================
// 
// 	Kartel
// 	Kartel.Trade.Web 
// 	IoCConfig.cs
// 
// 	Created by: ykorshev 
// 	 at 07.04.2013 17:52
// 
// ============================================================

using Kartel.Domain.DAL;
using Kartel.Domain.IoC;
using Kartel.Trade.Web.Classes;
using Kartel.Trade.Web.Classes.Cache;

namespace Kartel.Trade.Web
{
    /// <summary>
    /// Инициализатор зависимостей
    /// </summary>
    public static class IoCConfig
    {
        /// <summary>
        /// Инициаилизирует
        /// </summary>
        public static void Init()
        {
            Locator.Init(new DataAccessLayer(),new WebLayer());
        }
    }
}