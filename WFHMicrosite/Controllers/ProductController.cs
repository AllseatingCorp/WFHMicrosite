using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Maps.Geocoding.Place;
using WFHMicrosite.Models;
using GoogleApi.Entities.Places.Search.Text.Request;
using GoogleApi;
using GoogleApi.Entities.Places.Search.Text.Response;

namespace WFHMicrosite.Controllers
{
    public class ProductController : Controller
    {
        readonly IConfiguration configuration;

        public ProductController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Index(int id)
        {
            string apiUrl = configuration.GetValue<string>("AppSettings:ApiUrl");
            UserProductModel data = new UserProductModel();
            string json = SendWebApiMessage(apiUrl + "users/" + id, "GET", "");
            data.User = JsonConvert.DeserializeObject<UserModel>(json);
            json = SendWebApiMessage(apiUrl + "userselections/" + id, "GET", "");
            data.UserSelections = JsonConvert.DeserializeObject<List<UserSelectionModel>>(json);
            json = SendWebApiMessage(apiUrl + "products/" + data.User.ProductId, "GET", "");
            data.Product = JsonConvert.DeserializeObject<ProductModel>(json);
            json = SendWebApiMessage(apiUrl + "productoptions/" + data.User.ProductId + "/Fabric", "GET", "");
            data.FabricOptions = JsonConvert.DeserializeObject<List<ProductOptionModel>>(json);
            json = SendWebApiMessage(apiUrl + "productoptions/" + data.User.ProductId + "/Mesh", "GET", "");
            data.MeshOptions = JsonConvert.DeserializeObject<List<ProductOptionModel>>(json);
            json = SendWebApiMessage(apiUrl + "productoptions/" + data.User.ProductId + "/Frame", "GET", "");
            data.FrameOptions = JsonConvert.DeserializeObject<List<ProductOptionModel>>(json);
            foreach (var item in data.UserSelections)
            {
                if (item.Type == "Fabric")
                {
                    data.FabricId = item.ProductOptionId;
                    foreach (var fabric in data.FabricOptions)
                    {
                        if (fabric.ProductOptionId == data.FabricId)
                            fabric.Default = true;
                        else
                            fabric.Default = false;
                    }
                }
                if (item.Type == "Mesh")
                {
                    data.MeshId = item.ProductOptionId;
                    foreach (var mesh in data.MeshOptions)
                    {
                        if (mesh.ProductOptionId == data.MeshId)
                            mesh.Default = true;
                        else
                            mesh.Default = false;
                    }
                }
                if (item.Type == "Frame")
                {
                    data.FrameId = item.ProductOptionId;
                    foreach (var frame in data.FrameOptions)
                    {
                        if (frame.ProductOptionId == data.FrameId)
                            frame.Default = true;
                        else
                            frame.Default = false;
                    }
                }
            }
            json = SendWebApiMessage(apiUrl + "productimages/" + data.User.ProductId + "/" + data.FabricId + "/" + data.MeshId + "/" + data.FrameId, "GET", "");
            try { data.ProductImage = JsonConvert.DeserializeObject<ProductImageModel>(json); }
            catch { data.ProductImage = null; }

            if (data.User.InProduction != null)
            {
                return RedirectToAction("Complete", new { id = data.User.UserId });
            }
            if (data.Product.InstallGuide == null) data.Product.InstallGuide = "";
            if (data.Product.UserGuide == null) data.Product.UserGuide = "";
            if (data.Product.VideoUrl == null) data.Product.VideoUrl = "";
            ViewBag.Logo = data.Product.LogoImage;
            ViewBag.Logo2 = data.Product.LogoImage2;
            ViewBag.Name = data.Product.Name;
            return View(data);
        }

        [HttpPost]
        public IActionResult Index(UserProductModel data)
        {
            data.User.PhoneNumber = new string(data.User.PhoneNumber.Where(char.IsDigit).ToArray());
            string apiUrl = configuration.GetValue<string>("AppSettings:ApiUrl");
            bool update = false;
            string json = SendWebApiMessage(apiUrl + "users/" + data.User.UserId, "GET", "");
            UserModel user = JsonConvert.DeserializeObject<UserModel>(json);
            if (data.User.Address1 != user.Address1) update = true;
            if (data.User.Address2 != user.Address2) update = true;
            if (data.User.AttnName != user.AttnName) update = true;
            if (data.User.City != user.City) update = true;
            if (data.User.Commercial != user.Commercial) update = true;
            if (data.User.Country != user.Country) update = true;
            if (data.User.EmailAddress != user.EmailAddress) update = true;
            if (data.User.PhoneNumber != user.PhoneNumber) update = true;
            if (data.User.PostalZip != user.PostalZip) update = true;
            if (data.User.ProvinceState != user.ProvinceState) update = true;
            if (data.User.SpecialInstructions != user.SpecialInstructions) update = true;
            json = SendWebApiMessage(apiUrl + "userselections/" + data.User.UserId, "GET", "");
            List<UserSelectionModel> userSelections = JsonConvert.DeserializeObject<List<UserSelectionModel>>(json);
            foreach (var item in userSelections)
            {
                if (item.Type == "Fabric" && item.ProductOptionId != data.FabricId)
                {
                    item.ProductOptionId = data.FabricId;
                    update = true;
                }
                if (item.Type == "Mesh" && item.ProductOptionId != data.MeshId)
                {
                    item.ProductOptionId = data.MeshId;
                    update = true;
                }
                if (item.Type == "Frame" && item.ProductOptionId != data.FrameId)
                {
                    item.ProductOptionId = data.FrameId;
                    update = true;
                }
            }

            if (update)
            {
                data.User.Completed = DateTime.Now;
                foreach (var item in userSelections)
                {
                    json = JsonConvert.SerializeObject(item);
                    SendWebApiMessage(apiUrl + "userselections/" + item.UserSelectionId, "PUT", json);
                }
            }

            json = JsonConvert.SerializeObject(data.User);
            SendWebApiMessage(apiUrl + "users/" + data.User.UserId, "PUT", json);

            json = SendWebApiMessage(apiUrl + "products/" + data.User.ProductId, "GET", "");
            data.Product = JsonConvert.DeserializeObject<ProductModel>(json);
            json = SendWebApiMessage(apiUrl + "productoptions/" + data.User.ProductId + "/Fabric", "GET", "");
            data.FabricOptions = JsonConvert.DeserializeObject<List<ProductOptionModel>>(json);
            json = SendWebApiMessage(apiUrl + "productoptions/" + data.User.ProductId + "/Mesh", "GET", "");
            data.MeshOptions = JsonConvert.DeserializeObject<List<ProductOptionModel>>(json);
            json = SendWebApiMessage(apiUrl + "productoptions/" + data.User.ProductId + "/Frame", "GET", "");
            data.FrameOptions = JsonConvert.DeserializeObject<List<ProductOptionModel>>(json);
            data.UserSelections = userSelections;
            foreach (var item in data.UserSelections)
            {
                if (item.Type == "Fabric")
                {
                    data.FabricId = item.ProductOptionId;
                    foreach (var fabric in data.FabricOptions)
                    {
                        if (fabric.ProductOptionId == data.FabricId)
                            fabric.Default = true;
                        else
                            fabric.Default = false;
                    }
                }
                if (item.Type == "Mesh")
                {
                    data.MeshId = item.ProductOptionId;
                    foreach (var mesh in data.MeshOptions)
                    {
                        if (mesh.ProductOptionId == data.MeshId)
                            mesh.Default = true;
                        else
                            mesh.Default = false;
                    }
                }
                if (item.Type == "Frame")
                {
                    data.FrameId = item.ProductOptionId;
                    foreach (var frame in data.FrameOptions)
                    {
                        if (frame.ProductOptionId == data.FrameId)
                            frame.Default = true;
                        else
                            frame.Default = false;
                    }
                }
            }
            json = SendWebApiMessage(apiUrl + "productimages/" + data.User.ProductId + "/" + data.FabricId + "/" + data.MeshId + "/" + data.FrameId, "GET", "");
            try { data.ProductImage = JsonConvert.DeserializeObject<ProductImageModel>(json); }
            catch { data.ProductImage = null; }

            if (update)
            {
                string result = AddressValidation(data.User, configuration.GetValue<bool>("AppSettings:AddressValidation"));
                if (string.IsNullOrEmpty(result))
                {
                    SendWebApiMessage(apiUrl + "email/" + data.User.UserId + "/2", "GET", "");
                    if (data.User.Language == "English")
                    {
                        ViewBag.SuccessResults = "Your selections have been updated!";
                    }
                    else
                    {
                        ViewBag.SuccessResults = "Vos sélections ont été mises à jour!";
                    }
                }
                else
                {
                    ViewBag.ErrorResults = result;
                }
            }
            else
            {
                if (data.User.Language == "English")
                {
                    ViewBag.SuccessResults = "Your selections have already been updated!";
                }
                else
                {
                    ViewBag.SuccessResults = "Vos sélections ont déjà été mises à jour!";
                }
            }
            ViewBag.Logo = data.Product.LogoImage;
            ViewBag.Logo2 = data.Product.LogoImage2;
            ViewBag.Name = data.Product.Name;
            return View(data);
        }

        public IActionResult ChairPartial(int product, int option1, int option2, int option3)
        {
            string apiUrl = configuration.GetValue<string>("AppSettings:ApiUrl");
            string json = SendWebApiMessage(apiUrl + "productimages/" + product + "/" + option1 + "/" + option2 + "/" + option3, "GET", "");
            ProductImageModel data;
            try { data = JsonConvert.DeserializeObject<ProductImageModel>(json); }
            catch { data = null; }
            return PartialView("_Chair", data);
        }

        public IActionResult Complete(int id)
        {
            string apiUrl = configuration.GetValue<string>("AppSettings:ApiUrl");
            UserProductModel data = new UserProductModel();
            string json = SendWebApiMessage(apiUrl + "users/" + id, "GET", "");
            data.User = JsonConvert.DeserializeObject<UserModel>(json);
            data.User.PhoneNumber = String.Format("{0:(###) ###-####}", data.User.PhoneNumber);
            json = SendWebApiMessage(apiUrl + "userselections/" + id, "GET", "");
            data.UserSelections = JsonConvert.DeserializeObject<List<UserSelectionModel>>(json);
            json = SendWebApiMessage(apiUrl + "products/" + data.User.ProductId, "GET", "");
            data.Product = JsonConvert.DeserializeObject<ProductModel>(json);
            json = SendWebApiMessage(apiUrl + "productoptions/" + data.User.ProductId + "/Fabric", "GET", "");
            data.FabricOptions = JsonConvert.DeserializeObject<List<ProductOptionModel>>(json);
            json = SendWebApiMessage(apiUrl + "productoptions/" + data.User.ProductId + "/Mesh", "GET", "");
            data.MeshOptions = JsonConvert.DeserializeObject<List<ProductOptionModel>>(json);
            foreach (var item in data.UserSelections)
            {
                if (item.Type == "Fabric")
                {
                    data.FabricId = item.ProductOptionId;
                    foreach (var fabric in data.FabricOptions)
                    {
                        if (fabric.ProductOptionId == data.FabricId)
                            fabric.Default = true;
                        else
                            fabric.Default = false;
                    }
                }
                if (item.Type == "Mesh")
                {
                    data.MeshId = item.ProductOptionId;
                    foreach (var mesh in data.MeshOptions)
                    {
                        if (mesh.ProductOptionId == data.MeshId)
                            mesh.Default = true;
                        else
                            mesh.Default = false;
                    }
                }
                if (item.Type == "Frame")
                {
                    data.FrameId = item.ProductOptionId;
                    foreach (var frame in data.FrameOptions)
                    {
                        if (frame.ProductOptionId == data.FrameId)
                            frame.Default = true;
                        else
                            frame.Default = false;
                    }
                }
            }
            json = SendWebApiMessage(apiUrl + "productimages/" + data.User.ProductId + "/" + data.FabricId + "/" + data.MeshId + "/" + data.FrameId, "GET", "");
            try { data.ProductImage = JsonConvert.DeserializeObject<ProductImageModel>(json); }
            catch { data.ProductImage = null; }

            bool status = false;
            DateTime dt;
            if (data.User.Shipped != null)
            {
                dt = (DateTime)data.User.Shipped;
                data.Status = "Your order has been shipped on " + dt.ToString("MMM d, yyyy");
                if (!string.IsNullOrEmpty(data.User.TrackingNumber))
                    data.Status += " Your tracking number is " + data.User.TrackingNumber;
                else
                    data.Status += " Your tracking number will be emailed to you when it is available.";
                status = true;
            }
            if (data.User.InProduction != null && !status)
            {
                dt = (DateTime)data.User.InProduction;
                data.Status = "Your order went into production on " + dt.ToString("MMM d, yyyy");
            }
            ViewBag.Logo = data.Product.LogoImage;
            ViewBag.Logo2 = data.Product.LogoImage2;
            ViewBag.Name = data.Product.Name;
            return View(data);
        }

        public List<string> AddressList(string address)
        {
            List<string> results = new List<string>();
            string url = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=" + address + "&key=AIzaSyB_HWeBELbvx5SELSUAeg2XLt1keM7wu-A";
            string message = SendWebApiMessage(url, "GET", "");
            PlacesTextSearchResponse addresses = JsonConvert.DeserializeObject<PlacesTextSearchResponse>(message);
            foreach (var item in addresses.Results)
            {
                results.Add(item.FormattedAddress);
            }
            return results;
        }

        private string AddressValidation(UserModel data, bool validate)
        {
            string result = "";
            if (validate)
            {
                string address = data.Address1.Replace(' ', '+');
                address += data.City + "+" + data.ProvinceState + "+" + data.Country + "+" + data.PostalZip.Replace(" ", "");
                string url = "https://maps.googleapis.com/maps/api/place/textsearch/json?query=" + address + "&key=AIzaSyB_HWeBELbvx5SELSUAeg2XLt1keM7wu-A";
                string message = SendWebApiMessage(url, "GET", "");
                if (!message.ToLower().Contains("ok"))
                {
                    if (data.Language == "English")
                    {
                        result = "Address not found!";
                    }
                    else
                    {
                        result = "Adresse introuvable!";
                    }
                }
            }
            return result;
        }

        private string SendWebApiMessage(string url, string method, string json)
        {
            string data = "";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Proxy = null;
            request.ServerCertificateValidationCallback = delegate { return true; };
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
            catch (Exception ex) { data = ex.Message; }
            return data;
        }
    }
}