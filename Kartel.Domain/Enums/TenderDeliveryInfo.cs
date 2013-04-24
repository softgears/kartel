namespace Kartel.Domain.Enums
{
    /// <summary>
    /// Информация о доставке в предложении к тендеру
    /// </summary>
    public enum TenderDeliveryInfo: int
    {
        /// <summary>
        /// Доставка входит в стоимость
        /// </summary>
        [EnumDescription("Входит в стоимость")]
        ВходитВСтоимость = 1,

        /// <summary>
        /// За доставку платит покупатель
        /// </summary>
        [EnumDescription("Платит покупатель")]
        ПлатитПокупатель = 2,

        /// <summary>
        /// Доставка до транспортной компании
        /// </summary>
        [EnumDescription("Доставка до транспортной компании")]
        ДоставкаДоТранспортнойКомпании = 3,

        /// <summary>
        /// Доставка до жд станции
        /// </summary>
        [EnumDescription("Доставка до Порта / ЖД станции")]
        ДоставкаДоПортаЖдСтанции = 4,

        /// <summary>
        /// Другое
        /// </summary>
        [EnumDescription("Другое")]
        Другое = 5
    }
}