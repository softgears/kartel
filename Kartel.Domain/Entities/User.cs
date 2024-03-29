﻿// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	User.cs
// 
// 	Created by: ykorshev 
// 	 at 20.04.2013 11:10
// 
// ============================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kartel.Domain.Enums;

namespace Kartel.Domain.Entities
{
    /// <summary>
    /// Зарегистрированный пользователь системы
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// Легальная информация о пользователе
        /// </summary>
        /// <returns></returns>
        public UserLegalInfo GetLegalInfo()
        {
            if (UserLegalInfos == null)
            {
                UserLegalInfos = new UserLegalInfo()
                    {
                        User = this
                    };
            }
            return UserLegalInfos;
        }

        /// <summary>
        /// Возвращает категории пользователя
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserCategory> GetUserCategories()
        {
            return UserCategories;
        }

        /// <summary>
        /// Возвращает список тендеров
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tender> GetTenders()
        {
            return Tenders;
        }

        /// <summary>
        /// Отображает список всех отправленых заявок на участие в тендерах
        /// </summary>
        /// <returns></returns>
        public IList<TenderOffer> GetTenderOffers()
        {
            return TenderOffers.OrderBy(to => to.DateCreated).ToList();
        }

        /// <summary>
        /// Возвращает основной контактный телефон пользователя
        /// </summary>
        /// <returns></returns>
        public string GetMainPhone()
        {
            if (UserPhones.Count > 0)
            {
                return UserPhones.First().ToString();
            }
            else
            {
                var parts = Phone.Split('|');
                if(parts.Length > 0)
                {
                    return parts[0];
                }
                else
                {
                    return Phone.Substring(0, 11);
                }
            }
        }

        /// <summary>
        /// Возвращает указанное количество случайных товаров
        /// </summary>
        /// <param name="count">Количество товаров</param>
        /// <returns></returns>
        public IList<Product> GetRandomProducts(int count = 4)
        {
            return Products.OrderBy(d => Guid.NewGuid()).Take(count).ToList();
        }

        /// <summary>
        /// Возвращает все товары пользователя
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetProducts()
        {
            return Products.OrderBy(p => p.Title);
        }

        /// <summary>
        /// Возвращает пользователькие номера телефонов
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserPhone>  GetUserPhones()
        {
            return UserPhones;
        }

        /// <summary>
        /// Возвращает основной номер телефона
        /// </summary>
        /// <returns></returns>
        public UserPhone GetMainUserPhone()
        {
            return UserPhones.FirstOrDefault(up => up.Type == (short) CustomPhoneType.MainPhone) ?? new UserPhone();
        }

        /// <summary>
        /// Возвращает основной номер факса
        /// </summary>
        /// <returns></returns>
        public UserPhone GetMainFaxPhone()
        {
            return UserPhones.FirstOrDefault(up => up.Type == (short) CustomPhoneType.MainFax) ?? new UserPhone();
        }

        /// <summary>
        /// Возвращает основной номер сотового
        /// </summary>
        /// <returns></returns>
        public UserPhone GetMainCellPhone()
        {
            return UserPhones.FirstOrDefault(up => up.Type == (short) CustomPhoneType.MainCell) ?? new UserPhone();
        }

        public IEnumerable<UserBanner> GetBanners()
        {
            return UserBanners.Where(b => b.Enabled).OrderBy(b => b.BannerPosition);
        }

        /// <summary>
        /// Формирует список возможных ссылок для баннера пользователя
        /// </summary>
        /// <returns></returns>
        public IDictionary<string,string> GetBannerHrefs()
        {
            var res = new Dictionary<string, string>();
            // Основные страницы
            res["/vendor/" + Id] = "Главная";
            res["/products/" + Id] = "Товары";
            res["/vendor/about/" + Id] = "О компании";
            res["/vendor/contacts/" + Id] = "Контакты";
            foreach(var userCat in UserCategories)
            {
                res["/vendor/category/" + userCat.Id] = userCat.Title;
            }
            foreach (var product in Products)
            {
                res["/product/" + product.Id] = product.Title;
            }
            return res;
        }
    }
}