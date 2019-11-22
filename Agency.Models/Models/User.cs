using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agency.Models.Models
{
    public class User : IUser<long>
    {
        public User()
        {
            Role = Role.None;
            //Status.IsDefined(Status, Status.Active);
        }
        public virtual long Id { get; set; }
        [Display(Name = "Логин")]
        //[InFastSearch]
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
        public virtual long PhoneNumber { get; set; }
        public virtual Role Role { get; set; }
        public virtual Status Status { get; set; }
    }
}
