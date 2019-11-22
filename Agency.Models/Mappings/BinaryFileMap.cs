using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agency.Models.Mappings
{
    public class BinaryFileMap : ClassMap<Models.BinaryFile>
    {
        public BinaryFileMap()
        {
            Id(f => f.Id).GeneratedBy.Identity();
            Map(f => f.Name);
            Map(f => f.Path);
            Map(f => f.ContentType);
            Map(f => f.Content).Length(int.MaxValue);

        }
    }
}
