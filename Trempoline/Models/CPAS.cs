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
    
    public partial class CPAS
    {
        public CPAS()
        {
            this.Beneficiaire = new HashSet<Beneficiaire>();
        }
    
        public int IDCPAS { get; set; }
        public int CodePostal { get; set; }
        public string Adresse1 { get; set; }
        public string Adresse2 { get; set; }
        public string Localite { get; set; }
    
        public virtual ICollection<Beneficiaire> Beneficiaire { get; set; }
    }
}