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
    
    public partial class Educateur
    {
        public Educateur()
        {
            this.Beneficiaire = new HashSet<Beneficiaire>();
            this.BeneficiaireProgramme = new HashSet<BeneficiaireProgramme>();
        }
    
        public int IDEducateur { get; set; }
        public Nullable<int> IDUser { get; set; }
    
        public virtual ICollection<Beneficiaire> Beneficiaire { get; set; }
        public virtual ICollection<BeneficiaireProgramme> BeneficiaireProgramme { get; set; }
        public virtual User User { get; set; }
    }
}