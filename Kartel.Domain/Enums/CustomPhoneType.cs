// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	CustomPhoneType.cs
// 
// 	Created by: ykorshev 
// 	 at 28.04.2013 17:43
// 
// ============================================================
namespace Kartel.Domain.Enums
{
    /// <summary>
    /// Типы телефонных номеров
    /// </summary>
    public enum CustomPhoneType: short
    {
        /// <summary>
        /// Основной номер телефона
        /// </summary>
        MainPhone = 1,

        /// <summary>
        /// Основной номер факса
        /// </summary>
        MainFax = 2,

        /// <summary>
        /// Основной номер сотового
        /// </summary>
        MainCell = 3,

        /// <summary>
        /// Дополнительный номер телефона
        /// </summary>
        CustomPhone = 4,

        /// <summary>
        /// Дополнительный номер сотового
        /// </summary>
        CustomCell = 5,

        /// <summary>
        /// Дополнительный номер факса
        /// </summary>
        CustomFax = 6

    }
}