using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agency.Models.Mappings
{
    public class UserMap : ClassMap<Models.User>
    {
        public UserMap()
        {
            Id(u => u.Id).GeneratedBy.Identity();
            Map(u => u.UserName).Length(30);
            Map(u => u.Password).Column("PasswordHash");
            Map(u => u.PhoneNumber);
            Map(u => u.Role).Nullable();
            Map(u => u.Status);
        }
    }
}
