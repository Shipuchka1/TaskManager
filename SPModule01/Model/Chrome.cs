namespace SPModule01.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Chrome")]
    public partial class Chrome
    {
        public int ChromeId { get; set; }

        public string ProcessName { get; set; }

        public long? Memory { get; set; }
    }
}
