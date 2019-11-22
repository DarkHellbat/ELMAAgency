using Agency.Models;
using Agency.Models.Filters;
using Agency.Models.Models;
using Agency.Models.Repository;
using Microsoft.AspNet.Identity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agency.Controllers
{
    public class CommonController : BaseController 
    {
        private EmployerRepository employerRepository;
        private JobseekerRepository jobseekerRepository;
        private CompanyRepository companyRepository;
        public CommonController(CompanyRepository companyRepository, UserRepository userRepository, EmployerRepository employerRepository, ExperienceRepository experienceRepository, JobseekerRepository jobseekerRepository)
            : base (userRepository, experienceRepository)
        {
            this.companyRepository = companyRepository;
            this.employerRepository = employerRepository;
            this.jobseekerRepository = jobseekerRepository;
        }
        public ActionResult Redirect()
        {
            var role = UserManager.GetRoles(Convert.ToInt64(User.Identity.GetUserId())).SingleOrDefault();
            //иногда возникает проблема с созданием UserManager. Выглядит как проблема из коробки. Но оно работает само по себе
            return RedirectToAction("Main", String.Format("{0}", role.ToString()));
        }
        /// <summary>
        /// Метод отображает необходимые вакансии для различных ролей
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ActionResult ShowVacancies(VacancyFilter filter, FetchOptions options)
        {
            var model = new VacancyListViewModel
            {
                Role = CurrentUser.Role
            };
            switch(CurrentUser.Role)
            {
                case Models.Role.Employer:
                    {
                        model.Vacancies = employerRepository.ShowMyVacancies(CurrentUser.Id, filter, options);
                       return View(model);
                    }
                case Models.Role.Admin:
                    {
                        model.Vacancies = employerRepository.GetAllWithSort(options);
                        return View(model);
                    }
                case Role.Jobseeker:
                    {
                        model.Vacancies = employerRepository.GetAllWithSort(options);
                        return View(model);
                    }                   
            }
            return View();
        }
        /// <summary>
        /// Метод отображает список анкет
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ActionResult ShowCandidates(JobseekersFilter filter, FetchOptions options)
        {
            var model = new ProfileListViewModel
            {
                Profiles = jobseekerRepository.GetAllWithSort(options)
            };

            return View(model);
        }

        public ActionResult AccessError()
        {
            ViewBag.Message = @"это не те дроиды... В смысле, вы не обладаете нужными правами доступа
                                для посещения этой страницы.";
            ViewBag.Title = "Ты не пройдешь!";
            ViewData["Role"] = CurrentUser.Role.ToString();

            return View();
        }
        /// <summary>
        /// метод для получения вакансий, соответствующих критериям поиска
        /// </summary>
        /// <param name="selectedCompany"></param>
        /// <param name="selectedExperience"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult GetSelectedItems(string selectedCompany, string selectedExperience, string name)
        {
            if (long.TryParse(selectedCompany, out long company) != true)
            {
                company = 0;
            }
            else
            {
                company = long.Parse(selectedCompany);
            }
            List<Experience> experiences = new List<Experience>();
            if (long.TryParse(selectedExperience, out long experience) != true)
            {
                experiences = null;
            }
            else
            {
                experiences.Add(experienceRepository.Load(long.Parse(selectedExperience)));
            }
            VacancyFilter filter = new VacancyFilter
            {
                CompanyName = companyRepository.Load(company),
                Experience = experiences,
                SearchString = name

            };
            IList<Vacancy> vacancies = employerRepository.GetVacanciesFiltered(filter);
            var model = new VacancyListViewModel
                {
                Vacancies = vacancies,
                Role = CurrentUser.Role
                };
            return PartialView("VacanciesTable", model);
        }
    }
}