// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	KartelDataContext.cs
// 
// 	Created by: ykorshev 
// 	 at 07.04.2013 17:38
// 
// ============================================================

using System.Configuration;

namespace Kartel.Domain.DAL
{
    /// <summary>
    /// Контекст доступа к данным
    /// </summary>
    public partial class KartelDataContext
    {
        /// <summary>
        /// Конструктор из конфигурации
        /// </summary>
        public KartelDataContext() : base(ConfigurationManager.ConnectionStrings["MainConnectionString"].ConnectionString)
        {
        }
    }
}