using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Trempoline.Helpers;
using Trempoline.Models;


namespace Trempoline.Controllers
{
    public class AttachementController : Controller
    {
        TrempolineEntities _Context;
        bool _Disposed;
        public AttachementController()
        {
            _Context = new TrempolineEntities();
            _Disposed = false;
        }

        [HttpGet]
        public ActionResult Create(int? id, int? IDTypeAction, string Start, string End) //beneficiary id
        {

            if (id.HasValue)
            {
                ViewBag.IDBeneficiaire = id;

                ViewBag.Service = _Context.TypeAction.AsEnumerable().Select(t => new SelectListItem
                {
                    Text = t.Valeur,
                    Value = t.IDTypeAction.ToString(),
                    Selected = IDTypeAction.HasValue && t.IDTypeAction.Equals(t.IDTypeAction)
                });

                IEnumerable<PieceJointe> _Attachements = (from PieceJointe p in _Context.PieceJointe
                                                          where p.IDBeneficiare == id.Value
                                                          select p).ToList()
                                                          .Where(p => Search(IDTypeAction, Start.ToFrenchDate(), End.ToFrenchDate(), p.IDTypeAction, p.DatePiece));

                return View(_Attachements);
            }

            return HttpNotFound("Formulaire non trouvée");
        }

        private bool Search(int? idTypeAction, DateTime? start, DateTime? end, int? idToCompare, DateTime dateToCompare)
        {
            if (idTypeAction.HasValue && start.HasValue && end.HasValue)
            {
                return idTypeAction.Value == idToCompare &&
                        dateToCompare.CompareTo(start.Value) >= 0 &&
                        dateToCompare.CompareTo(end.Value) <= 0;
            }
            else if (idTypeAction.HasValue && start.HasValue)
            {
                return idTypeAction.Value == idToCompare &&
                       dateToCompare.CompareTo(start.Value) >= 0;
            }
            else if (idTypeAction.HasValue && end.HasValue)
            {
                return idTypeAction.Value == idToCompare &&
                        dateToCompare.CompareTo(end.Value) <= 0;
            }
            else if (start.HasValue && end.HasValue)
            {
                return dateToCompare.CompareTo(start.Value) >= 0 &&
                        dateToCompare.CompareTo(end.Value) <= 0;
            }
            else if (idTypeAction.HasValue)
            {
                return idTypeAction.Value == idToCompare;
            }
            else if (start.HasValue)
            {
                return dateToCompare.CompareTo(start.Value) >= 0;
            }
            else if (start.HasValue)
            {
                return dateToCompare.CompareTo(end.Value) <= 0;
            }

            return true;
        }

        [HttpPost]
        public ActionResult Create(int? IDBeneficiaire, IEnumerable<Attachement> attachements, IEnumerable<int> attachementsToDelete)
        {
            try
            {
                using (TransactionScope scope = TransactionScopeHelper.Scope)
                {

                    if (attachementsToDelete != null)
                    {
                        foreach (int i in attachementsToDelete)
                        {
                            _Context.Entry<PieceJointe>(_Context.PieceJointe.Single(p => p.IDPieceJointe == i)).State = System.Data.EntityState.Deleted;
                        }
                    }

                    if (attachements != null)
                    {
                        foreach (Attachement attach in attachements)
                        {
                            if (attach.Piece != null) //if it has file the ones that already have a file are sent but no need to save them 
                            {
                                PieceJointe piece = new PieceJointe();
                                piece.IDBeneficiare = IDBeneficiaire.Value;
                                var date = attach.DatePiece.ToFrenchDate();
                                piece.DatePiece = date.HasValue ? date.Value : DateTime.Now;
                                piece.ContentType = attach.Piece.ContentType;
                                piece.LibellePiece = attach.LibellePiece;
                                piece.IDTypeAction = attach.IDTypeAction;

                                using (MemoryStream target = new MemoryStream())
                                {
                                    attach.Piece.InputStream.CopyTo(target);
                                    piece.Piece = target.ToArray();
                                }

                                _Context.PieceJointe.Add(piece);
                            }
                        }
                    }
                    _Context.SaveChanges();
                    scope.Complete();
                    return RedirectToAction("Detail", "Beneficiary", new { id = IDBeneficiaire.Value });
                }
            }
            catch (Exception e)
            {
                return View("Error", e.InnerException.Message);
            }

            //  return Redirect(Url.RouteUrl(new { controller = "Thread", action = "ShowThreadLastPostPage", threadOid = threadId }) + "#" + postOid);

        }

        [HttpGet]
        public ActionResult Download(int benefId, int fileId)
        {
            try
            {
                using (TrempolineEntities context = new TrempolineEntities())
                {
                    PieceJointe attachement = context.PieceJointe.SingleOrDefault(p => p.IDBeneficiare == benefId && p.IDPieceJointe == fileId);

                    return File(attachement.Piece, attachement.ContentType, attachement.LibellePiece);
                }
            }
            catch (Exception)
            {
                return HttpNotFound("Le fichier n'a pas été trouvée");
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    _Context.Dispose();
                }
            }
            base.Dispose(disposing);
        }

    }

    public class Attachement
    {
        public string LibellePiece { get; set; }
        public string DatePiece { get; set; }
        public Nullable<int> IDTypeAction { get; set; }
        public HttpPostedFileBase Piece { get; set; }

    }

}
