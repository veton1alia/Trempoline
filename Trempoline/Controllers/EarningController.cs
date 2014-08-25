using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Trempoline.Controllers
{
    public class EarningController : Controller
    {
        //
        // GET: /Earning/
        [HttpGet]
        public ActionResult Create(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(object id)
        {
            return View();
        }

    }
}
