using System;
using System.Text;

namespace Kartel.Domain.Infrastructure.Misc
{
    /// <summary>
    /// Статический класс содержащий утилитарные методы для работ со строками
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Генерирует строку, состояющую из указанного символа в указанном количестве
        /// </summary>
        /// <param name="character">Символ</param>
        /// <param name="count">Количество</param>
        /// <returns>Цельная строка</returns>
        public static string GenerateString(char character, int count)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < count; i++)
            {
                sb.Append(character);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Нормализует строку, содержащую номер телефона
        /// </summary>
        /// <param name="phone">Номер телефона</param>
        /// <returns>Нормализованный номер телефона</returns>
        public static string NormalizePhoneNumber(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return "";
            }
            var str = new StringBuilder(phone);
            str.Replace("-", string.Empty).Replace(")", string.Empty).Replace("(", string.Empty).Replace("+7", "7");
            if (phone.StartsWith("8"))
            {
                str[0] = '7';
            }
            return str.ToString();
        }

        /// <summary>
        /// Форматирует номер телефона для удобного отображения на сайте
        /// </summary>
        /// <param name="phone">Номер телефона</param>
        /// <returns></returns>
        public static string FormatPhoneNumber(this string phone)
        {
            var ph = NormalizePhoneNumber(phone);
            var sb = new StringBuilder(ph);
            if (sb.Length == 11)
            {
                return sb.Insert(0, '+').Insert(2, '-').Insert(6, '-').Insert(10, '-').Insert(13, '-').ToString();
            }
            else if (sb.Length == 6)
            {
                return sb.Insert(2, "-").Insert(5, "-").ToString();
            }
            else return ph;
        }

        /// <summary>
        /// Форматирует число как цену, разбивая его на разряды
        /// </summary>
        /// <param name="price">Цена</param>
        /// <returns></returns>
        public static string FormatPrice(this double? price)
        {
            if (price == null)
            {
                return string.Empty;
            }
            return string.Format("{0:N}", (long)price.Value).Replace(",00","");
        }

        /// <summary>
        /// Обрезает строку после указанного количества символо
        /// </summary>
        /// <param name="str">Строка</param>
        /// <param name="maxLength">Длина</param>
        /// <returns></returns>
        public static string TrimEllipsis(this string str, int maxLength)
        {
            if (str.Length > maxLength)
            {
                return String.Format("{0}...", str.Substring(0, maxLength));
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// Извлекает логин из указанного email адреса
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetEmailLogin(this string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return "";
            }
            var idx = str.IndexOf('@');
            if (idx < 0)
            {
                return str;
            }
            return str.Substring(0, idx);
        }
    }
}