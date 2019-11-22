using Agency.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agency.Models.Filters
{
    public class ExperienceFilter : BaseFilter
    {
        public List<Experience> Experiences { get; set; }
    }
}
