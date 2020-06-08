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
                id = 1;
                //return NotFound();
            }

            string apiUrl = configuration.GetValue<string>("AppSettings:ApiUrl");
            LoginModel model = new LoginModel() { ProductId = (int)id };
            string json = SendWebApiMessage(apiUrl + "products/" + id, "GET", "");
            if (!string.IsNullOrEmpty(json))
            {
                ProductModel data = JsonConvert.DeserializeObject<ProductModel>(json);
                ViewBag.Logo = data.LogoImage;
                ViewBag.Logo2 = data.LogoImage2;
                ViewBag.Name = data.Name;
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(LoginModel model)
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
                    return RedirectToAction("Index", "Product", new { id = data.UserId });
                }
                ViewBag.ErrorResults = "Log in failed - check your email and PIN";
            }
            json = SendWebApiMessage(apiUrl + "products/" + model.ProductId, "GET", "");
            if (!string.IsNullOrEmpty(json))
            {
                ProductModel data = JsonConvert.DeserializeObject<ProductModel>(json);
                ViewBag.Logo = data.LogoImage;
                ViewBag.Logo2 = data.LogoImage2;
                ViewBag.Name = data.Name;
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