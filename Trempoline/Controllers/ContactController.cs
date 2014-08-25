using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tools;
using Trempoline.Models;
using System.Transactions;
using Trempoline.Helpers;

namespace Trempoline.Controllers
{
    public class ContactController : Controller
    {

        ContactInformation _Contact;
        TrempolineEntities _Context;
                    
        public ContactController()
        {
            _Contact = new ContactInformation();
            _Context = new TrempolineEntities();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(_Contact);
        }

        [HttpPost]
        public ActionResult Create(ContactInformation con)
        {

            try
            {
                if (!con.Beneficiary.PermisConduire.HasValue) //if not checked set to null 
                {
                    con.Beneficiary.DatePermisConduire = null;
                }

                _Context.Beneficiaire.Add(con.Beneficiary);
                _Context.SaveChanges();

                if (con.Addresses != null && con.Localities != null)
                {
                    foreach (Localite item in con.Localities)
                    {
                        _Context.Localite.Add(item);
                    }

                    _Context.SaveChanges();

                    int i = 0;
                    foreach (Adresse item in con.Addresses)
                    {
                        item.IDLocalite = con.Localities[i].IDLocalite;
                        _Context.Adresse.Add(item);
                        i++;
                    }

                    _Context.SaveChanges();

                    foreach (Adresse item in con.Addresses)
                    {
                        _Context.AdresseBeneficiaire.Add(
                            new AdresseBeneficiaire
                            {
                                IDAdresse = item.IDAdresse,
                                IDBeneficiare = con.Beneficiary.IDBeneficiare
                            });
                    }

                    _Context.SaveChanges();
                }

                if (con.Stays != null)
                {
                    foreach (Sejour stay in con.Stays)
                    {
                        stay.IDBeneficiare = con.Beneficiary.IDBeneficiare;

                        if (!stay.Prolongation.HasValue)
                        {
                            stay.DateFinProlongation = null;
                        }

                        _Context.Sejour.Add(stay);
                    }

                    _Context.SaveChanges();
                }

                return RedirectToAction("Detail", "Beneficiary", new { id = con.Beneficiary.IDBeneficiare });

            }
            catch (DbUpdateException e)
            {
                throw e;
            }
            catch (DbEntityValidationException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        [HttpGet]
        public ActionResult Edit(Nullable<int> id)
        {

            ContactInformation contact = new ContactInformation();
            try
            {
                contact.Beneficiary = _Context.Beneficiaire.SingleOrDefault(b => b.IDBeneficiare == id.Value);

                contact.Addresses = _Context.AdresseBeneficiaire
                                    .Where(ab => ab.IDBeneficiare == contact.Beneficiary.IDBeneficiare)
                                    .Join(_Context.Adresse,
                                          ab => ab.IDAdresse,
                                          a => a.IDAdresse,
                                          (ab, a) => a)
                                    .ToList();

                return View("Create", contact);
            }
            catch // (Exception e)
            {
                return HttpNotFound("Le beneficiare n'a pas été trouvée");
            }

        }

        [HttpPost]
        public ActionResult Update(ContactInformation contact)
        {
            try
            {
                using (TransactionScope scope = TransactionScopeHelper.Scope)
                {

                    int beneficiaryId = contact.Beneficiary.IDBeneficiare;
                    _Context.Entry(contact.Beneficiary).State = EntityState.Modified;
                 /*   _Context.Entry(_Context.Beneficiaire.SingleOrDefault(b => b.IDBeneficiare == beneficiaryId))
                            .CurrentValues.SetValues(contact.Beneficiary);*/

                    //delete all stays 
                    _Context.Sejour.Where(s => s.IDBeneficiare == beneficiaryId)
                        .ToList().ForEach(s => _Context.Sejour.Remove(s));

                    //add new stays 
                    if (contact.Stays != null)
                    {
                        contact.Stays.ToList().ForEach(s =>
                        {
                            s.Beneficiaire = contact.Beneficiary;
                            s.IDBeneficiare = beneficiaryId;
                            _Context.Sejour.Add(s);
                        });
                    }

                    // delete localities everything else will delete on cascade
                   var locs = _Context.AdresseBeneficiaire
                              .Where(ab => ab.IDBeneficiare == beneficiaryId)
                              .Join(_Context.Adresse,
                                    ab => ab.IDAdresse,
                                    a => a.IDAdresse,
                                    (ab, a) => a)
                                .Join(_Context.Localite,
                                      a => a.IDLocalite,
                                      l => l.IDLocalite,
                                      (a, l) => l)
                                .ToList();

                        locs.ForEach(l => _Context.Localite.Remove(l));

                    _Context.SaveChanges();

                    if (contact.Localities != null && contact.Addresses != null)
                    {
                        foreach (Localite loc in contact.Localities)
                        {
                            _Context.Localite.Add(loc);
                        }

                        _Context.SaveChanges();

                        int i = 0;

                        foreach (Adresse ad in contact.Addresses)
                        {
                            ad.Localite = contact.Localities[i];
                            ad.IDLocalite = contact.Localities[i].IDLocalite;
                            _Context.Adresse.Add(ad);
                            i++;
                        }

                        _Context.SaveChanges();

                        foreach (Adresse ad in contact.Addresses)
                        {
                            _Context.AdresseBeneficiaire.Add(new AdresseBeneficiaire
                                {
                                    IDAdresse = ad.IDAdresse,
                                    IDBeneficiare = beneficiaryId,
                                    Adresse = ad,
                                    Beneficiaire = contact.Beneficiary
                                });
                        }

                        _Context.SaveChanges(); // save addresse beneficiares
                    }

                    scope.Complete(); // everything went well so complete

                    return RedirectToAction("Detail", "Beneficiary", new { id = contact.Beneficiary.IDBeneficiare });

                }
            }

            catch (DbEntityValidationException e)
            {
                return View("~/Views/Shared/Error.cshtml", e.Message);
            }
            catch (Exception e)
            {
                return View("~/Views/Shared/Error.cshtml", e.Message);
            }

        }


        bool _Disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    _Contact.Dispose();
                    _Context.Dispose();
                }

                _Disposed = true;
            }
            base.Dispose(disposing);
        }

    }

    public class ContactInformation : IDisposable
    {
        TrempolineEntities _Db;

        public ContactInformation()
        {
            _Db = new TrempolineEntities();
        }

        public IEnumerable<SelectListItem> AddressTypes
        {
            get
            {

                return _Db.TypeAdresse.AsEnumerable().Select
                    (
                        t => new SelectListItem
                        {
                            Value = t.IDTypeAdresse.ToString(),
                            Text = t.Valeur
                        }
                    );


            }
        }
        public IEnumerable<SelectListItem> TypePriseEnCharges
        {
            get
            {
                return _Db.TypePriseEnCharge.AsEnumerable()
                        .Select
                        (
                            t => new SelectListItem
                            {
                                Value = t.IDTypePriseEnCharge.ToString(),
                                Text = t.Valeur,
                                Selected = Beneficiary!= null && t.IDTypePriseEnCharge == Beneficiary.IDTypePriseEnCharge
                            }
                        );

            }
        }
        public Beneficiaire Beneficiary { get; set; }
        public List<Adresse> Addresses { get; set; }
        public List<Localite> Localities { get; set; }
        public IEnumerable<Sejour> Stays { get; set; }


        bool _Disposed = false;
        protected void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    _Db.Dispose();
                }
                _Disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
