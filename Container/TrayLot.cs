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
    
    public partial class TrayLot
    {
        public TrayLot()
        {
            this.Panel = new HashSet<Panel>();
        }
    
        public int Lotid { get; set; }
        public string TrayGroupName { get; set; }
        public string MachineName { get; set; }
        public string AddTime { get; set; }
        public int ProductInfo { get; set; }
    
        public virtual FinishedLot FinishedLot { get; set; }
        public virtual OnInspectLot OnInspectLot { get; set; }
        public virtual ICollection<Panel> Panel { get; set; }
        public virtual ProductInfo ProductInfo1 { get; set; }
        public virtual WaitLot WaitLot { get; set; }
    }
}