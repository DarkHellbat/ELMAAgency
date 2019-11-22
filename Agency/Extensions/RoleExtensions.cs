using Agency.Models;
using Agency.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Agency.Extensions
{
    public class RoleExtensions
    {
        public static Role RoleCheck(string roleName)
        {
            var role = new Role();
            switch (roleName)
            {
                case "Admin":
                    role = Models.Role.Admin;
                    break;
                case "Employer":
                    role = Models.Role.Employer;
                    break;
                case "Jobseeker":
                    role = Models.Role.Jobseeker;
                    break;
            }
            return role;
        }
    }
}