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
    
    public partial class User
    {
        public User()
        {
            this.FinishedLot = new HashSet<FinishedLot>();
            this.InspectResult = new HashSet<InspectResult>();
            this.OnInspectLot = new HashSet<OnInspectLot>();
            this.AET_IMAGE_EXAM_RESULT = new HashSet<AET_IMAGE_EXAM_RESULT>();
        }
    
        public int IndexId { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string PassWord { get; set; }
        public int Grade { get; set; }
    
        public virtual ICollection<FinishedLot> FinishedLot { get; set; }
        public virtual ICollection<InspectResult> InspectResult { get; set; }
        public virtual ICollection<OnInspectLot> OnInspectLot { get; set; }
        public virtual ICollection<AET_IMAGE_EXAM_RESULT> AET_IMAGE_EXAM_RESULT { get; set; }
    }
}