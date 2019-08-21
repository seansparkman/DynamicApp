using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicApp.Backend.Models
{
    public class Section
        : OrderedEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SectionId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        public ICollection<Layout> Children { get; set; }


        public int PageId { get; set; }
        public MobilePage Page { get; set; }
    }
}
