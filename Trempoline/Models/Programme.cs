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
    
    public partial class Programme
    {
        public Programme()
        {
            this.BeneficiaireProgramme = new HashSet<BeneficiaireProgramme>();
        }
    
        public int IDProgramme { get; set; }
        public string Secteur { get; set; }
        public string Niveau { get; set; }
        public string Phase { get; set; }
        public int Sequence { get; set; }
    
        public virtual ICollection<BeneficiaireProgramme> BeneficiaireProgramme { get; set; }
    }
}
