using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartel.Domain.Entities;
using Kartel.Domain.Enums;
using Kartel.Domain.Infrastructure.Mailing.Templates;
using Kartel.Domain.Infrastructure.Misc;
using Kartel.Domain.Infrastructure.Routing;
using Kartel.Domain.Interfaces.Infrastructure;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;
using Kartel.Trade.Web.Areas.ControlPanel.Models;
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

        #region Восстановление забытого пароля
        /// <summary>
        /// Страница восстановления пароля
        /// </summary>
        public ActionResult ForgotPassword()
        {
            if (IsAuthentificated)
            {
                return RedirectToAction("Index", "Main");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            if (IsAuthentificated)
            {
                return RedirectToAction("Index", "Main");
            }

            var userRepository = Locator.GetService<IUsersRepository>();
            var user = userRepository.Find(f => f.Email == email);
            if (user == null)
            {
                TempData["Error"] = "Пользователь с таким логином (email) не найден.";
                return View("ForgotPassword");
            }

            user.PasswordHash = PasswordUtils.Hashify(user.PasswordHash, 5).ToLower();

            const string subject = "Восстановление пароля";
            var template =
                new ParametrizedFileTemplate(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Mail", "ResetPassword.html"), new
                        {
                            Subject = subject,
                            Content = Url.Action("ResetPassword", null, 
                            new { email = user.Email, reset =  user.PasswordHash }, Request.Url.Scheme)
                        });

            Locator.GetService<IMailNotificationManager>().Notify(user, subject, template.ToString());
            userRepository.SubmitChanges();
            TempData["Message"] = "Инструкция по восстановлению пароля отправлена на e-mail, указанный при регистрации";

            return View();
        }

        public ActionResult ResetPassword(string email, string reset)
        {
            if (IsAuthentificated)
            {
                return RedirectToAction("Index", "Main");
            }

            var repository = Locator.GetService<IUsersRepository>();
            var user = repository.GetUserByLoginAndPasswordHash(email, reset);
            if (user == null)
            {
                TempData["Error"] = "Введён неправильный код восстановления";
                return View("ForgotPassword");
            }
            return View("ResetPassword", user);
        }

        [HttpPost]
        public ActionResult DoResetPassword(string password, string confirm, string email)
        {
            if (IsAuthentificated)
            {
                return RedirectToAction("Index", "Main");
            }

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirm))
            {
                TempData["Error"] = "Поля должны быть заполнены";
                return View("ResetPassword");
            }

            if (password != confirm)
            {
                TempData["Error"] = "Пароли не совпадают";
                return View("ResetPassword");
            }

            if(string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Пользователь не найден";
                return View("ResetPassword");
            }

            var repository = Locator.GetService<IUsersRepository>();
            var user = repository.Find(f => f.Email == email);
            if (user == null)
            {
                TempData["Error"] = "Пользователь не найден";
                return View("ResetPassword");
            }

            user.PasswordHash = PasswordUtils.QuickMD5(password);
            TempData["Message"] = "Новый пароль установлен. Заполните форму, чтобы авторизоваться";
            repository.SubmitChanges();
            return RedirectToAction("Login");
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
            UserCategory category;
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
        /// Обрабатывает категории - создает или сохраняет новую пользовательскую категорию
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveCategoryJson(string name)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }
            

            if (!string.IsNullOrEmpty(name))
            {
                var category = new UserCategory
                    {
                        Title = name,
                        Position = CurrentUser.UserCategories.Count > 0
                                        ? CurrentUser.UserCategories.Max(c => c.Position) + 1000
                                        : 1000
                    };
                CurrentUser.UserCategories.Add(category);
            }

            // Сохраняем
            UsersRepository.SubmitChanges();

            // Переходим на страницу товара
            return Json(new {success = true},JsonRequestBehavior.AllowGet);
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

        /// <summary>
        /// Перемещает категорию вверх на одну позицию
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("account/products/move-category-up/{id}")]
        public ActionResult MoveCategoryUp(long id)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }
            var userCategories = CurrentUser.UserCategories.OrderBy(f => f.Position).ToList();

            var userCategory = userCategories.FirstOrDefault(c => c.Id == id);
            if (userCategory == null)
            {
                return RedirectToAction("Products");
            }

            var currentCategoryPos = userCategory.Position;

            if (currentCategoryPos != 0)
            {
                for (int i = 0; i < userCategories.Count; i++)
                {
                    if (userCategories[i] == userCategory)
                    {
                        var prevCategory = userCategories[i - 1];
                        userCategory.Position = prevCategory.Position;
                        prevCategory.Position = currentCategoryPos;
                    }
                }

                UsersRepository.SubmitChanges();
                return RedirectToAction("Products");
            }

            return RedirectToAction("Products");
        }

        /// <summary>
        /// Перемещает категорию вниз на одну позицию
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("account/products/move-category-down/{id}")]
        public ActionResult MoveCategoryDown(long id)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }
            var userCategories = CurrentUser.UserCategories.OrderBy(f => f.Position).ToList();

            var userCategory = userCategories.FirstOrDefault(c => c.Id == id);
            if (userCategory == null)
            {
                return RedirectToAction("Products");
            }

            var currentCategoryPos = userCategory.Position;

            if (currentCategoryPos != userCategories.Last().Position)
            {
                for (int i = 0; i < userCategories.Count; i++)
                {
                    if (userCategories[i] == userCategory)
                    {
                        var nextCategory = userCategories[i + 1];
                        userCategory.Position = nextCategory.Position;
                        nextCategory.Position = currentCategoryPos;
                    }
                }

                UsersRepository.SubmitChanges();
                return RedirectToAction("Products");
            }

            return RedirectToAction("Products");
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
            return View("EditProduct", new Product(){User = CurrentUser});
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
        public ActionResult SaveProduct(Product model, HttpPostedFileBase[] images, string deletedImages)
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

            var imagesRepository = Locator.GetService<IProductImagesRepository>();

            if (images.Any())
            {
                

                foreach (var image in images)
                {
                    if (image != null && image.ContentLength > 0 && image.ContentType.Contains("image"))
                    {
                        var name = String.Format("prod-{0}-{1}{2}", product.Id,
                                             new Random(Environment.TickCount).Next(Int32.MaxValue),
                                             Path.GetExtension(image.FileName));

                        FileUtils.SavePostedFile(image, "prodimage", name);

                        imagesRepository.Add(new ProductImage
                            {
                                Image = name,
                                Product = product,
                                ProductId = product.Id
                            });
                    }
                }

                imagesRepository.SubmitChanges();
            }

            if (!string.IsNullOrEmpty(deletedImages))
            {
                int[] imageIds = deletedImages.Split(',').Select(n => Convert.ToInt32(n)).ToArray();

                if (imageIds.Any())
                {
                    foreach (var imageId in imageIds)
                    {
                        var image = imagesRepository.Find(f => f.Id == imageId);
                        if (image != null)
                        {
                            imagesRepository.Delete(image);
                        }
                    }

                    imagesRepository.SubmitChanges();
                }
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

        #region Интеграция с UniSender

        /// <summary>
        /// Обрабатывает редирект либо регистрацию пользователя в системе unisender
        /// </summary>
        /// <returns></returns>
        [Route("account/sending")]
        public ActionResult UniSender()
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Переводим на юнисендер
            if (CurrentUser.UniSenderActivated)
            {
                return View("UniSenderIFrame");
            }
            else
            {
                // Навигационная цепочка
                PushNavigationChainItem("Главная страница", "/");
                PushNavigationChainItem("Личный кабинет", "", false);
                PushNavigationChainItem("Регистрация в UniSender", "/account/sending", true);

                return View("UniSenderRegistration");
            }
        }

        /// <summary>
        /// Обрабатывает запрос пользователя на регистрацию в системе UniSender
        /// </summary>
        /// <returns></returns>
        [Route("account/sending/registration")]
        public ActionResult UniSenderRegistration()
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            if (CurrentUser.UniSenderActivated)
            {
                return RedirectToAction("UniSender");
            }

            // Менеджер
            var api = Locator.GetService<IUniSenderAPI>();
            
            // Выполняем регистрацию
            var login = CurrentUser.Email.GetEmailLogin() + "_kartel";
            var successfull = api.RegisterUser(CurrentUser.Email, login);
            if (successfull)
            {
                // Навигационная цепочка
                PushNavigationChainItem("Главная страница", "/");
                PushNavigationChainItem("Личный кабинет", "", false);
                PushNavigationChainItem("Успешная регистрация в UniSender", "/account/sending", true);

                CurrentUser.UniSenderActivated = true;
                UsersRepository.SubmitChanges();

                return View("UniSenderRegistrationSuccess");
            }
            else
            {
                throw new Exception("Shit happens");
            }
        }

        #endregion

        #region Золотой поставщик

        [Route("account/gold")]
        public ActionResult GoldVendor()
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Личный кабинет", "", false);
            PushNavigationChainItem("Золотой поставщик", "", true);

            // Отображаем вид
            return View();
        }

        /// <summary>
        /// Создает и отображает счет для юридического лица за золотого поставщика
        /// </summary>
        /// <returns></returns>
        [Route("account/gold/bill-legal")][HttpPost]
        public ActionResult GoldVendorLegalBill(long period, string companyName)
        {
            // Репозиторий
            var rep = Locator.GetService<IBillsRepository>();
            var settingsRep = Locator.GetService<ISettingsRepository>();
            var price = settingsRep.GetValue<decimal>("gold" + period);

            // Создаем и отображаем счет
            var bill = new Bill()
                {
                    User = CurrentUser,
                    ActivationTarget = "tariff",
                    ActivationAmount = (int) period,
                    DateCreated = DateTime.Now,
                    Amount = price
                };
            rep.Add(bill);
            rep.SubmitChanges();

            ViewBag.company = companyName;
            ViewBag.price = price;
            ViewBag.period = period;
            return View(bill);
        }

        /// <summary>
        /// Создает и отображает счет для физического лица за услугу золотого поставщика
        /// </summary>
        /// <returns></returns>
        [Route("account/gold/bill-phys")]
        [HttpPost]
        public ActionResult GoldVendorPhysBill(long period, string fio, string address)
        {
            // Репозиторий
            var rep = Locator.GetService<IBillsRepository>();
            var settingsRep = Locator.GetService<ISettingsRepository>();
            var price = settingsRep.GetValue<decimal>("gold" + period);

            // Создаем и отображаем счет
            var bill = new Bill()
            {
                User = CurrentUser,
                ActivationTarget = "tariff",
                ActivationAmount = (int)period,
                DateCreated = DateTime.Now,
                Amount = price
            };
            rep.Add(bill);
            rep.SubmitChanges();

            ViewBag.fio = fio;
            ViewBag.address = address;
            ViewBag.price = price;
            ViewBag.period = period;
            return View(bill);
        }

        #endregion

        #region Auto completes

        /// <summary>
        /// Возвращает автокомплит стран
        /// </summary>
        /// <param name="term">Строка для поиска</param>
        /// <returns></returns>
        public ActionResult CountriesAutoComplete(string term)
        {
            // Репозиторий
            var rep = Locator.GetService<ICountriesRepository>();
            var countries = rep.GetAllCountries().Where(c => c.Name.ToLower().Contains(term.ToLower())).Select(c => c.Name);
            return Json(countries,JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Выбор дизайна

        /// <summary>
        /// Отображает сайт пользователя с компонентом выбора дизайна
        /// </summary>
        /// <returns></returns>
        public ActionResult Design()
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            return Redirect(string.Format("/vendor/{0}?selectDesign=1", CurrentUser.Id));
        }

        /// <summary>
        /// Обрабатывает сохранение выбранного дизайна пользователя
        /// </summary>
        /// <param name="design">Выбранный дизайн</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveDesign(string design)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Сохраняем дизайн
            CurrentUser.Design = design;
            // TODO: сделать проверку премиальности дизайнов
            UsersRepository.SubmitChanges();

            // Перенаправляемся на сайт пользователя
            return RedirectToAction("Index", "UserSite", new {id = CurrentUser.Id});
        }

        #endregion

        #region Горячие товары

        /// <summary>
        /// Отображает страницу горячих товаров
        /// </summary>
        /// <returns></returns>
        [Route("account/hot-products")]
        public ActionResult HotProducts()
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Личный кабинет", "/account", false);
            PushNavigationChainItem("Горячие товары", "/account/hot-products", true);

            // Выбираем горячие товары
            var hotProducts = from product in CurrentUser.Products
                              select product.HotProducts;

            // Отображаем вид
            return View(hotProducts.ToList());
        }

        /// <summary>
        /// Создает и отображает счет для юридического лица за золотого поставщика
        /// </summary>
        /// <returns></returns>
        [Route("account/hot-products/bill-legal")]
        [HttpPost]
        public ActionResult HotProductsLegalBill(long shows, string companyName)
        {
            // Репозиторий
            var rep = Locator.GetService<IBillsRepository>();
            double price = 30.0/1000.0*shows;

            // Создаем и отображаем счет
            var bill = new Bill()
            {
                User = CurrentUser,
                ActivationTarget = "hot-products",
                ActivationAmount = (int)shows,
                DateCreated = DateTime.Now,
                Amount = (decimal) price
            };
            rep.Add(bill);
            rep.SubmitChanges();

            ViewBag.company = companyName;
            ViewBag.price = price;
            ViewBag.shows = shows;
            return View(bill);
        }

        /// <summary>
        /// Создает и отображает счет для физического лица за услугу золотого поставщика
        /// </summary>
        /// <returns></returns>
        [Route("account/hot-products/bill-phys")]
        [HttpPost]
        public ActionResult HotProductsPhysBill(long shows, string fio, string address)
        {
            // Репозиторий
            var rep = Locator.GetService<IBillsRepository>();
            double price = 30.0 / 1000.0 * shows;

            // Создаем и отображаем счет
            var bill = new Bill()
            {
                User = CurrentUser,
                ActivationTarget = "hot-products",
                ActivationAmount = (int)shows,
                DateCreated = DateTime.Now,
                Amount = (decimal) price
            };
            rep.Add(bill);
            rep.SubmitChanges();

            ViewBag.fio = fio;
            ViewBag.address = address;
            ViewBag.price = price;
            ViewBag.shows = shows;
            return View(bill);
        }

        /// <summary>
        /// Отключает горячие товары из блока показа
        /// </summary>
        /// <param name="ids">Идентификаторы товара, разделенные запятой</param>
        /// <returns></returns>
        public ActionResult HotProductsUnhot(string ids)
        {
            // Репозиторий
            var rep = Locator.GetService<IProductsRepository>();
            var products =
                ids.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(c => Convert.ToInt32(c)).Select(c => rep.Load(c)).Where(p => p != null && p.HotProducts != null && CurrentUser.Products.Any(pr => pr.Id == p.Id)).ToList();

            // Отключаем показ
            if (products.Count > 0)
            {
                products.ForEach(p => p.HotProducts.EnableHotProduct = false);
            }
            rep.SubmitChanges();

            return RedirectToAction("HotProducts");
        }

        /// <summary>
        /// Делает указанные товары горячими товарами т.е. отображает их в блоке горячих товаров
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult HotProductsMakeHot(string ids)
        {
            // Репозиторий
            var rep = Locator.GetService<IProductsRepository>();
            var products =
                ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(c => Convert.ToInt32(c)).Select(c => rep.Load(c)).Where(p => p != null && CurrentUser.Products.Any(pr => pr.Id == p.Id)).ToList();

            // Включаем отображение
            if (products.Count > 0)
            {
                foreach(var product in products)
                {
                    if (product.HotProducts == null)
                    {
                        product.HotProducts = new HotProduct()
                            {
                                Product = product,
                                Clicks = 0,
                                Views = 0,
                                PayedViews = 0,
                                EnableHotProduct = false
                            };
                    }
                    product.HotProducts.EnableHotProduct = true;
                }
                rep.SubmitChanges();
            }

            // Отображаем страницу горячих товаров
            return RedirectToAction("HotProducts");
        }

        #endregion

        #region Баннер пользовательского сайта

        /// <summary>
        /// Отображает страницу редактирования баннера на сайт
        /// </summary>
        /// <returns></returns>
        public ActionResult Banner()
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Проверяем что у пользователя есть баннер
            if (CurrentUser.Banner == null)
            {
                CurrentUser.Banner = new UserBanner()
                    {
                        User = CurrentUser
                    };
                UsersRepository.SubmitChanges();
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Личный кабинет", "", false);
            PushNavigationChainItem("Баннер на главную страницу сайта", "", true);

            // Отдаем
            return View(CurrentUser.Banner);
        }

        /// <summary>
        /// Обрабатывает сохранение баннера
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveBanner(UserBanner banner)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Проверяем что у пользователя есть баннер
            if (CurrentUser.Banner == null)
            {
                CurrentUser.Banner = new UserBanner()
                {
                    User = CurrentUser
                };
                UsersRepository.SubmitChanges();
            }

            // Сохраняем
            CurrentUser.Banner.Color = banner.Color;
            CurrentUser.Banner.Size = banner.Size;
            CurrentUser.Banner.Position = banner.Position;
            CurrentUser.Banner.Text = banner.Text;
            CurrentUser.Banner.Height = banner.Height;
            CurrentUser.Banner.Enabled = banner.Enabled;

            // Сохраняем файл
            var image = Request.Files["bg"];
            if (image != null && image.ContentLength > 0 && image.ContentType.Contains("image"))
            {
                var filename = String.Format("{0}-{1}{2}", Path.GetFileNameWithoutExtension(image.FileName),
                                             new Random(System.Environment.TickCount).Next(Int32.MaxValue), Path.GetExtension(image.FileName));
                FileUtils.SavePostedFile(image,"banner",filename);
                CurrentUser.Banner.ImageUrl = filename;
            }

            UsersRepository.SubmitChanges();

            // Возвращаем обратно на страницу редактирования
            return RedirectToAction("Banner");
        }

        #endregion
    }
}
