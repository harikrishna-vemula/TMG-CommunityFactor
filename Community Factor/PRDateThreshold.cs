//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Community_Factor
{
    using System;
    using System.Collections.Generic;
    
    public partial class PRDateThreshold
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRDateThreshold()
        {
            this.PRSettings = new HashSet<PRSetting>();
        }
    
        public string DateThresholdID { get; set; }
        public int StartProcess { get; set; }
        public int OptOut { get; set; }
        public int SurveyStart { get; set; }
        public int SurveyEnd { get; set; }
        public int NewRate { get; set; }
        public int CreateRenewal { get; set; }
        public int TenantDeadline { get; set; }
        public int ExpireRenewal { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRSetting> PRSettings { get; set; }
    }
}