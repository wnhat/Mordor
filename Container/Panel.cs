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
    
    public partial class Panel
    {
        public int IndexId { get; set; }
        public string PanelId { get; set; }
        public int LotId { get; set; }
        public int PIAOI1PANELJUDGE { get; set; }
        public int PIAOI2PANELJUDGE { get; set; }
        public int TFEAOIPANELJUDGE { get; set; }
        public int ACTAOIPANELJUDGE { get; set; }
        public string LastGrade { get; set; }
        public string LastDetailGrade { get; set; }
    
        public virtual InspectResult InspectResult { get; set; }
        public virtual TrayLot TrayLot { get; set; }
    }
}
