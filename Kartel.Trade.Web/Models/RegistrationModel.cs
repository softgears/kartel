namespace Kartel.Trade.Web.Models
{
    /// <summary>
    /// МОдель используемая при регистрации
    /// </summary>
    public class RegistrationModel
    {
        /// <summary>
        /// Организация
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Страна
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Город
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Код страны
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Код города
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Адрес сайта
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Пароль 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Подтверждение пароля
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Код значения капчи
        /// </summary>
        public string CaptchaValue { get; set; }
    }
}