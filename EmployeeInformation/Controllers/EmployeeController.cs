using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Session;
using EmployeeInformation.Models;
using EmployeeInformation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.RazorPages;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeInformation.Controllers
{
    public class EmployeeController : Controller
    {
        private EmployeeServices services = new EmployeeServices();
        public EmployeeController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("UserConnection").ToString();
            services.UserConnectionString = connectionString;
        }
        // GET: /<controller>/
        public ActionResult<string> GetIpAddress()
        {
            var model = new IPAddressList();
            var hostname = Dns.GetHostName();
            IPHostEntry local = Dns.GetHostEntry(hostname);
            //IPAddress[] IPAddress = getIP.AddressList;
            var strIP = HttpContext.Connection.RemoteIpAddress?.ToString();
            ViewBag.CurrentIP = "Current IP: " + strIP;

            foreach (IPAddress ipAddress in local.AddressList)
            {
                model.IPhost = ipAddress.ToString();
                Console.WriteLine(model.IPhost);
                ViewBag.IpList = "Host IP Address: " + ipAddress.ToString();
            }

            if (IsLocalIP(model.IPhost))
            {
                Console.WriteLine("Yes, it is a local system");
                ViewBag.Msg = "IP Address is Local";
            }
            else
            {
                Console.WriteLine("No, it is not a local system");
                ViewBag.Msg = "IP Address is not in Local";
            }
            return model.IPhost.ToString();
        }

        public static bool IsLocalIP(string host)
        {
            try
            {
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);
                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (IPAddress hostIP in hostIPs)
                {
                    // is localhost
                    if (IPAddress.IsLoopback(hostIP)) return true;
                    // is local address
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP)) return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public IActionResult Index()
        {
            var employees = services.Employees();
            var model = new EmployeeIndexViewModel
            {
                Employees = employees
            };
           //Table list of employees
            return View(model);
        }
        public IActionResult Login()
        {
            var model = new Employee();
            GetIpAddress();
            return View();
        }

        public async Task<IActionResult> LoginController(Employee model)
        {
            var resultIP = GetIpAddress().Value;
            if (ModelState.IsValid)
            {
                if (IsLocalIP(resultIP))
                {
                    ViewBag.Msg = "IP Address is Local";
                    var result = services.Login(model);
                    //if (result.Contains("successfully"))
                    //{
                        HttpContext.Session.SetString("ID", model.Id.ToString());
                        return RedirectToAction("Index");
                    //}
                    //else
                    //{
                    //    var msg = "Invalid";
                    //    return ViewBag.ErrorMsg(msg);
                    //}
                    
                }
                else
                {
                    ViewBag.Msg = "IP Address is not Local";
                    await Authenticate(model);
                    return RedirectToAction("Authenticate");
                }
            }
            return View();
        }

        public static string GenerateOTP()
        {
            var model = new Employee();
            Random randomOTP = new Random();
            model.OTP = randomOTP.Next(100000, 999999);
            return model.OTP.ToString();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendOTP()
        {
            var model = new Employee();
            var EmployeeID = int.Parse(HttpContext.Session.GetString("ID"));
            var otp = int.Parse(HttpContext.Session.GetString("OTP"));
            var GetEmail = services.GetEmail(model, EmployeeID); //get the receiver of the user

            if (ModelState.IsValid)
            {
                var message = new MailMessage();
                message.From = new MailAddress("ISAAC@Toyota.com.ph");
                message.To.Add(new MailAddress(GetEmail.ToString()));
                message.CC.Add("Joshua.Moreno@toyota.com.ph");
                message.Subject = "One Time Password";
                message.Body = "Your One Time Password is " + otp + ". Note: Do not share your OTP";
                message.IsBodyHtml = true;


				using (var smtp = new SmtpClient("tmp-mail-gw-01.toyota.com.ph"))
				{

                    //smtp.UseDefaultCredentials = false;
                    //var credential = new NetworkCredential
                    //{
                    //	UserName = "Joshua.Moreno@toyota.com.ph",  // replace with valid value
                    //	Password = "@S3curity01"  // replace with valid value
                    //};
                    //smtp.Credentials = credential;
                    //smtp.Host = "tmp-mail-gw-01.toyota.com.ph";
                    //smtp.Port = 587; //25
                    //smtp.EnableSsl = true;

                    //smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    //smtp.Timeout = 600000;
					await smtp.SendMailAsync(message);
				}
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authenticate(Employee model)
        {
            if (ModelState.IsValid)
            {
                var result = services.Login(model);
                HttpContext.Session.SetString("ID", model.Id.ToString());

                var otpresult = GenerateOTP();
                HttpContext.Session.SetString("OTP", otpresult.ToString());
                await SendOTP();

                return View(model);
            }
            else
            { return View(); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyOTP(Employee model)
        {
            var otp = int.Parse(HttpContext.Session.GetString("OTP"));
            if (ModelState.IsValid)
            {
                if (model.VerifyOTP == otp)
                {
                    return RedirectToAction("Index");
                }
				else
				{
                    ViewBag.Message = "Invalid Code";
                    //ModelState.AddModelError("Error", "Invalid Code");
                    return View("Authenticate");
                }
			}
            return View("Authenticate");
        }

        public async Task<ActionResult<int>> ResendOTP()
        {
            await SendOTP();
            return View("Authenticate");
        }

        public IActionResult Create()
        {
            var model = new Employee();
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Employee model)
        {
            if (ModelState.IsValid)
            {
                var result = services.Save(model);
                return RedirectToAction("Index");
            }
            else
            { return View(); }
        }
        public IActionResult Delete(int id)
        {
            var result = services.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
