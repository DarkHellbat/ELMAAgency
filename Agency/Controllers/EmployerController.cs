﻿using Agency.Models;
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
    public class EmployerController : BaseController
    {
        private CompanyRepository companyRepository;
        private EmployerRepository employerRepository;
        private JobseekerRepository jobseekerRepository;

        public EmployerController(JobseekerRepository jobseekerRepository ,EmployerRepository employerRepository, CompanyRepository companyRepository, ExperienceRepository experienceRepository, UserRepository userRepository)
            : base(userRepository, experienceRepository)
        {
            this.employerRepository = employerRepository;
            this.companyRepository = companyRepository;
            this.jobseekerRepository = jobseekerRepository;
        }
        /// <summary>
        /// Метод для формирования списка компаний
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetCompanies()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (var c in companyRepository.GetAll())
            {
                SelectListItem item = new SelectListItem
                {
                    Text = c.CompanyName,
                    Value = c.Id.ToString()
                };
                selectList.Add(item);
            }
            return selectList;
        }

        public ActionResult Main()
        {
            return View();
        }
        /// <summary>
        /// Метод для создания новой вакансии
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateVacancy()
        {
            var model = new VacancyViewModel
            {
                Experience = GetExperienceLists(),
                Company = GetCompanies() 
            };
            return View(model);
        } 

        [HttpPost]
        public ActionResult CreateVacancy(VacancyViewModel model)
        {
            if (ModelState.IsValid)
            {
                List<long> IdList = new List<long>();
                if (model.NewExperience != null)
                {
                    IdList.AddRange(experienceRepository.CreateNewExperience(model.NewExperience));
                }
                if (model.NewExperience == null && model.SelectedExperience == null)
                {
                    IdList.AddRange(experienceRepository.CreateNewExperience("Без опыта"));
                }
                if (model.SelectedExperience != null)
                    foreach (var e in model.SelectedExperience)
                {
                    IdList.Add(Convert.ToInt64(e));
                }
                var vacancy = new Vacancy
                {
                    Ends = model.Ends,
                    Starts = model.Starts,
                    VacancyName = model.Name,
                    Status = Status.Active,
                    VacancyDescription = model.Description,
                    Company = companyRepository.Load(long.Parse(model.SelectedCompany)),
                    Requirements = experienceRepository.GetSelectedExperience(IdList)
                };
                try
                {
                    var id = employerRepository.SaveWProcedure(vacancy, Convert.ToInt64(User.Identity.GetUserId()));
                    vacancy = employerRepository.Load(id);
                    vacancy.Requirements = experienceRepository.GetSelectedExperience(IdList);
                    employerRepository.Save(vacancy);
                    return RedirectToAction("Main", "employer");
                }
                catch
                {
                    return RedirectToAction("Main", "employer");
                }
            }
            else
            {
                return RedirectToAction("Main");
            }
        }
        /// <summary>
        /// Метод для изменения выбранной вакансии
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult EditVacancy(long Id)
        {
            var vacancy = employerRepository.Load(Id);
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var c in companyRepository.GetAll())
            {
                items.Add(new SelectListItem { Text = c.CompanyName, Value = c.Id.ToString() });
                if (c == vacancy.Company)
                {
                    items.Last().Selected = true;
                }
            }

            var exp = GetExperienceLists();
            foreach (SelectListItem e in exp)
            {
                if (vacancy.Requirements.Contains(experienceRepository.Load(Convert.ToInt64(e.Value))) == true)
                    e.Selected = true;
            }

            var model = new VacancyViewModel
            {
                Entity = vacancy,
                Id = vacancy.Id,
                Experience = exp,
                Company = items,
                Description = vacancy.VacancyDescription,
                Ends = vacancy.Ends,
                Name = vacancy.VacancyName,
                Starts = vacancy.Starts
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult EditVacancy(VacancyViewModel model)
        {
            List<long> IdList = new List<long>();
            foreach (var e in model.SelectedExperience)
            {
                IdList.Add(Convert.ToInt64(e));
            }
            var vacancy = new Vacancy
            {
                Id = model.Id,
                Ends = model.Ends,
                Starts = model.Starts,
                VacancyName = model.Name,
                Status = Status.Active,
                VacancyDescription = model.Description,
                Company = companyRepository.Load(Convert.ToInt64(model.SelectedCompany)),
                Creator = UserManager.FindById(Convert.ToInt64(User.Identity.GetUserId())),
                Requirements = experienceRepository.GetSelectedExperience(IdList)
            };
            employerRepository.Save(vacancy);
            return RedirectToAction("Main", "employer");
        }

        /// <summary>
        /// Метод позволяет открыть или закрыть выбранную вакансию
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult ChangeStatus(long Id)
        {
            Vacancy vacancy = employerRepository.Load(Id);
            if (vacancy.Status == Status.Active)
                vacancy.Status = Status.Blocked;
            else
                vacancy.Status = Status.Active;
            employerRepository.Save(vacancy);
            return RedirectToAction("ShowVacancies", "common"); 
        }
        /// <summary>
        /// Метод формирует список компаний для передачи в представление фильтра
        /// </summary>
        /// <returns></returns>
        public ActionResult FillCompany()
        {
            var list = companyRepository.GetAll()
                        .AsEnumerable()
                        .Select(s => new { Id = s.Id, Name = s.CompanyName})
                        .ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Метод формирует список опыта для передачи в представление фильтра
        /// </summary>
        /// <returns></returns>
        public ActionResult FillExperience()
        {
            var list = experienceRepository.GetAll()
                        .AsEnumerable()
                        .Select(s => new { Id = s.Id, Name = s.Skill })
                        .ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Позволяет HR найти анкету, подходящую по параметрам вакансии
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult FindCandidate(long Id)
        {
            var exp = employerRepository.Load(Id).Requirements;
            List<long> list = new List<long>();
            foreach (var e in exp)
            {
                list.Add(e.Id);
            }
            var model = new ProfileListViewModel
                {
                Profiles = jobseekerRepository.FindSuitableCandidate(list)
                };

            return View("ShowCandidates", "", model);
        }



    }
}