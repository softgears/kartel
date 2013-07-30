// ============================================================
// 
// 	Kartel
// 	Kartel.Trade.Web 
// 	UserJsonModel.cs
// 
// 	Created by: ykorshev 
// 	 at 30.07.2013 11:05
// 
// ============================================================

using Kartel.Domain.Entities;
using Kartel.Trade.Web.Classes.Ext;
using Newtonsoft.Json;

namespace Kartel.Trade.Web.Areas.ControlPanel.Models
{
    /// <summary>
    /// Json модель пользователя
    /// </summary>
    public class UserJsonModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("subdomain")]
        public string Subdomain { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("fio")]
        public string Fio { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("skype")]
        public string Skype { get; set; }

        [JsonProperty("icq")]
        public string ICQ { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("about")]
        public string About { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("zip")]
        public string ZIP { get; set; }

        [JsonProperty("postcode")]
        public string PostCode { get; set; }

        [JsonProperty("importer")]
        public bool Importer { get; set; }

        [JsonProperty("oem")]
        public bool OEM { get; set; }

        [JsonProperty("whoseller")]
        public bool Whoseller { get; set; }

        [JsonProperty("exporter")]
        public bool Exporter { get; set; }

        [JsonProperty("odm")]
        public bool ODM { get; set; }

        [JsonProperty("singleSeller")]
        public bool SingleSeller { get; set; }

        [JsonProperty("developer")]
        public bool Developer { get; set; }

        [JsonProperty("agent")]
        public bool Agent { get; set; }

        [JsonProperty("distributor")]
        public bool Distributor { get; set; }

        [JsonProperty("ogrn")]
        public string OGRN { get; set; }

        [JsonProperty("inn")]
        public string INN { get; set; }

        [JsonProperty("kpp")]
        public string KPP { get; set; }

        [JsonProperty("accountRnumber")]
        public string AccountRNumber { get; set; }

        [JsonProperty("accountKnumber")]
        public string AccountKNumber { get; set; }

        [JsonProperty("bank")]
        public string Bank { get; set; }

        [JsonProperty("bankBik")]
        public string BankBIK { get; set; }

        [JsonProperty("dealer")]
        public string Dealer { get; set; }

        [JsonProperty("tariff")]
        public string Tariff { get; set; }

        [JsonProperty("tariffExpiration")]
        public string TariffExpiration { get; set; }

        [JsonProperty("regDate")]
        public string RegDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public UserJsonModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public UserJsonModel(User user)
        {
            Id = user.Id;
            Login = user.Login;
            Subdomain = user.Subdomain;
            Company = user.Company;
            Brand = user.Brand;
            Fio = user.FIO;
            Email = user.Email;
            Skype = user.Skype;
            ICQ = user.ICQ;
            Url = user.Url;
            Country = user.Country;
            Region = user.Region;
            City = user.City;
            About = user.About;
            Address = user.Address;
            ZIP = user.ZIP;
            PostCode = user.PostCode;
            if (user.UserOccupationInfos != null)
            {
                Importer = user.UserOccupationInfos.Importer;
                OEM = user.UserOccupationInfos.OEM;
                Whoseller = user.UserOccupationInfos.Whoseller;
                Exporter = user.UserOccupationInfos.Exporter;
                ODM = user.UserOccupationInfos.ODM;
                SingleSeller = user.UserOccupationInfos.SingleSeller;
                Developer = user.UserOccupationInfos.Developer;
                Agent = user.UserOccupationInfos.Agent;
            }
            if (user.UserLegalInfos != null)
            {
                OGRN = user.UserLegalInfos.OGRN;
                INN = user.UserLegalInfos.INN;
                KPP = user.UserLegalInfos.KPP;
                AccountRNumber = user.UserLegalInfos.AccountRNumber;
                AccountKNumber = user.UserLegalInfos.AccountKNumber;
                Bank = user.UserLegalInfos.AccountBank;
                BankBIK = user.UserLegalInfos.AccountBankBIK;
            }
            Tariff = user.Tarif;
            TariffExpiration = user.TariffExpiration.FormatDateTime();
            Dealer = user.Dealer;
            RegDate = user.Date.FormatDate();
        }

        /// <summary>
        /// Обновляет пользователя из модели
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(User user)
        {
            user.Id = this.Id;
            user.Login = this.Login;
            user.Subdomain = this.Subdomain;
            user.Company = this.Company;
            user.Brand = this.Brand;
            user.FIO = this.Fio;
            user.Email = this.Email;
            user.Skype = this.Skype;
            user.ICQ = this.ICQ;
            user.Url = this.Url;
            user.Country = this.Country;
            user.Region = this.Region;
            user.City = this.City;
            user.About = this.About;
            user.Address = this.Address;
            user.ZIP = this.ZIP;
            user.PostCode = this.PostCode;
            if (user.UserOccupationInfos == null)
            {
                user.UserOccupationInfos = new UserOccupationInfo()
                {
                    User = user
                };
            }
            user.UserOccupationInfos.Importer = this.Importer;
            user.UserOccupationInfos.OEM = this.OEM;
            user.UserOccupationInfos.Whoseller = this.Whoseller;
            user.UserOccupationInfos.Exporter = this.Exporter;
            user.UserOccupationInfos.ODM = this.ODM;
            user.UserOccupationInfos.SingleSeller = this.SingleSeller;
            user.UserOccupationInfos.Developer = this.Developer;
            user.UserOccupationInfos.Agent = this.Agent;
            if (user.UserLegalInfos == null)
            {
                user.UserLegalInfos = new UserLegalInfo()
                    {
                        User = user
                    };
            }
            user.UserLegalInfos.OGRN = this.OGRN;
            user.UserLegalInfos.INN = this.INN;
            user.UserLegalInfos.KPP = this.KPP;
            user.UserLegalInfos.AccountRNumber = this.AccountRNumber;
            user.UserLegalInfos.AccountKNumber = this.AccountKNumber;
            user.UserLegalInfos.AccountBank = this.Bank;
            user.UserLegalInfos.AccountBankBIK = this.BankBIK;
            user.Dealer = this.Dealer;
        }
    }
}