using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Trempoline.Models;
using Trempoline.Helpers;

namespace Trempoline.Controllers
{
    public class EarningController : Controller
    {
        //
        // GET: /Earning/
        [HttpGet]
        public ActionResult Create(int id)
        {
            
            using (TrempolineEntities context = new TrempolineEntities())
            {

                Beneficiaire b = context.Beneficiaire.SingleOrDefault(be => be.IDBeneficiare == id);
                ViewBag.Total = Value(b.RevenusCPAS) +
                                Value(b.RevenusAllocHandic) +
                                Value(b.MontantPensionAlim) + 
                                Value(b.RevenusIITMutuelle) + 
                                Value(b.AutresRevenus);

                return View(b);
            }

        }

        private double Value(double? value)
        {
            return value.HasValue ? value.Value : 0;
        }

        [HttpPost]
        public ActionResult Create(Beneficiaire beneficiary)
        {
            using (TrempolineEntities context = new TrempolineEntities())
            {
              
                Beneficiaire benef = context.Beneficiaire.SingleOrDefault(b => b.IDBeneficiare == beneficiary.IDBeneficiare);


                //benef.RevenusIITMutuelle = beneficiary.RevenusIITMutuelle;
                //benef.RevenusCPAS = beneficiary.RevenusCPAS;
                //benef.RevenusAllocHandic = beneficiary.RevenusAllocHandic;
                //benef.MontantPensionAlim = beneficiary.MontantPensionAlim;
                //benef.AutresRevenus = beneficiary.AutresRevenus;
                //benef.DateDebutRevenuCPAS = beneficiary.DateDebutRevenuCPAS;
                //benef.DateDebutRevenuIITMutuelle = beneficiary.DateDebutRevenuIITMutuelle;
                //benef.CommentairesRevenus = beneficiary.CommentairesRevenus;
                //context.SaveChanges(); 

                if (benef.Update(beneficiary, Request.Form.AllKeys, new string[] { "IDBeneficiare" }))
                {
                    context.SaveChanges();
                }

                return Redirect(String.Format("/Beneficiary/Detail/{0}#tab={1}", beneficiary.IDBeneficiare, "Earning"));   
            }
            //return RedirectToAction("Detail", "Beneficiary", new { id = beneficiary.IDBeneficiare, tab="Earning" });
        }
    }

}
