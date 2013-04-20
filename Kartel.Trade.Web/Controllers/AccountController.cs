using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartel.Domain.Entities;
using Kartel.Domain.Infrastructure.Misc;
using Kartel.Domain.Infrastructure.Routing;
using Kartel.Domain.Interfaces.Repositories;
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
            UsersRepository.Add(newUser);
            UsersRepository.SubmitChanges();
            // TODO: добавить авторизацию пользователя

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
        public ActionResult SaveCompanyProfile(User model)
        {
            if (!IsAuthentificated)
            {
                return RedirectToAction("Register");
            }

            // Сохраняем
            // Основное инфо
            CurrentUser.Company = model.Company;
            CurrentUser.Brand = model.Brand;
            // TODO: доабвить сохранение лого компании
            CurrentUser.FIO = model.FIO;
            // TODO: добавить сохранение портрета пользователя
            // TODO: добавить сохранение номером телефона
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

            // Сохраняем
            UsersRepository.SubmitChanges();
            return View("ProfileSaved");
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

        #endregion

    }
}
