// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	UserPhone.cs
// 
// 	Created by: ykorshev 
// 	 at 02.07.2013 12:22
// 
// ============================================================
namespace Kartel.Domain.Entities
{
    /// <summary>
    /// Записанный пользовательский телефон
    /// </summary>
    public partial class UserPhone
    {
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("{0}({1}){2}", CountryCode, CityCode, PhoneNumber);
        }
    }
}