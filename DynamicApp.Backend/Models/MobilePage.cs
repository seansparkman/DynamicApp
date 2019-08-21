using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicApp.Backend.Models
{
    public class MobilePage
        : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MobilePageId { get; set; }

        public string Title { get; set; }
        public ICollection<Section> Sections { get; set; }
    }
}
