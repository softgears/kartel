namespace Kartel.Domain.Enums
{
    /// <summary>
    /// Информация об оплате в предложении по тендеру
    /// </summary>
    public enum TenderPaymentInfo: int
    {
        /// <summary>
        /// Полная предоплата
        /// </summary>
        [EnumDescription("100% предоплата")]
        ПолнаяПредоплата = 1,

        /// <summary>
        /// Предоплата 70%
        /// </summary>
        [EnumDescription("70% предоплата, 30% по факту поставки")]
        Предоплата70Процентов = 2,

        /// <summary>
        /// Предоплата 50%
        /// </summary>
        [EnumDescription("50% предоплата, 50% по факту поставки")]
        Предоплата50Процентов = 3,

        /// <summary>
        /// Предоплата 30%
        /// </summary>
        [EnumDescription("30% предоплата, 70% по факту поставки")]
        Предоплата30Процентов = 4,

        /// <summary>
        /// Без предоплаты
        /// </summary>
        [EnumDescription("100% оплаты по факту поставки")]
        ПредоплатыНет = 5,

        /// <summary>
        /// Другое
        /// </summary>
        [EnumDescription("Другое")]
        Другое = 6
    }
}