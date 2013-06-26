using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartel.Domain.Entities;
using Kartel.Domain.Enums;
using Kartel.Domain.Infrastructure.Misc;
using Kartel.Domain.Infrastructure.Routing;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;
using Kartel.Trade.Web.Classes.Utils;
using Kartel.Trade.Web.Models;
using XCaptcha;

namespace Kartel.Trade.Web.Controllers
{
    /// <summary>
    /// Контроллер управления поьзователями и личным кабинетом
    /// </summary>
    public class AccountController : BaseController
    {
        /// <summary>
        /// Репозиторий пользователей
        /// </summary>
        public IUsersRepository UsersRepository { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Mvc.Controller"/> class.
        /// </summary>
        public AccountController(IUsersRepository usersRepository)
        {
            UsersRepository = usersRepository;
        }

        #region Регистрация

        /// <summary>
        /// Проверяет уникальность имени пользователя
        /// </summary>
        /// <param name="Email">Email пользователя</param>
        /// <returns></returns>
        public ActionResult CheckLogin(string Email)
        {
            var email = UsersRepository.ExistsUserWithLogin(Email);
            if (email)
            {
                return Content("\"Пользователь с таким Email уже зарегистрирован\"");
            }
            else
            {
                return Content("true");
            }
        }

        /// <summary>
        /// Возвращает каптчу и устанавливает ее ключ в сессию текущего контекста
        /// </summary>
        /// <returns></returns>
        public ActionResult RegCaptcha()
        {
            var builder = new XCaptcha.ImageBuilder()
            {
                Canvas = new DefaultCanvas()
                {
                    Width = 115,
                    Height = 37
                }
            };
            var captcha = builder.Create(new Random(System.Environment.TickCount).Next(65535).ToString());
            Session.Add("reg_captcha", captcha.Solution);
            return new FileContentResult(captcha.Image, captcha.ContentType);
        }

        /// <summary>
        /// Проверяет правильность каптчи регистрации
        /// </summary>
        /// <param name="captcha">Каптча</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckRegCaptcha(string captcha)
        {
            var solution = Session["reg_captcha"];
            if (solution == null || solution.ToString() != captcha)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }

        /// <summary>
        /// Отображает страницу регистарции на сайте
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            // Навигационная цепочка
            PushNavigationChainItem("Главная страница","/");
            PushNavigationChainItem("Регистрация","",true);

            // Отображаем
            return View();
        }

        /// <summary>
        /// Обрабатывает регистарцию пользователя
        /// </summary>
        /// <param name="model">Модель данных</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(RegistrationModel model)
        {
            // Проверяем уникальность пользователя
            var exists = UsersRepository.ExistsUserWithLogin(model.Email);
            if (exists)
            {
                ViewBag.message = "На сайте уже зарегистрирован пользователь с таким адресом";
                return View("RegistrationResult");
            }

            // Проверяем соответствие кода каптчи
            var solution = Session["reg_captcha"];
            if (solution == null || solution.ToString() != model.CaptchaValue)
            {
                ViewBag.message = "Не правильный код подтверждения каптчи";
                return View("RegistrationResult");
            }

            // Видимо все ок - создаем нового пользователя
            var newUser = new User()
                {
                    Company = model.OrganizationName,
                    Address = model.Address,
                    City = model.City,
                    Region = model.Region,
                    Login = model.Email,
                    Email = model.Email,
                    Phone = String.Format("{0}{1}{2}", model.CountryCode, model.CityCode, model.PhoneNumber),
                    Url = model.Website,
                    PasswordHash = PasswordUtils.QuickMD5(model.Password),
                    Country = model.Country,
                    Date = DateTime.Now,
                    Tarif = "free"
                };
            newUser.UserPhones.Add(new UserPhone()
                {
                    User = newUser,
                    CityCode = model.CityCode,
                    CountryCode = model.CountryCode,
                    PhoneNumber = model.PhoneNumber
                });
            UsersRepository.Add(newUser);
            UsersRepository.SubmitChanges();

            // Уведомляем об успешном создании
            ViewBag.message = "Вы были успешно зарегистрированы на нашем сайте. Теперь вы можете войти в личный кабинет и добавить свои товары в наш каталог";
            return View("RegistrationResult");
        }

        #endregion

        #region Авторизация

        /// <summary>
        /// Отображает страницу входа
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Вход в личный кабинет", "", true);

            if (IsAuthentificated)
            {
                return RedirectToAction("Index"); // Главная страница личного кабинета
            }

            return View();
        }

        /// <summary>
        /// Обрабатывает авторизацию на сайте
        /// </summary>
        /// <param name="model">Модель данных авторизации</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (IsAuthentificated)
            {
                return RedirectToAction("Index");
            }

            // Ищем пользователя
            var user = UsersRepository.GetUserByLoginAndPasswordHash(model.Login, PasswordUtils.QuickMD5(model.Password));
            if (user == null)
            {
                // Выдаем информацию
                this.ModelState.AddModelError("common","Не найдена такая пара имени пользователя и пароля");

                // Навигационная цепочка
                PushNavigationChainItem("Главная страница", "/");
                PushNavigationChainItem("Вход в личный кабинет", "", true);

                return View("Login");
            }

            // Пользователь найден - авторизуемся
            user.LoggedDate = DateTime.Now;
            user.LoggedIP = Request.UserHostAddress;
            UsersRepository.SubmitChanges();

            // Авторизуем пользователя
            AuthorizeUser(user,model.Remember);

            // Перенаправляем в личный кабинет
            return RedirectToAction("Index");

        }

        /// <summary>
        /// Отрабатывает выход из личного кабинета
        /// </summary>
        /// <returns></returns>
        public ActionResult Logoff()
        {
            // Если авторизованы то закрываем.
            if (IsAuthentificated)
            {
                CloseAuthorization();
            }

            return RedirectToAction("Index","Main");
        }

        #endregion

        #region Главная страница

        /// <summary>
        /// Отображает главную страницу личного кабинета
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Личный кабинет", "", false);
            PushNavigationChainItem("Профиль компании", "", true);

            return View(CurrentUser);
        }

        /// <summary>
        /// Обрабатывает сохранение профайла компании
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost][Route("account/save-profile")]
        public ActionResult SaveCompanyProfile(User model, FormCollection collection)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Сохраняем
            // Основное инфо
            CurrentUser.Company = model.Company;
            CurrentUser.Brand = model.Brand;
            CurrentUser.FIO = model.FIO;

            // Основной телефон
            var mainPhone = CurrentUser.GetMainUserPhone();
            if (mainPhone.User == null)
            {
                mainPhone.User = CurrentUser;
                mainPhone.Type = (short) CustomPhoneType.MainPhone;
                CurrentUser.UserPhones.Add(mainPhone);
            }
            mainPhone.CountryCode = collection["PhoneCountryCode"];
            mainPhone.CityCode = collection["PhoneCityCode"];
            mainPhone.PhoneNumber = collection["PhoneNumber"];

            // Факс
            var faxPhone = CurrentUser.GetMainFaxPhone();
            if (faxPhone.User == null)
            {
                faxPhone.User = CurrentUser;
                faxPhone.Type = (short) CustomPhoneType.MainFax;
                CurrentUser.UserPhones.Add(faxPhone);
            }
            faxPhone.CountryCode = collection["FaxCountryCode"];
            faxPhone.CityCode = collection["FaxCityCode"];
            faxPhone.PhoneNumber = collection["FaxNumber"];

            // Сотовый
            var cellPhone = CurrentUser.GetMainCellPhone();
            if (cellPhone.User == null)
            {
                cellPhone.User = CurrentUser;
                cellPhone.Type = (short) CustomPhoneType.MainCell;
                CurrentUser.UserPhones.Add(cellPhone);
            }
            cellPhone.CountryCode = collection["CellPhoneCountryCode"];
            cellPhone.CityCode = collection["CellPhoneCityCode"];
            cellPhone.PhoneNumber = collection["CellPhoneNumber"];

            // Другие контакты
            CurrentUser.Skype = model.Skype;
            CurrentUser.ICQ = model.ICQ;
            CurrentUser.Url = model.Url;
            CurrentUser.Country = model.Country;
            CurrentUser.Region = model.Region;
            CurrentUser.Address = model.Address;
            CurrentUser.City = model.City;
            CurrentUser.PostCode = model.PostCode;

            // Деятельность
            if (CurrentUser.UserOccupationInfos == null)
            {
                CurrentUser.UserOccupationInfos = new UserOccupationInfo()
                    {
                        User = CurrentUser
                    };
            }
            CurrentUser.UserOccupationInfos.Importer = model.UserOccupationInfos.Importer;
            CurrentUser.UserOccupationInfos.OEM = model.UserOccupationInfos.OEM;
            CurrentUser.UserOccupationInfos.Whoseller = model.UserOccupationInfos.Whoseller;
            CurrentUser.UserOccupationInfos.Exporter = model.UserOccupationInfos.Exporter;
            CurrentUser.UserOccupationInfos.ODM = model.UserOccupationInfos.ODM;
            CurrentUser.UserOccupationInfos.SingleSeller = model.UserOccupationInfos.SingleSeller;
            CurrentUser.UserOccupationInfos.Developer = model.UserOccupationInfos.Developer;
            CurrentUser.UserOccupationInfos.Agent = model.UserOccupationInfos.Agent;
            CurrentUser.UserOccupationInfos.Distributor = model.UserOccupationInfos.Distributor;

            // О компании
            CurrentUser.About = model.About;

            // Банковские реквизиты
            if (CurrentUser.UserLegalInfos == null)
            {
                CurrentUser.UserLegalInfos = new UserLegalInfo()
                {
                    User = CurrentUser
                };
            }
            CurrentUser.UserLegalInfos.OGRN = model.UserLegalInfos.OGRN;
            CurrentUser.UserLegalInfos.INN = model.UserLegalInfos.INN;
            CurrentUser.UserLegalInfos.KPP = model.UserLegalInfos.KPP;
            CurrentUser.UserLegalInfos.AccountRNumber = model.UserLegalInfos.AccountRNumber;
            CurrentUser.UserLegalInfos.AccountKNumber = model.UserLegalInfos.AccountKNumber;
            CurrentUser.UserLegalInfos.AccountBank = model.UserLegalInfos.AccountBank;
            CurrentUser.UserLegalInfos.AccountBankBIK = model.UserLegalInfos.AccountBankBIK;

            // Дилер
            CurrentUser.Dealer = model.Dealer;

            // Сохраняем фотографии товара и контактного лица
            var logoImage = Request.Files["LogoImage"];
            if (logoImage != null && logoImage.ContentLength > 0 && logoImage.ContentType.Contains("image"))
            {
                var fileName = String.Format("logo-{0}-{1}{2}", CurrentUser.Id,
                                             new Random(System.Environment.TickCount).Next(Int32.MaxValue),
                                             Path.GetExtension(logoImage.FileName));
                FileUtils.SavePostedFile(logoImage, "userimage", fileName);
                CurrentUser.LogoUrl = fileName;
            }
            var fioImage = Request.Files["FIOImage"];
            if (fioImage != null && fioImage.ContentLength > 0 && fioImage.ContentType.Contains("image"))
            {
                var fileName = String.Format("fio-{0}-{1}{2}", CurrentUser.Id,
                                             new Random(System.Environment.TickCount).Next(Int32.MaxValue),
                                             Path.GetExtension(fioImage.FileName));
                FileUtils.SavePostedFile(fioImage, "userimage", fileName);
                CurrentUser.FIOImg = fileName;
            }

            // Сохраняем
            UsersRepository.SubmitChanges();

            return View("ProfileSaved");
        }

        /// <summary>
        /// Обрабатывает сохранение поддомена для текущего пользователя
        /// </summary>
        /// <param name="domain">Выбранный поддомен</param>
        /// <returns></returns>
        [HttpPost][Route("account/domain-save")]
        public ActionResult DomainSave(string domain)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            if (string.IsNullOrEmpty(domain))
            {
                domain = CurrentUser.Id.ToString();
            }

            // Проверяем
            var exists = UsersRepository.Find(u => u.Subdomain.ToLower() == domain.ToLower() && u.Id != CurrentUser.Id) != null;
            if  (exists)
            {
                ViewBag.message = "Такой домен уже зарегистрирован в системе";
                return View();
            }

            // Устанавливаем
            CurrentUser.Subdomain = domain.ToLower();
            UsersRepository.SubmitChanges();

            // Отображаем вид
            ViewBag.message = string.Format("Ваш поддомен был успешно изменен. Теперь вы можете зайти на ваш сайт по адрес <a href='http://{0}.karteltrade.ru'>http://{0}.karteltrade.ru</a>", domain.ToLower());
            return View();
        }

        #endregion

        #region Управление товарами

        /// <summary>
        /// Отображает главную страницу управления товарами
        /// </summary>
        /// <returns></returns>
        public ActionResult Products()
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Личный кабинет", "", false);
            PushNavigationChainItem("Товары", "", true);

            // Отображаем вид
            return View();
        }

        #region Категории

        /// <summary>
        /// Отображает форму добавления новой пользовательской категории
        /// </summary>
        /// <returns></returns>
        [Route("account/products/add-category")]
        public ActionResult AddCategory()
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Личный кабинет", "", false);
            PushNavigationChainItem("Товары", "/account/products", false);
            PushNavigationChainItem("Создание категории", "", true);

            // Отображаем вид
            return View("EditCategory",new UserCategory());
        }

        /// <summary>
        /// Обрабатывает категории - создает или сохраняет новую пользовательскую категорию
        /// </summary>
        /// <param name="model">Модель</param>
        /// <returns></returns>
        [Route("account/products/save-category")][HttpPost]
        public ActionResult SaveCategory(UserCategory model)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Проверяем что у нас - создание или редактирование
            UserCategory category = null;
            if (model.Id <= 0)
            {
                category = model;
                category.Position = CurrentUser.UserCategories.Count > 0
                                        ? CurrentUser.UserCategories.Max(c => c.Position) + 1000
                                        : 1000;
                CurrentUser.UserCategories.Add(category);
            }
            else
            {
                category = CurrentUser.UserCategories.First(c => c.Id == model.Id);
                category.Title = model.Title;
                category.Description = model.Description;
                category.Position = model.Position;
            }

            // Сохраняем
            UsersRepository.SubmitChanges();

            // Переходим на страницу товара
            return RedirectToAction("Products");
        }

        /// <summary>
        /// Отображает форму редактирования пользовательской категории
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns></returns>
        [Route("account/products/edit-category/{id}")]
        public ActionResult EditCategory(long id)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Личный кабинет", "", false);
            PushNavigationChainItem("Товары", "/account/products", false);
            PushNavigationChainItem("Редактирование категории", "", true);

            var category = CurrentUser.UserCategories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return RedirectToAction("Products");
            }

            return View("EditCategory", category);
        }

        /// <summary>
        /// Удаляет указанную категорию товара
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns></returns>
        [Route("account/products/delete-category/{id}")]
        public ActionResult DeleteCategory(long id)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Ищем категорию и удаляем
            var category = CurrentUser.UserCategories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return RedirectToAction("Products");
            }
            CurrentUser.UserCategories.Remove(category);
            category.User = null;
            foreach(var product in category.Products)
            {
                product.Category = null;
            }

            // Сохраняем
            UsersRepository.SubmitChanges();

            // Переходим на список категорий
            return RedirectToAction("Products");
        }

        /// <summary>
        /// Отображает список товаров, который находится в указанной пользовательской категории
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns>Страница со список категорий</returns>
        [Route("account/products/category-products/{id}")]
        public ActionResult CategoryProducts(long id)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            var userCategory = CurrentUser.UserCategories.FirstOrDefault(c => c.Id == id);
            if (userCategory == null)
            {
                return RedirectToAction("Products");
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Личный кабинет", "", false);
            PushNavigationChainItem("Товары", "/account/products", false);
            PushNavigationChainItem(string.Format("Товары в категории \"{0}\"", userCategory.Title), "", true);

            // Отображаем вид
            return View(userCategory.Products.ToList());
        }

        #endregion

        #region Товары

        /// <summary>
        /// Отображает форму добавления нового товара на сайт
        /// </summary>
        /// <returns></returns>
        [Route("account/products/add-product")]
        public ActionResult AddProduct()
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Личный кабинет", "", false);
            PushNavigationChainItem("Товары", "/account/products", false);
            PushNavigationChainItem("Добавление товара", "", true);

            // Отображаем вид
            return View("EditProduct", new Product());
        }

        /// <summary>
        /// Возвращает разметку дочерних категорий к указанной категории
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost][Route("account/products/get-sub-categories")]
        public ActionResult GetProductSubCategories(long id)
        {
            // Репозиторий категорий
            var rep = Locator.GetService<ICategoriesRepository>();

            // Ищем категорию
            var category = rep.Load(id);
            if (category == null)
            {
                return Content("Не найдена категория");
            }

            return PartialView("ProductSubCategories", category.ChildCategories.OrderBy(c => c.Title).ToList());
        }

        /// <summary>
        /// Возвращает строку полного пути указанной категории
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns>Строка пути</returns>
        [HttpPost]
        [Route("account/products/get-category-path")]
        public ActionResult GetCategoryPath(long id)
        {
            // Репозиторий категорий
            var rep = Locator.GetService<ICategoriesRepository>();

            // Ищем категорию
            var category = rep.Load(id);
            if (category == null)
            {
                return Content("");
            }

            return Content(String.Join(" / ", category.GetFullCategoriesPath().Select(c => c.Title)));
        }

        /// <summary>
        /// Выполняет сохранение продукта - создание или изменение существующего
        /// </summary>
        /// <param name="model">Модель с формы</param>
        /// <returns></returns>
        [HttpPost][ValidateInput(false)]
        [Route("account/products/save-product")]
        public ActionResult SaveProduct(Product model)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Проверяем что у нас - создание или сохранение
            Product product = null;
            if (model.Id <= 0)
            {
                product = model;
                product.Date = DateTime.Now;
                product.User = CurrentUser;
                CurrentUser.Products.Add(product);
            }
            else
            {
                product = Locator.GetService<IProductsRepository>().Load(model.Id);
                product.Title = model.Title;
                product.Keywords = model.Keywords;
                product.CategoryId = model.CategoryId;
                product.UserCategoryId = model.UserCategoryId;
                product.User = CurrentUser;
                product.Field1 = model.Field1;
                product.Field3 = model.Field3;
                product.Field4 = model.Field4;
                product.Field5 = model.Field5;
                product.Field6 = model.Field6;
                product.Field8 = model.Field8;
                product.Field9 = model.Field9;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Currency = model.Currency;
                product.Measure = model.Measure;
                product.MinimunLotSize = model.MinimunLotSize;
                product.MinimumLotMeasure = model.MinimumLotMeasure;
                product.VendorCountry = model.VendorCountry;
                product.DeliveryTime = model.DeliveryTime;
                product.DeliveryPossibilityDay = model.DeliveryPossibilityDay;
                product.DeliveryPossibilityMeasure = model.DeliveryPossibilityMeasure;
                product.DeliveryPossibilityTime = model.DeliveryPossibilityTime;
                product.ProductCode = model.ProductCode;
                product.ProductBox = model.ProductBox;

                // Информация о горячем товаре
                if (product.HotProducts == null)
                {
                    product.HotProducts = new HotProduct()
                        {
                            Clicks = 0,
                            EnableHotProduct = false,
                            PayedViews = 0,
                            Product = product,
                            Views = 0
                        };
                }
                product.HotProducts.EnableHotProduct = model.HotProducts.EnableHotProduct;
            }

            // Сохраняем изменения
            UsersRepository.SubmitChanges();

            // Сохраняем фотографию
            var productImageFile = Request.Files["ProductImage"];
            if (productImageFile != null && productImageFile.ContentLength > 0 && productImageFile.ContentType.Contains("image"))
            {
                var fileName = String.Format("prod-{0}-{1}{2}", product.Id,
                                             new Random(System.Environment.TickCount).Next(Int32.MaxValue),
                                             Path.GetExtension(productImageFile.FileName));
                FileUtils.SavePostedFile(productImageFile,"prodimage",fileName);
                product.Img = fileName;
                UsersRepository.SubmitChanges();
            }

            // Перенаправляемся на список продуктов
            return RedirectToAction("CategoryProducts",new {id = product.UserCategoryId});
        }

        /// <summary>
        /// Отображает страницу редактирования товара
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <returns></returns>
        [Route("account/products/edit-product/{id}")]
        public ActionResult EditProduct(long id)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Ищем товар и открываем его
            var product = CurrentUser.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return RedirectToAction("Products");
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Личный кабинет", "", false);
            PushNavigationChainItem("Товары", "/account/products", false);
            PushNavigationChainItem(string.Format("Редактирование товара \"{0}\"", product.Title), "", true);

            // Вид
            return View("EditProduct", product);
        }

        /// <summary>
        /// Удаляет товар с указанным идентификатором
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <returns></returns>
        [Route("account/products/delete-product/{id}")]
        public ActionResult DeleteProduct(long id)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Ищем товар и открываем его
            var product = CurrentUser.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return RedirectToAction("Products");
            }

            // Запоминаем куда возвращаться
            var catId = product.UserCategoryId;

            // Удаляем товар
            Locator.GetService<IProductsRepository>().Delete(product);
            UsersRepository.SubmitChanges();

            // Переходим в запомненную категорию
            return RedirectToAction("CategoryProducts", new {id = catId});
        }

        /// <summary>
        /// Обрабатывает перемещение указанных товаров между пользовательскими категориями
        /// </summary>
        /// <param name="selectedIds"></param>
        /// <param name="newCategoryId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("account/move-products")]
        public ActionResult MoveProducts(string selectedIds, int newCategoryId)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Загружаем продукты
            var products =
                selectedIds.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(s => Convert.ToInt32(s)).
                    Select(i => CurrentUser.Products.FirstOrDefault(p => p.Id == i)).Where(p => p != null).ToList();

            var targetCategory = CurrentUser.UserCategories.FirstOrDefault(c => c.Id == newCategoryId);
            if (products.Count > 0 && targetCategory != null)
            {
                foreach(var prod in products)
                {
                    prod.UserCategory = targetCategory;
                }
                UsersRepository.SubmitChanges();
            }

            return RedirectToAction("CategoryProducts", new {id = newCategoryId});
        }

        #endregion

        #endregion

        #region Тендеры

        /// <summary>
        /// Отображает список тендеров и заявки на участие в тендерах если они есть
        /// </summary>
        /// <returns></returns>
        [Route("account/tenders")]
        public ActionResult Tenders()
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Личный кабинет", "/account", false);
            PushNavigationChainItem("Тендеры", "/account/tenders", true);

            return View();
        }

        /// <summary>
        /// Отображает страницу создания нового тендера
        /// </summary>
        /// <returns></returns>
        [Route("account/tenders/add-tender")]
        public ActionResult AddTender()
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Личный кабинет", "/account", false);
            PushNavigationChainItem("Тендеры", "/account/tenders", false);
            PushNavigationChainItem("Новый тендер", "", true);

            return View("EditTender", new Tender());
        }

        /// <summary>
        /// Обрабатывает создание или сохранение нового тендера
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("account/tenders/save-tender")]
        public ActionResult SaveTender(Tender model)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Сохраняем или создание
            Tender tender = null;
            if (model.Id <= 0)
            {
                tender = model;
                tender.DateCreated = DateTime.Now;

                // TODO: добавить сохранение фотографий по тендерам

                CurrentUser.Tenders.Add(tender);
            }
            else
            {
                tender = CurrentUser.Tenders.First(t => t.Id == model.Id);
                tender.Title = model.Title;
                tender.Keywords = model.Keywords;
                tender.Description = model.Description;
                tender.MinPrice = model.MinPrice;
                tender.MaxPrice = model.MaxPrice;
                tender.Category = Locator.GetService<ICategoriesRepository>().Load(model.CategoryId);
                tender.Measure = model.Measure;
                tender.Size = model.Size;
                tender.Currency = model.Currency;
                tender.DateStart = model.DateStart;
                tender.DateEnd = model.DateEnd;
            }
            UsersRepository.SubmitChanges();

            // Сохраняем фото к тендеру
            var tenderImageFile = Request.Files["TenderImage"];
            if (tenderImageFile != null && tenderImageFile.ContentLength > 0 && tenderImageFile.ContentType.Contains("image"))
            {
                var fileName = String.Format("tender-{0}-{1}{2}", tender.Id,
                                             new Random(System.Environment.TickCount).Next(Int32.MaxValue),
                                             Path.GetExtension(tenderImageFile.FileName));
                FileUtils.SavePostedFile(tenderImageFile, "tenderimage", fileName);
                tender.Image = fileName;
                UsersRepository.SubmitChanges();
            }

            // Перенаправляемся на список тендеров
            return RedirectToAction("Tenders");
        }

        /// <summary>
        /// Отображает страницу редактирования тендера
        /// </summary>
        /// <param name="id">Идентификатор тендера</param>
        /// <returns></returns>
        [Route("account/tenders/edit/{id}")]
        public ActionResult EditTender(long id)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Ищем тендер и открываем его
            var tender = CurrentUser.Tenders.FirstOrDefault(p => p.Id == id);
            if (tender == null)
            {
                return RedirectToAction("Tenders");
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Личный кабинет", "", false);
            PushNavigationChainItem("Тендеры", "/account/tenders", false);
            PushNavigationChainItem(string.Format("Редактирование тендера \"{0}\"", tender.Title), "", true);

            // Вид
            return View("EditTender", tender);
        }

        /// <summary>
        /// Обрабатывает удаление указанного тендера
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("account/tenders/delete/{id}")]
        public ActionResult DeleteTender(long id)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Ищем тендер и открываем его
            var tender = CurrentUser.Tenders.FirstOrDefault(p => p.Id == id);
            if (tender == null)
            {
                return RedirectToAction("Tenders");
            }

            // Удаляем тендер
            Locator.GetService<ITendersRepository>().Delete(tender);
            UsersRepository.SubmitChanges();

            // Переходим в запомненную категорию
            return RedirectToAction("Tenders");
        }

        /// <summary>
        /// Отображает страницу с указанным тендером
        /// </summary>
        /// <param name="id">Идентификатор тендера</param>
        /// <returns></returns>
        [Route("tenders/tender/{id}")]
        public ActionResult ViewTender(long id)
        {
            // Ищем 
            var rep = Locator.GetService<ITendersRepository>();
            var tender = rep.Load(id);
            if (tender == null)
            {
                return RedirectToAction("Tenders");
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Тендеры", "/tenders", false);
            PushNavigationChainItem(tender.Category.Title, "/tenders/category/"+tender.CategoryId, false);
            PushNavigationChainItem(tender.Title, "", true);

            return View(tender);
        }

        /// <summary>
        /// Обрабатывает заявку на тендер
        /// </summary>
        /// <param name="model">Модель данных заявки по тендеру</param>
        /// <returns></returns>
        [HttpPost][Route("tenders/participate-tender")]
        public ActionResult ParticipateTender(TenderOffer model)
        {
            // Проверяем авторизованность
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Ищем тендер
            var tender = Locator.GetService<ITendersRepository>().Load(model.TenderId);
            if (tender == null)
            {
                return RedirectToAction("Tenders","Main");
            }

            // Добавляем заявку по тендеру
            model.User = CurrentUser;
            model.Tender = tender;
            model.DateCreated = DateTime.Now;
            tender.TenderOffers.Add(model);
            UsersRepository.SubmitChanges();
            
            // перенаправляемся обратно на тендер
            return RedirectToAction("ViewTender", new {id = model.TenderId});
        }

        /// <summary>
        /// Отображает список всех заявок на участие в тендерах, которые вы отправляли
        /// </summary>
        /// <returns></returns>
        [Route("account/tenders/participation")]
        public ActionResult TendersParticipation()
        {
            // Проверяем авторизованность
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Личный кабинет", "", false);
            PushNavigationChainItem("Тендеры", "/account/tenders", false);
            PushNavigationChainItem("Участие в тендерах", "", true);

            return View();
        }

        /// <summary>
        /// Отменяет вашу заявку на участие в тендоре
        /// </summary>
        /// <param name="id">Идентификатор заявки</param>
        /// <returns></returns>
        [Route("account/tenders/delete-request/{id}")]
        public ActionResult DeleteTenderRequest(long id)
        {
            // Проверяем авторизованность
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Ищем заявку
            var offer = CurrentUser.TenderOffers.FirstOrDefault(o => o.Id == id);
            if (offer == null)
            {
                return RedirectToAction("TendersParticipation");
            }

            // удаляем
            CurrentUser.TenderOffers.Remove(offer);
            offer.Tender.TenderOffers.Remove(offer);
            UsersRepository.SubmitChanges();

            return RedirectToAction("TendersParticipation");
        }

        #endregion

        #region Изменение пароля

        /// <summary>
        /// Отображает страницу с формой изменения пароля
        /// </summary>
        /// <returns></returns>
        [Route("account/change-password")]
        public ActionResult ChangePassword()
        {
            // Проверяем авторизованность
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Личный кабинет", "", false);
            PushNavigationChainItem("Изменение пароля", "/account/change-password", false);

            // Отображаем вид
            return View();
        }

        /// <summary>
        /// Обрабатывает изменение пароля пользователем
        /// </summary>
        /// <param name="model">Модель</param>
        /// <returns></returns>
        [HttpPost][Route("account/do-change-password")]
        public ActionResult DoChangePassword(ChangePasswordModel model)
        {
            // Проверяем авторизованность
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Сверяем пароль
            if (CurrentUser.PasswordHash != PasswordUtils.QuickMD5(model.OldPassword))
            {
                ModelState.AddModelError("Password","Неправильный пароль");
                return View("ChangePassword");
            }

            // Меняем
            CurrentUser.PasswordHash = PasswordUtils.QuickMD5(model.NewPassword);
            UsersRepository.SubmitChanges();

            return View("ChangePasswordSuccess");
        }

        #endregion

    }
}
