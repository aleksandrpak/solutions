//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Viewer
{
    using System;
    using System.Collections.Generic;
    
    public partial class Entry
    {
        public Entry()
        {
            this.EntryAliases = new HashSet<EntryAlias>();
        }
    
        public int EntryId { get; set; }
        public string Headword { get; set; }
        public string Article { get; set; }
        public string OriginalHeadword { get; set; }
    
        public virtual ICollection<EntryAlias> EntryAliases { get; set; }
    }
}
