// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	ProductsRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 07.04.2013 17:42
// 
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;
using Lucene.Net.Documents;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// XML реализация репозитория стран
    /// </summary>
    public class CountriesRepository: ICountriesRepository
    {
        /// <summary>
        /// Инициализирует новый инстанс абстрактного репозитория для указанного типа
        /// </summary>
        public IEnumerable<Country> GetAllCountries()
        {
            var xdoc = XDocument.Load(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "Countries.xml")).Document;
            var countries = xdoc.Descendants("Country").Select(c => new Country
                {
                    Name = c.Element("Name").Value,
                    Code = c.Element("Code").Value,
                    Flag = c.Element("Flag").Value
                }).ToList();
            return countries;
        }
    }
}