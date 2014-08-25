using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Trempoline.Controllers
{
    public class ProgramController : Controller
    {
        //
        // GET: /Program/

        [HttpGet]
        public ActionResult Create(int? id)
        {
            if(id.HasValue)
            {

            }
            return View();
        }

    }
}
