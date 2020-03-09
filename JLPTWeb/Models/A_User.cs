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
    using System.ComponentModel.DataAnnotations;

    public partial class A_User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public A_User()
        {
            this.A_Alphabet = new HashSet<A_Alphabet>();
            this.A_Comp = new HashSet<A_Comp>();
            this.A_Country = new HashSet<A_Country>();
            this.A_Kanji = new HashSet<A_Kanji>();
            this.A_KanjiExam = new HashSet<A_KanjiExam>();
            this.A_Like = new HashSet<A_Like>();
            this.A_Mean = new HashSet<A_Mean>();
            this.A_Sen_Mean = new HashSet<A_Sen_Mean>();
            this.A_Sen_Mean1 = new HashSet<A_Sen_Mean>();
            this.A_Sentence = new HashSet<A_Sentence>();
            this.A_Sentence1 = new HashSet<A_Sentence>();
            this.A_Sentence2 = new HashSet<A_Sentence>();
            this.A_Topic = new HashSet<A_Topic>();
            this.A_TopicSentence = new HashSet<A_TopicSentence>();
            this.A_TopicVoc = new HashSet<A_TopicVoc>();
            this.A_TopicVocDetail = new HashSet<A_TopicVocDetail>();
            this.A_Voc_Mean = new HashSet<A_Voc_Mean>();
            this.A_Voc_Mean1 = new HashSet<A_Voc_Mean>();
            this.A_Voc_Sen = new HashSet<A_Voc_Sen>();
            this.A_Vocabulary = new HashSet<A_Vocabulary>();
        }
    
        public int UserId { get; set; }
        [Required(ErrorMessage = "B?n chua nh?p d?a ch? Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "B?n chua nh?p m?t kh?u")]
        public string Password { get; set; }
        [Required(ErrorMessage = "B?n chua x? nh?p m?t kh?u")]
        [Compare("Password", ErrorMessage = "M?t kh?u kh?g kh?p")]
        public string ConfirmPassword { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public string TelNumber { get; set; }
        public string Sex { get; set; }
        public int RoleId { get; set; }
        public int ActiveFlag { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public Nullable<System.DateTime> DeleteTime { get; set; }
        public Nullable<int> LevelId { get; set; }
        public string AboutMe { get; set; }
        public string Avatar { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Alphabet> A_Alphabet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Comp> A_Comp { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Country> A_Country { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Kanji> A_Kanji { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_KanjiExam> A_KanjiExam { get; set; }
        public virtual A_Level A_Level { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Like> A_Like { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Mean> A_Mean { get; set; }
        public virtual A_Role A_Role { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Sen_Mean> A_Sen_Mean { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Sen_Mean> A_Sen_Mean1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Sentence> A_Sentence { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Sentence> A_Sentence1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Sentence> A_Sentence2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Topic> A_Topic { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_TopicSentence> A_TopicSentence { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_TopicVoc> A_TopicVoc { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_TopicVocDetail> A_TopicVocDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Voc_Mean> A_Voc_Mean { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Voc_Mean> A_Voc_Mean1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Voc_Sen> A_Voc_Sen { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<A_Vocabulary> A_Vocabulary { get; set; }
    }
}
