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
    
    public partial class B_News
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string NewContent { get; set; }
        public string TitleWithRuby { get; set; }
        public string NewContentWithRuby { get; set; }
        public string ImagePath { get; set; }
        public string MoviePath { get; set; }
        public string NewIdNHK { get; set; }
        public Nullable<System.DateTime> NewsDate { get; set; }
        public string NewsWebUrl { get; set; }
        public Nullable<int> ActiveFlag { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public Nullable<System.DateTime> DeleteTime { get; set; }
    }
}
