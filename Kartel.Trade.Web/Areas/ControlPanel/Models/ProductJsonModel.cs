// ============================================================
// 
// 	Kartel
// 	Kartel.Trade.Web 
// 	ProductJsonModel.cs
// 
// 	Created by: ykorshev 
// 	 at 30.07.2013 12:42
// 
// ============================================================

using System.Linq;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;
using Kartel.Trade.Web.Classes.Ext;
using Newtonsoft.Json;

namespace Kartel.Trade.Web.Areas.ControlPanel.Models
{
    /// <summary>
    /// JSON модель продукта
    /// </summary>
    public class ProductJsonModel
    {
        /// <summary>
        /// ИД товара
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("keywords")]
        public string Keywords { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("img")]
        public string Img { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("field1")]
        public string Field1 { get; set; }

        [JsonProperty("field2")]
        public string Field2 { get; set; }

        [JsonProperty("field3")]
        public string Field3 { get; set; }

        [JsonProperty("field4")]
        public string Field4 { get; set; }

        [JsonProperty("field5")]
        public string Field5 { get; set; }

        [JsonProperty("field6")]
        public string Field6 { get; set; }

        [JsonProperty("field7")]
        public string Field7 { get; set; }

        [JsonProperty("field8")]
        public string Field8 { get; set; }

        [JsonProperty("field9")]
        public string Field9 { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("measure")]
        public string Measure { get; set; }

        [JsonProperty("minimumLotSize")]
        public string MinimumLotSize { get; set; }

        [JsonProperty("minimumLotMeasure")]
        public string MinimumLotMeasure { get; set; }

        [JsonProperty("vendorCountry")]
        public string VendorCountry { get; set; }

        [JsonProperty("deliveryTime")]
        public string DeliveryTime { get; set; }

        [JsonProperty("deliveryPossibilityDay")]
        public string DeliveryPossibilityDay { get; set; }

        [JsonProperty("deliveryPossibilityMeasure")]
        public string DeliveryPossibilityMeasure { get; set; }

        [JsonProperty("deliveryPossibilityTime")]
        public string DeliveryPossibilityTime { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("productBox")]
        public string ProductBox { get; set; }

        [JsonProperty("userCategoryId")]
        public int UserCategoryId { get; set; }

        [JsonProperty("categoryId")]
        public int CategoryId { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }

        [JsonProperty("userCategoryName")]
        public string UserCategoryName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ProductJsonModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ProductJsonModel(Product product)
        {
            Id = product.Id;
            Title = product.Title;
            Keywords = product.Keywords;
            Description = product.Description;
            Img = product.Img;
            Date = product.Date.FormatDateTime();
            Field1 = product.Field1;
            //Field2 = product.Field2;
            Field3 = product.Field3;
            Field4 = product.Field4;
            Field5 = product.Field5;
            Field6 = product.Field6;
            //Field7 = product.Field7;
            Field8 = product.Field8;
            Field9 = product.Field9;
            Price = product.Price;
            Currency = product.Currency;
            Measure = product.Measure;
            MinimumLotMeasure = product.MinimunLotSize;
            MinimumLotMeasure = product.MinimumLotMeasure;
            VendorCountry = product.VendorCountry;
            DeliveryTime = product.DeliveryTime;
            DeliveryPossibilityDay = product.DeliveryPossibilityDay;
            DeliveryPossibilityTime = product.DeliveryPossibilityTime;
            DeliveryPossibilityMeasure = product.DeliveryPossibilityMeasure;
            ProductCode = product.ProductCode;
            ProductBox = product.ProductBox;
            CategoryId = product.CategoryId;
            UserCategoryId = product.UserCategoryId;
            UserCategoryName = product.UserCategory != null ? product.UserCategory.Title : "";
            CategoryName = product.Category != null ? product.Category.Title : "";
            UserId = product.UserId;
        }

        /// <summary>
        /// Обновляет продукт
        /// </summary>
        /// <param name="item"></param>
        public void UpdateProduct(Product item)
        {
            item.Title = this.Title;
            item.Keywords = this.Keywords;
            item.Description = this.Description;
            item.Img = this.Img;
            item.Field1 = this.Field1;
            item.Field3 = this.Field3;
            item.Field4 = this.Field4;
            item.Field5 = this.Field5;
            item.Field6 = this.Field6;
            item.Field9 = this.Field9;
            item.Price = this.Price;
            item.Currency = this.Currency;
            item.Measure = this.Measure;
            item.MinimumLotMeasure = this.MinimumLotSize;
            item.MinimumLotMeasure = this.MinimumLotMeasure;
            item.VendorCountry = this.VendorCountry;
            item.DeliveryTime = this.DeliveryTime;
            item.DeliveryPossibilityDay = this.DeliveryPossibilityDay;
            item.DeliveryPossibilityTime = this.DeliveryPossibilityTime;
            item.DeliveryPossibilityMeasure = this.DeliveryPossibilityMeasure;
            item.ProductCode = this.ProductCode;
            item.ProductBox = this.ProductBox;
            if (this.CategoryId != item.CategoryId)
            {
                item.Category = Locator.GetService<ICategoriesRepository>().Load(this.CategoryId);
            }
            if (this.UserCategoryId != item.UserCategoryId && item.User != null)
            {
                item.UserCategory = item.User.UserCategories.FirstOrDefault(c => c.Id == this.UserCategoryId);
            }
            if (this.UserId != item.UserId)
            {
                item.User = Locator.GetService<IUsersRepository>().Load(this.UserId);
            }
        }
    }
}