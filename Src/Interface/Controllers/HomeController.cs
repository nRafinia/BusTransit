using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Engine.Accounting.Data;
using F4ST.Common.Containers;
using F4ST.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Interface.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Interface.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServiceProvider _provider;

        public HomeController(ILogger<HomeController> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        public async Task<IActionResult> Index()
        {
            using (var rep = _provider.GetRepository("DB"))
            {
                await rep.Add(new User()
                {
                    Id=null,
                    Firstname = "test"
                });
                await rep.SaveChanges();
            }

            return View();
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
