// ============================================================
// 
// 	Kartel
// 	Kartel.Trade.Web 
// 	TenderJsonModel.cs
// 
// 	Created by: ykorshev 
// 	 at 30.07.2013 14:37
// 
// ============================================================

using System;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;
using Kartel.Trade.Web.Classes.Ext;
using Newtonsoft.Json;

namespace Kartel.Trade.Web.Areas.ControlPanel.Models
{
    /// <summary>
    /// JSON модель тендера
    /// </summary>
    public class TenderJsonModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("categoryId")]
        public int CategoryId { get; set; }

        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("minprice")]
        public string MinPrice { get; set; }

        [JsonProperty("maxprice")]
        public string MaxPrice { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("measure")]
        public string Measure { get; set; }

        [JsonProperty("dateStart")]
        public DateTime? DateStart { get; set; }

        [JsonProperty("dateEnd")]
        public DateTime? DateEnd { get; set; }

        [JsonProperty("period")]
        public string Period { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("keywords")]
        public string Keywords { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TenderJsonModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TenderJsonModel(Tender tender)
        {
            Id = tender.Id;
            UserId = tender.UserId;
            CategoryId = tender.CategoryId;
            CategoryName = tender.Category != null ? tender.Category.Title : "";
            Title = tender.Title;
            Description = tender.Description;
            MinPrice = tender.MinPrice.ToString("0");
            MaxPrice = tender.MaxPrice.ToString("0");
            Currency = tender.Currency;
            Size = tender.Size;
            Measure = tender.Measure;
            DateStart = tender.DateStart;
            DateEnd = tender.DateEnd;
            Period = tender.Period;
            Image = tender.Image;
            Keywords = tender.Keywords;
            DateCreated = tender.DateCreated;
        }

        /// <summary>
        /// Обновляет содержимое тендера из модели
        /// </summary>
        /// <param name="tender"></param>
        public void UpdateTender(Tender tender)
        {
            tender.Title = this.Title;
            tender.Description = this.Description;
            tender.MinPrice = Convert.ToDecimal(this.MinPrice);
            tender.MaxPrice = Convert.ToDecimal(this.MaxPrice);
            tender.Currency = this.Currency;
            tender.Size = this.Size;
            tender.Measure = this.Measure;
            tender.DateStart = this.DateStart;
            tender.DateEnd = this.DateEnd;
            tender.Period = this.Period;
            tender.Image = this.Image;
            tender.Keywords = this.Keywords;
            if (this.CategoryId != tender.CategoryId)
            {
                tender.Category = Locator.GetService<ICategoriesRepository>().Load(this.CategoryId);
            }
            if (this.UserId != tender.UserId)
            {
                tender.User = Locator.GetService<IUsersRepository>().Load(this.UserId);
            }
        }
    }
}