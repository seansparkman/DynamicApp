using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicApp.Backend.Models
{
    public class OrderedEntity
        : BaseEntity
    {
        public int Order { get; set; }
    }
}
