using Agency.Models.Models;
using FluentNHibernate;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agency.Models.Mappings
{
    public class CandidateMap : ClassMap<Candidate>
    {
        public CandidateMap()
        {
            Id(u => u.Id).GeneratedBy.Identity();
            Map(c => c.Name).Length(100);
            Map(c => c.DateofBirth);
            References(c => c.User);
            HasManyToMany(c => c.Experience).Table("CandidateExperience");
            References(c => c.Avatar).Cascade.SaveUpdate(); 
        }
    }
}
