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
    
    public partial class A_Like
    {
        public int LikeId { get; set; }
        public int UserId { get; set; }
        public long SenMeanId { get; set; }
        public int LikeFlag { get; set; }
        public int ActiveFlag { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public Nullable<System.DateTime> DeleteTime { get; set; }
    
        public virtual A_Sen_Mean A_Sen_Mean { get; set; }
        public virtual A_User A_User { get; set; }
    }
}
