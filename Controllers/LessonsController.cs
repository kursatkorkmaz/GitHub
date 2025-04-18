using ExamEase.Data;
using ExamEase.Models;
using ExamEase.Resources;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System;
using ExamEase.Utils;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.Threading.Tasks;


namespace ExamEase.Controllers
{

    [Authorize(Roles = "System Admin, Teacher,Student")]
    public class LessonsController :Controller

    {

        private DefaultDBContext db;
        private Util util;
        private readonly UserManager<AspNetUsers> _userManager;
        private IWebHostEnvironment Environment;
        private ErrorLoggingService _logger;

        public LessonsController(DefaultDBContext db, Util util, UserManager<AspNetUsers> userManager, IWebHostEnvironment _Environment, ErrorLoggingService logger)
        {
            this.db = db;
            this.util = util;
            _userManager = userManager;
            Environment = _Environment;
            _logger = logger;
        }

        public IActionResult Index()
        {
            QuestionViewModel model = new QuestionViewModel();
            SetupSelectLists(model);
            QuestionListing listing = new QuestionListing();
            listing.SubjectIdSelectList = model.SubjectIdSelectList;
            listing.QuestionTypeIdSelectList = model.QuestionTypeIdSelectList;
            return View(listing);
        }

        public void SetupSelectLists(QuestionViewModel model)
        {
            model.SubjectIdSelectList = (from t1 in db.Subjects
                                         orderby t1.Name
                                         select new SelectListItem
                                         {
                                             Text = t1.Name,
                                             Value = t1.Id,
                                             Selected = t1.Id == model.SubjectId ? true : false
                                         }).ToList();
            model.QuestionTypeIdSelectList = (from t1 in db.QuestionTypes
                                              orderby t1.Name
                                              select new SelectListItem
                                              {
                                                  Text = t1.Name,
                                                  Value = t1.Id,
                                                  Selected = t1.Id == model.QuestionTypeId ? true : false
                                              }).ToList();
            model.ActiveInactiveSelectList = util.GetActiveInactiveDropDown(model.IsActive);
        }


    }
}
