// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	SettingsRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 28.06.2013 10:56
// 
// ============================================================

using System;
using System.ComponentModel;
using System.Globalization;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// СУБД реализация репозитория настроек
    /// </summary>
    public class SettingsRepository: BaseRepository<Setting>, ISettingsRepository
    {
        /// <summary>
        /// Инициализирует новый инстанс абстрактного репозитория для указанного типа
        /// </summary>
        /// <param name="dataContext"></param>
        public SettingsRepository(KartelDataContext dataContext) : base(dataContext)
        {
        }

        /// <summary>
        /// Загружает указанную сущность по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или Null</returns>
        public override Setting Load(long id)
        {
            throw new NotImplementedException("Тут такое не нужно");
        }

        /// <summary>
        /// Получает настройку в виде строки
        /// </summary>
        /// <param name="name">Имя-ключ настройки</param>
        /// <returns></returns>
        public string GetValue(string name)
        {
            var set = Find(s => s.Key.ToLower() == name.ToLower());
            if (set == null)
            {
                return null;
            }
            else
            {
                return set.Value;
            }
        }

        /// <summary>
        /// Получает настройку, конвертирую ее в указанный тип
        /// </summary>
        /// <typeparam name="T">Тип настройки, к которому конвертировать</typeparam>
        /// <param name="name">Имя-ключ настройки</param>
        /// <returns></returns>
        public T GetValue<T>(string name)
        {
            string val = GetValue(name);
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (typeof(T) != typeof(double))
            {
                return (T)converter.ConvertFrom(val);
            }
            else
            {
                return (T)((object)Convert.ToDouble(val, CultureInfo.InvariantCulture));
            }

        }

        /// <summary>
        /// Устанавливает настройку, конвертирую ее в строку
        /// </summary>
        /// <param name="name">Имя-ключ настройки</param>
        /// <param name="value">Значениен настройки</param>
        public void SetValue(string name, object value)
        {
            var set = Find(s => s.Key.ToLower() == name.ToLower());
            if (set == null)
            {
                set = new Setting()
                {
                    Value = value.ToString()
                };
                Add(set);
            }
            else
            {
                set.Value = value.ToString();
            }
        }

        /// <summary>
        /// Проверяет, была ли когда-либо установлена указанная настройка
        /// </summary>
        /// <param name="name">Наименование настройки</param>
        /// <returns>true если была, иначе false</returns>
        public bool HasSetting(string name)
        {
            return Find(s => s.Key.ToLower() == name.ToLower()) != null;
        }
    }
}