//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JLPTWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class A_Mean
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public A_Mean()
        {
            this.A_Sen_Mean = new HashSet<A_Sen_Mean>();
            this.A_Voc_Mean = new HashSet<A_Voc_Mean>();
        }
    
        public long MeanId { get; set; }
        public string MeanContent { get; set; }
        public string ContentSearch { get; set; }
        public string ContentFormat { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> ActiveFlag { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public Nullable<System.DateTime> DeleteTime { get; set; }
    
        public virtual A_User A_User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Sen_Mean> A_Sen_Mean { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Voc_Mean> A_Voc_Mean { get; set; }
    }
}