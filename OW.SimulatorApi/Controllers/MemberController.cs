using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OW.SimulatorApi.Models;

namespace OW.SimulatorApi.Controllers
{
    public class MemberController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }



    }
}
