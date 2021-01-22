using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WFHMicrosite.Models;

namespace WFHMicrosite.Controllers
{
    public class HomeController : Controller
    {
        readonly IConfiguration configuration;

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string apiUrl = configuration.GetValue<string>("AppSettings:ApiUrl");
            RegisterModel model = new RegisterModel() { SiteId = (int)id };
            string json = SendWebApiMessage(apiUrl + "sites/" + id, "GET", "");
            if (!string.IsNullOrEmpty(json))
            {
                SiteModel data = JsonConvert.DeserializeObject<SiteModel>(json);
                json = SendWebApiMessage(apiUrl + "products/" + id, "GET", "");
                List<ProductModel> products = JsonConvert.DeserializeObject<List<ProductModel>>(json);
                model.ProductId = products.Where(x => x.Default).Select(y => y.ProductId).FirstOrDefault();
                model.Language = data.Language;
                ViewBag.Logo = data.LogoImage;
                ViewBag.Logo2 = data.LogoImage2;
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(RegisterModel model)
        {
            string apiUrl = configuration.GetValue<string>("AppSettings:ApiUrl");
            string json;
            if (ModelState.IsValid)
            {
                UserModel user = new UserModel() { ProductId = model.ProductId, EmailAddress = model.EmailAddress, Language = model.Language, Quantity = 1 };
                json = JsonConvert.SerializeObject(user);
                json = SendWebApiMessage(apiUrl + "users", "POST", json);
                if (!string.IsNullOrEmpty(json))
                {
                    if (model.Language == "English")
                    {
                        ViewBag.Results = "Register successful - check your email for the login link and your PIN";
                    }
                    else
                    {
                        ViewBag.Results = "Inscription réussie - vérifiez votre courrier électronique pour le lien de connexion et votre code PIN";
                    }
                }
                else
                {
                    if (model.Language == "English")
                    {
                        ViewBag.Results = "Register failed - resubmit you email address";
                    }
                    else
                    {
                        ViewBag.Results = "Inscription échouée - soumettez à nouveau votre adresse e-mail";
                    }
                }
            }
            model.EmailAddress = "";
            json = SendWebApiMessage(apiUrl + "sites/" + model.SiteId, "GET", "");
            if (!string.IsNullOrEmpty(json))
            {
                SiteModel data = JsonConvert.DeserializeObject<SiteModel>(json);
                ViewBag.Logo = data.LogoImage;
                ViewBag.Logo2 = data.LogoImage2;
            }
            return View(model);
        }

        public IActionResult Login(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string apiUrl = configuration.GetValue<string>("AppSettings:ApiUrl");
            LoginModel model = new LoginModel() { SiteId = (int)id };
            string json = SendWebApiMessage(apiUrl + "sites/" + id, "GET", "");
            if (!string.IsNullOrEmpty(json))
            {
                SiteModel data = JsonConvert.DeserializeObject<SiteModel>(json);
                json = SendWebApiMessage(apiUrl + "products/" + id, "GET", "");
                List<ProductModel> products = JsonConvert.DeserializeObject<List<ProductModel>>(json);
                model.ProductId = products.Where(x => x.Default).Select(y => y.ProductId).FirstOrDefault();
                model.Language = data.Language;
                ViewBag.Logo = data.LogoImage;
                ViewBag.Logo2 = data.LogoImage2;
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            string apiUrl = configuration.GetValue<string>("AppSettings:ApiUrl");
            string json;
            if (ModelState.IsValid)
            {
                json = JsonConvert.SerializeObject(model);
                json = SendWebApiMessage(apiUrl + "login", "POST", json);
                if (!string.IsNullOrEmpty(json))
                {
                    LoginModel data = JsonConvert.DeserializeObject<LoginModel>(json);
                    return RedirectToAction("Index", "Product", new { id = data.UserId, siteid = data.SiteId });
                }
                if (model.Language == "English")
                {
                    ViewBag.ErrorResults = "Log in failed - check your email and PIN";
                }
                else
                {
                    ViewBag.ErrorResults = "Inscription échouée - soumettez à nouveau votre adresse e-mail";
                }
            }
            json = SendWebApiMessage(apiUrl + "sites/" + model.SiteId, "GET", "");
            if (!string.IsNullOrEmpty(json))
            {
                SiteModel data = JsonConvert.DeserializeObject<SiteModel>(json);
                ViewBag.Logo = data.LogoImage;
                ViewBag.Logo2 = data.LogoImage2;
            }
            return View(model);
        }

        private string SendWebApiMessage(string url, string method, string json)
        {
            string data = "";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Proxy = null;
            request.ContentType = "application/json";
            request.Method = method;
            if (json != "")
            {
                StreamWriter stream = new StreamWriter(request.GetRequestStream());
                stream.Write(json);
                stream.Flush();
            }
            try
            {
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                data = reader.ReadToEnd();
            }
            catch { }
            return data;
        }
    }
}