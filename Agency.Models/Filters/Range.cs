using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agency.Models.Filters
{
    public abstract class Range<T>
    {
        public T From { get; set; }

        public T To { get; set; }
    }
}
