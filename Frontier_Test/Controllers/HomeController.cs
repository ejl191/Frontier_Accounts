using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Frontier_Test.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Frontier_Test.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "AAA";


            var request = (HttpWebRequest)WebRequest.Create("https://frontiercodingtests.azurewebsites.net/api/accounts/getall");

            request.Method = "GET";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            var content = string.Empty;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }
            //TODO: Break a lot of this out into methods
            //TODO: Move HTML out of code
            var accounts = GetAccounts(content);
            ViewData["Message"] = "Your contact page.";

            StringBuilder active = new StringBuilder();
            StringBuilder Inactive = new StringBuilder();
            StringBuilder OverDue = new StringBuilder();
            var ul = "<ul class=\"account-data-list\"><li><label>Name:</label>{{LastName}}, {{FirstName}}</li><li><label>Email:</label>{{Email}}</li><li><label>Phone Number:" +
                "</label>{{PhoneNumber}}</li><li><label>Amount Due:</label>{{AmountDue}}</li>{{DueDate}}</ul>";

            foreach(var account in accounts)
            {
                if((int)account.AccountStatusId == 0)
                {
                    var listing = ul.Replace("{{LastName}}", account.LastName);
                    listing = listing.Replace("{{FirstName}}", account.FirstName);
                    listing = listing.Replace("{{Email}}", account.Email);
                    listing = listing.Replace("{{PhoneNumber}}", String.Format("{0:(###)-###-####}", Convert.ToInt64(account.PhoneNumber)));
                    listing = listing.Replace("{{AmountDue}}", "$" + account.AmountDue.ToString());
                    listing = listing.Replace("{{DueDate}}", GetDate(account));
                    active = active.Append(listing);
                }
                else if((int)account.AccountStatusId == 2)
                {
                    var listing = ul.Replace("{{LastName}}", account.LastName);
                    listing = listing.Replace("{{FirstName}}", account.FirstName);
                    listing = listing.Replace("{{Email}}", account.Email);
                    listing = listing.Replace("{{PhoneNumber}}", String.Format("{0:(###)-###-####}", Convert.ToInt64(account.PhoneNumber)));
                    listing = listing.Replace("{{AmountDue}}", "$" + account.AmountDue.ToString());
                    listing = listing.Replace("{{DueDate}}", GetDate(account));
                    Inactive = Inactive.Append(listing);
                } else
                {
                    var listing = ul.Replace("{{LastName}}", account.LastName);
                    listing = listing.Replace("{{FirstName}}", account.FirstName);
                    listing = listing.Replace("{{Email}}", account.Email);
                    listing = listing.Replace("{{PhoneNumber}}", String.Format("{0:(###)-###-####}", Convert.ToInt64(account.PhoneNumber)));
                    listing = listing.Replace("{{AmountDue}}", "$" + account.AmountDue.ToString());
                    listing = listing.Replace("{{DueDate}}", GetDate(account));
                    OverDue = OverDue.Append(listing);
                }
            }

            ViewData["Active"] = active.ToString();
            ViewData["Inactive"] = Inactive.ToString();
            ViewData["Overdue"] = OverDue.ToString();


            return View();

        }

        private static string GetDate(Account account)
        {
            if (String.IsNullOrEmpty(account.PaymentDueDate.ToString()))
            {
                return "";
            } else
            {
                return "<li><label>Due Date:</label>" + account.PaymentDueDate.ToString() + "</li>";
            }
            
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<Account> GetAccounts(string json)
        {
            List<Account> accounts = new List<Account>();

            //TODO: Break this out directly into an object
            var accountArray = JToken.Parse(json);
            for(var x = 0; x < accountArray.Count(); x++)
            {
                var account = new Account()
                {
                    Id = Convert.ToInt32(accountArray[x]["Id"].ToString()),
                    FirstName = accountArray[x]["FirstName"].ToString(),
                    LastName = accountArray[x]["LastName"].ToString(),
                    Email = accountArray[x]["Email"].ToString(),
                    PhoneNumber = accountArray[x]["PhoneNumber"].ToString(),
                    AmountDue = Convert.ToDecimal(accountArray[x]["AmountDue"].ToString()),
                    AccountStatusId = Enum.Parse<AccountStatuses>(accountArray[x]["AccountStatusId"].ToString())
                };

                if (!String.IsNullOrEmpty(accountArray[x]["PaymentDueDate"].ToString()))
                {
                    account.PaymentDueDate = DateTime.Parse(accountArray[x]["PaymentDueDate"].ToString());
                }

                accounts.Add(account);

                
            }

            return accounts;
        }
    
        
        
    }
}
