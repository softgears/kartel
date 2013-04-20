// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	UserCategory.cs
// 
// 	Created by: ykorshev 
// 	 at 20.04.2013 16:14
// 
// ============================================================
namespace Kartel.Domain.Entities
{
    /// <summary>
    /// СОзданная пользователем категория
    /// </summary>
    public partial class UserCategory
    {
        /// <summary>
        /// Количество продуктов в пользовательской категории
        /// </summary>
        /// <returns></returns>
        public int GetProductsCount()
        {
            return Products.Count;
        }
    }
}