//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RDLCReport.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class tb_business_unit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tb_business_unit()
        {
            this.tb_project = new HashSet<tb_project>();
        }
    
        public int BusinessUnitID { get; set; }
        public string BusinessUnitName { get; set; }
        public string BusinessUnitCategory { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_project> tb_project { get; set; }
    }
}
