namespace Kartel.Trade.Web.Models
{
    /// <summary>
    /// Модель авторизации на сайте
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Запомнить
        /// </summary>
        public bool Remember { get; set; }
 
    }
}