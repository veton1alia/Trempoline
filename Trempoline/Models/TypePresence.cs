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
    
    public partial class TypePresence
    {
        public TypePresence()
        {
            this.Presentiel = new HashSet<Presentiel>();
        }
    
        public int IDTypePresence { get; set; }
        public string Valeur { get; set; }
    
        public virtual ICollection<Presentiel> Presentiel { get; set; }
    }
}
