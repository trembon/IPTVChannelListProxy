using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IPTVChannelListProxy.Models;
using IPTVChannelListProxy.Services;
using IPTVChannelListProxy.Database;

namespace IPTVChannelListProxy.Controllers
{
    public class HomeController : Controller
    {
        private readonly DefaultContext defaultContext;

        public HomeController(DefaultContext defaultContext)
        {
            this.defaultContext = defaultContext;
        }

        public IActionResult Index()
        {
            var model = new SourcesViewModel();
            model.M3USources = defaultContext.M3USources.OrderBy(s => s.Name).ToList();

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
