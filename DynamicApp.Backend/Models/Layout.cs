using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicApp.Backend.Models
{
    public class Layout
        : OrderedEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LayoutId { get; set; }
        public string Title { get; set; }
        public string Data { get; set; }
        public string Body { get; set; }

        public int SectionId { get; set; }
        public Section Section { get; set; }
    }
}
