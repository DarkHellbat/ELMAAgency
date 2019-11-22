using Agency.Models;
using Agency.Models.Models;
using Agency.Models.Repository;
using Microsoft.AspNet.Identity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Agency.Controllers
{
    public class JobseekerController : BaseController
    {
        private JobseekerRepository jobseekerRepository;
        private BinaryFileRepository fileRepository;
        private EmployerRepository employerRepository;

        public JobseekerController(EmployerRepository employerRepository, JobseekerRepository jobseekerRepository, ExperienceRepository experienceRepository, BinaryFileRepository fileRepository, 
            UserRepository userRepository) : base (userRepository, experienceRepository)
        {
            this.jobseekerRepository = jobseekerRepository;
            this.employerRepository = employerRepository;
            this.experienceRepository = experienceRepository;
            this.fileRepository = fileRepository;
        }

        public ActionResult Main()
        {
            return View();
        }
  /// <summary>
  /// Метод для создания новой анкеты
  /// </summary>
  /// <returns></returns>
        public ActionResult CreateProfile()
        {
            var model = new ProfileModel
            {
                Experience = GetExperienceLists()
            };   
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> CreateProfile(ProfileModel model)
        {
            if (ModelState.IsValid)
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;
                var file = new BinaryFile();
                if (model.Photo != null)
                {
                    file = new BinaryFile
                    {
                        Name = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + model.Photo.FileName,
                        Path = Path.Combine(path, @"App_Data\Files", DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + model.Photo.FileName),
                        ContentType = model.Photo.ContentType
                    };
                    if (!Directory.Exists(file.Path))
                    {
                        Directory.CreateDirectory(Path.Combine(path, @"App_Data\Files"));
                    }
                    using (var fileStream = System.IO.File.Create(file.Path))
                    {
                        model.Photo.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
                        model.Photo.InputStream.CopyTo(fileStream);
                    }
                    fileRepository.Save(file);
                }
            List<long> IdList = new List<long>();
            if (model.NewExperience!=null)
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
            Candidate candidate = new Candidate
                {
                    DateofBirth = model.DateOfBirth,
                    Name = model.Name,
                    Experience = experienceRepository.GetSelectedExperience(IdList),
                    User = UserManager.FindById(Convert.ToInt64(User.Identity.GetUserId())),
                    Avatar = file
                };
                try
                {
                    jobseekerRepository.Save(candidate);
                    return RedirectToAction("Main", "Jobseeker");
                }
               catch
                {
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record Inserted Successfully')", true);
                    return RedirectToAction("Main", "Jobseeker");
                }

            }
            return RedirectToAction("Main", "Jobseeker"); //добавить оповещения
        }
        /// <summary>
        /// Метод изменяет анкету текущего пользователя
        /// </summary>
        /// <returns></returns>
        public ActionResult EditProfile()
        {
            var profile = jobseekerRepository.FindProfile(Convert.ToInt64(User.Identity.GetUserId()));
            if (profile!=null)
            {
                var exp = GetExperienceLists();
                foreach (SelectListItem e in exp)
                {
                    if (profile.Experience.Contains(experienceRepository.Load(Convert.ToInt64(e.Value))) == true)
                        e.Selected = true;
                }
                BinaryFile file = new BinaryFile();
                if (profile.Avatar != null && System.IO.File.Exists(profile.Avatar.Path))
                {
                    file.Content = System.IO.File.ReadAllBytes(profile.Avatar.Path);
                    file.ContentType = profile.Avatar.ContentType;
                    file.Name = profile.Avatar.Name;
                    file.Path = profile.Avatar.Path;
                }
                var model = new ProfileModel
                {
                    Entity = profile,
                    Id = profile.Id,
                    Experience = exp,
                    DateOfBirth = profile.DateofBirth,
                    Name = profile.Name,
                    File = file
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("CreateProfile", "Jobseeker");
            }
        }
        [HttpPost]
        public ActionResult EditProfile(ProfileModel model)
        {
            if (ModelState.IsValid)
            {
                
                List<long> IdList = new List<long>();
                foreach (var e in model.SelectedExperience)
                {
                    IdList.Add(Convert.ToInt64(e));
                }
                if (model.NewExperience != null)
                {
                    IdList.AddRange(experienceRepository.CreateNewExperience(model.NewExperience));
                }
                
                var candidate = new Candidate
                {
                    Id = model.Id,
                    DateofBirth = model.DateOfBirth,
                    Name = model.Name,
                    User = UserManager.FindById(Convert.ToInt64(User.Identity.GetUserId())),
                    Experience = experienceRepository.GetSelectedExperience(IdList)
                };
                if (model.Photo != null)
                {
                    var path = AppDomain.CurrentDomain.BaseDirectory;
                    var file = new BinaryFile()
                    {
                        Name = model.Photo.FileName,
                        Path = Path.Combine(path, @"App_Data\Files", DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + model.Photo.FileName),
                        ContentType = model.Photo.ContentType
                    };
                    if (!Directory.Exists(file.Path))
                    {
                        Directory.CreateDirectory(Path.Combine(path, @"App_Data\Files"));
                    }
                    using (var fileStream = System.IO.File.Create(file.Path))
                    {
                        model.Photo.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
                        model.Photo.InputStream.CopyTo(fileStream);
                    }
                    fileRepository.Save(file);
                    candidate.Avatar = file;
                }
                try
                {
                    jobseekerRepository.Save(candidate);
                    return RedirectToAction("Main", "Jobseeker");
                }
                catch
                {
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record Inserted Successfully')", true);
                    return RedirectToAction("Main", "Jobseeker");
                }

            }
            return RedirectToAction("Main", "Jobseeker"); //добавить оповещения
        }
        /// <summary>
        /// Позволяет соискателю найти подходящую вакансию
        /// </summary>
        /// <returns></returns>
        public ActionResult FindVacancy()
        {
            var exp = jobseekerRepository.FindProfile(long.Parse(User.Identity.GetUserId())).Experience;
            List<long> list = new List<long>();
            foreach (var e in exp)
            {
                list.Add(e.Id);
            }
            var model = new VacancyListViewModel
            {
                Vacancies = employerRepository.FindSuitableVacancy(list)
            };

            return View("ShowVacancies", "", model);
        }
    }
}