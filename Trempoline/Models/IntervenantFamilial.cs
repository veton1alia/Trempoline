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
    
    public partial class IntervenantFamilial
    {
        public int IDIntervenantFamimlial { get; set; }
        public string Rôle { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Adresse1 { get; set; }
        public string Adresse2 { get; set; }
        public string Localite { get; set; }
        public Nullable<int> CodePostal { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string GSM { get; set; }
        public Nullable<int> IDBeneficiare { get; set; }
    
        public virtual Beneficiaire Beneficiaire { get; set; }
    }
}
