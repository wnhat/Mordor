//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Container
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductInfo
    {
        public ProductInfo()
        {
            this.TrayLot = new HashSet<TrayLot>();
        }
    
        public int IndexId { get; set; }
        public string Name { get; set; }
        public string ProductType { get; set; }
        public string FGcode { get; set; }
    
        public virtual OninspectProduct OninspectProduct { get; set; }
        public virtual ICollection<TrayLot> TrayLot { get; set; }
    }
}
