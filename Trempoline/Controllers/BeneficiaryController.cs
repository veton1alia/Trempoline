using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Trempoline.Models;
using Trempoline.Helpers;

namespace Trempoline.Controllers
{
    public class BeneficiaryController : Controller
    {
        //
        // GET: /Beneficiary/

        public ActionResult Index(SearchBeneficiary searhBeneficiairy)
        {
            return View(searhBeneficiairy.Get());
        }

        public ActionResult Detail(int? id = null)
        {
            Beneficiaire benef = null;

            if (id.HasValue)
            {
                using (TrempolineEntities db = new TrempolineEntities())
                {
                    benef = db.Beneficiaire.SingleOrDefault(b => b.IDBeneficiare == id);
                }
            }

            return View(benef);
        }


        public string Delete(int id)
        {
            try 
	        {	        
		        using (TrempolineEntities context = new TrempolineEntities())
                {
                   var benef = context.Beneficiaire.SingleOrDefault(b=>b.IDBeneficiare == id);
                   context.Beneficiaire.Remove(benef);
                   context.SaveChanges();
                }
                return "OK";
	        }
	        catch (Exception e)
	        {
		
		        return e.InnerException != null ? e.InnerException.Message : e.Message;
	        }
            
        }
    }

    public class SearchBeneficiary
    {
        public Nullable<int> IDBeneficiaire { get; set; } // data to be shown in search table
        public Nullable<DateTime> DateEntree { get; set; } // data to be shown in search table 
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public Nullable<Secteur> Secteur { get; set; }
        public Nullable<Choice> Suivi { get; set; }
        public string DateEntreeDebut { get; set; }
        public string DateEntreeFin { get; set; }
        public Nullable<Choice> ServiceKangourou { get; set; }
        public Nullable<Choice> ServiceParentalite { get; set; }
        public Nullable<Choice> Horus { get; set; }

        public IEnumerable<SearchBeneficiary> Get()
        {
            using (TrempolineEntities context = new TrempolineEntities())
            {
                //Nullable<DateTime> dateDebut = DateEntreeDebut != null ? DateEntreeDebut.ToFrenchDate() : null;
                //Nullable<DateTime> dateFin = DateEntreeFin != null ? DateEntreeFin.ToFrenchDate() : null;

                IEnumerable<SearchBeneficiary> list = from Beneficiaire b in context.Beneficiaire
                                                      where Nom == null && Prenom == null  //if all null then don't go any further, just one to continue
                                                            || Nom != null && Prenom != null && b.Nom.Equals(Nom) && b.Prenom.Equals(Prenom)
                                                            || Nom != null && b.Nom.Equals(Nom)
                                                            || Prenom != null && b.Prenom.Equals(Prenom)
                                                      join Sejour s in context.Sejour on b.IDBeneficiare equals s.IDBeneficiare //into bs
                                                      //from g in bs.DefaultIfEmpty() //not left join 
                                                      group s by new { b.IDBeneficiare, b.Nom, b.Prenom } into grouped
                                                      select new SearchBeneficiary()
                                                      {
                                                          IDBeneficiaire = grouped.Key.IDBeneficiare,
                                                          Nom = grouped.Key.Nom,
                                                          Prenom =grouped.Key.Prenom,
                                                          DateEntree = grouped.Min(s => s.DateEntree)
                                                      };

                if (list.Any()) return list.ToList();
                else return null;
            }
        }


    }

    public enum Choice { Oui, Non, OuiNon }
    public enum Secteur { PremierContact, Accueil, CommunauteTherapeutique, ReinsertionSociale }
}
