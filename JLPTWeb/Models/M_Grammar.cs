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
    
    public partial class M_Grammar
    {
        public string GrammarID { get; set; }
        public string LevelID { get; set; }
        public string BookID { get; set; }
        public string GrammarName { get; set; }
        public string Struct { get; set; }
        public string MeanVN { get; set; }
        public string MeanEN { get; set; }
        public string FullVN { get; set; }
        public string FullEN { get; set; }
        public string FullJP { get; set; }
        public string Relationship { get; set; }
        public string Group { get; set; }
        public string SearchKey { get; set; }
        public string Yobi_2 { get; set; }
        public string Yobi_3 { get; set; }
        public string Yobi_4 { get; set; }
        public Nullable<System.DateTime> InsDate { get; set; }
        public Nullable<System.DateTime> UpdDate { get; set; }
    
        public virtual M_Level M_Level { get; set; }
    }
}
