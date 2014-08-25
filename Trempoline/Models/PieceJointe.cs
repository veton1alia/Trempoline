//------------------------------------------------------------------------------
// <auto-generated>
//    Ce code a été généré à partir d'un modèle.
//
//    Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//    Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Trempoline.Models
{
    using System;
    using System.Collections.Generic;
    using Trempoline.Helpers;
    public partial class PieceJointe
    {
        public int IDPieceJointe { get; set; }
        
        private string _LibellePiece;
        public string LibellePiece 
        { 
            get { return _LibellePiece.Trimmed(); }
            set { _LibellePiece = value; } 
        }
        public System.DateTime DatePiece { get; set; }
        public int IDBeneficiare { get; set; }
        public Nullable<int> IDTypeAction { get; set; }
        public byte[] Piece { get; set; }
        public string ContentType { get; set; }
        public virtual Beneficiaire Beneficiaire { get; set; }
        public virtual TypeAction TypeAction { get; set; }
    }
}
