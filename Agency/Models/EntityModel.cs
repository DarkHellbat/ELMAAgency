using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.Models
{
    public abstract class EntityModel<T>
    {
        public T Entity { get; set; }
        public long Id { get; set; }
    }
}