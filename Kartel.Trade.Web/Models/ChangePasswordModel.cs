// ============================================================
// 
// 	Kartel
// 	Kartel.Trade.Web 
// 	ChangePasswordModel.cs
// 
// 	Created by: ykorshev 
// 	 at 20.06.2013 22:36
// 
// ============================================================
namespace Kartel.Trade.Web.Models
{
    /// <summary>
    /// Модель изменения пароля
    /// </summary>
    public class ChangePasswordModel
    {
        /// <summary>
        /// Старый пароль
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// Новый пароль
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Подтверждение нового пароля
        /// </summary>
        public string NewPasswordConfirm { get; set; }
    }
}