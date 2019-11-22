using Agency.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agency.Models.Filters
{
    public class VacancyFilter : BaseFilter
    {
        public List<Experience> Experience { get; set; }
        public List<Experience> SelectedExperience { get; set; }
        public DateRange StartDateRange { get; set; }
        public DateRange EndDateRange { get; set; }
        public Company CompanyName { get; set; }
        public List<Status> Statuses { get; set; }
    }
}
