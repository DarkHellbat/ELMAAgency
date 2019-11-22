using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agency.Models.Mappings
{
    public class CompanyMap : ClassMap<Models.Company>
    {
        public CompanyMap()
        {
            Id(u => u.Id).GeneratedBy.Identity();
            Map(u => u.CompanyName).Length(100);
        }
    }
}
