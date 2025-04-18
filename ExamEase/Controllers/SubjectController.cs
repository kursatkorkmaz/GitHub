using ExamEase.Data;
using ExamEase.Models;
using ExamEase.Resources;
using ExamEase.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.IO;

namespace ExamEase.Controllers
{
    [Authorize(Roles = "System Admin")]
    public class SubjectController : Controller
    {
        private DefaultDBContext db;
        private Util util;
        private readonly UserManager<AspNetUsers> _userManager;
        private ErrorLoggingService _logger;

        public SubjectController(DefaultDBContext db, Util util, UserManager<AspNetUsers> userManager, ErrorLoggingService logger)
        {
            this.db = db;
            this.util = util;
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewRecord(string Id)
        {
            SubjectViewModel model = new SubjectViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "View");
            }
            return View(model);
        }

        public IActionResult Edit(string Id)
        {
            SubjectViewModel model = new SubjectViewModel();
            if (Id != null)
            {
                model = GetViewModel(Id, "Edit");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(SubjectViewModel model)
        {
            try
            {
                ValidateModel(model);

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                SaveRecord(model);
                TempData["NotifySuccess"] = Resource.RecordSavedSuccessfully;
            }
            catch (Exception ex)
            {
                TempData["NotifyFailed"] = Resource.FailedExceptionError;
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return RedirectToAction("index");
        }

        public IActionResult Delete(string Id)
        {
            try
            {
                if (Id != null)
                {
                    Subject cl = db.Subjects.Where(a => a.Id == Id).FirstOrDefault();
                    if (cl != null)
                    {
                        db.Subjects.Remove(cl);
                        db.SaveChanges();
                    }
                }
                TempData["NotifySuccess"] = Resource.RecordDeletedSuccessfully;
            }
            catch (Exception ex)
            {
                TempData["NotifyFailed"] = Resource.FailedExceptionError;
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return RedirectToAction("index");
        }

        public async Task<IActionResult> GetPartialViewListing(string sort, string search, int? pg, int? size)
        {
            try
            {
                List<ColumnHeader> headers = new List<ColumnHeader>();
                if (string.IsNullOrEmpty(sort))
                {
                    sort = SubjectListConfig.DefaultSortOrder;
                }
                headers = ListUtil.GetColumnHeaders(SubjectListConfig.DefaultColumnHeaders, sort);
                var list = ReadSubjects();
                string searchMessage = SubjectListConfig.SearchMessage;
                list = SubjectListConfig.PerformSearch(list, search);
                list = SubjectListConfig.PerformSort(list, sort);
                ViewData["CurrentSort"] = sort;
                ViewData["CurrentPage"] = pg ?? 1;
                ViewData["CurrentSearch"] = search;
                int? total = list.Count();
                int? defaultSize = SubjectListConfig.DefaultPageSize;
                size = size == 0 || size == null ? (defaultSize != -1 ? defaultSize : total) : size == -1 ? total : size;
                ViewData["CurrentSize"] = size;
                PaginatedList<SubjectViewModel> result = await PaginatedList<SubjectViewModel>.CreateAsync(list, pg ?? 1, size.Value, total.Value, headers, searchMessage);
                return PartialView("~/Views/Subject/_MainList.cshtml", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            }
            return PartialView("~/Views/Shared/Error.cshtml", null);
        }

        public IQueryable<SubjectViewModel> ReadSubjects()
        {
            var list = (from t1 in db.Subjects
                        select new SubjectViewModel
                        {
                            Id = t1.Id,
                            Name = t1.Name
                        });
            return list;
        }

        public SubjectViewModel GetViewModel(string Id, string type)
        {
            SubjectViewModel model = new SubjectViewModel();
            Subject cl = db.Subjects.Where(a => a.Id == Id).FirstOrDefault();
            model.Id = cl.Id;
            model.Name = cl.Name;
            if (type == "View")
            {
                model.CreatedAndModified = util.GetCreatedAndModified(cl.CreatedBy, cl.IsoUtcCreatedOn, cl.ModifiedBy, cl.IsoUtcModifiedOn);
            }
            return model;
        }

        public void ValidateModel(SubjectViewModel model)
        {
            if (model != null)
            {
                bool duplicated = false;
                if (model.Id != null)
                {
                    duplicated = db.Subjects.Where(a => a.Name == model.Name && a.Id != model.Id).Any();
                }
                else
                {
                    duplicated = db.Subjects.Where(a => a.Name == model.Name).Select(a => a.Id).Any();
                }
                if (duplicated == true)
                {
                    ModelState.AddModelError("Name", Resource.ThisIsADuplicatedValue);
                }
            }
        }

        public void SaveRecord(SubjectViewModel model)
        {
            if (model != null)
            {
                if (model.Id == null)
                {
                    Subject cl = new Subject();
                    cl.Id = Guid.NewGuid().ToString();
                    cl.Name = model.Name;
                    cl.CreatedBy = _userManager.GetUserId(User);
                    cl.CreatedOn = util.GetSystemTimeZoneDateTimeNow();
                    cl.IsoUtcCreatedOn = util.GetIsoUtcNow();
                    db.Subjects.Add(cl);
                    db.SaveChanges();
                }
                else
                {
                    Subject cl = db.Subjects.Where(a => a.Id == model.Id).FirstOrDefault();
                    cl.Name = model.Name;
                    cl.ModifiedBy = _userManager.GetUserId(User);
                    cl.ModifiedOn = util.GetSystemTimeZoneDateTimeNow();
                    cl.IsoUtcModifiedOn = util.GetIsoUtcNow();
                    db.Entry(cl).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
                if (util != null)
                {
                    util.Dispose();
                    util = null;
                }
            }

            base.Dispose(disposing);
        }


       


        public IActionResult Import()
        {
            ImportFromExcel importFromExcel = new ImportFromExcel();
            return View(importFromExcel);
        }


        [HttpPost]
        public IActionResult ImportSubject(ImportFromExcel model, IFormFile File)
        {
            //try
            //{
            List<string> errors = new List<string>();
            List<ImportFromExcelError> errorsList = new List<ImportFromExcelError>();

            SubjectViewModelImport subjectViewModel = new SubjectViewModelImport();

            int successCount = 0;
            int dtRowsCount = 0;
            List<string> columns = new List<string>();

            using (var memoryStream = new MemoryStream())
            {
                File.CopyTo(memoryStream);
                using (var reader = ExcelReaderFactory.CreateReader(memoryStream))
                {
                    var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    var dt = ds.Tables[0];

                    foreach (var col in dt.Columns.Cast<DataColumn>())
                    {
                        col.ColumnName = col.ColumnName.Replace("*", "");
                        columns.Add(col.ColumnName);
                    }
                    dtRowsCount = dt.Rows.Count;

                    errors = util.ValidateColumns(columns, new List<string>
                    {
                        "Name","ExcellKurID",
                        "ExcellTestID"
                    });

                    //if all columns validated
                    if (errors.Count == 0)
                    {
                        for (int i = 0; i < dtRowsCount; i++)
                        {
                            //try
                            //{
                            //decimal markValue = 0;
                            string Name = dt.Rows[i].Field<string>("Name");
                            double ExcellKurID = dt.Rows[i].Field<double>("ExcellKurID");
                            double ExcellTestID = dt.Rows[i].Field<double>("ExcellTestID");



                            //string subjectId = db.Subjects.Where(a => a.Name == subject).Select(a => a.Id).FirstOrDefault();
                            //string questionTypeId = db.QuestionTypes.Where(a => a.Name == questionType).Select(a => a.Id).FirstOrDefault();

                            subjectViewModel.Name = Name;
                            subjectViewModel.ExcellKurID = Convert.ToInt32(ExcellKurID);
                            subjectViewModel.ExcellTestID = Convert.ToInt32(ExcellTestID);
                            //subjectViewModel.AnswerOrder = Convert.ToInt32(AnswerOrder);
                            //errors = ValidateImportRowanswer(subjectViewModel);

                            //if (errors.Count() > 0)
                            //{
                            //    ImportFromExcelError importFromExcelError = new ImportFromExcelError();
                            //    importFromExcelError.Row = $"At Row {i + 2}";
                            //    importFromExcelError.Errors = errors;
                            //    errorsList.Add(importFromExcelError);
                            //    continue;
                            //}

                            Subject subject = new Subject();
                            subject.Id = Guid.NewGuid().ToString();
                            subject.Name = Name;
                            subject.CreatedBy = _userManager.GetUserId(User);
                            subject.CreatedOn = util.GetSystemTimeZoneDateTimeNow();
                            subject.ModifiedBy = "634b8ece-0ba0-4c3d-ab96-c671c9b4d468";
                            subject.ModifiedOn = util.GetSystemTimeZoneDateTimeNow(); ;
                            subject.IsoUtcCreatedOn = util.GetSystemTimeZoneDateTimeNow().ToString();
                            subject.IsoUtcModifiedOn= util.GetSystemTimeZoneDateTimeNow().ToString();

                            subject.SubjectOrder = 1;
                            subject.ExcellKurID = Convert.ToInt32(ExcellKurID);
                            subject.ExcellTestID = Convert.ToInt32(ExcellTestID);
                       

                            //question.Questionexcellid = Convert.ToInt32(Questionexcellid);
                            db.Subjects.Add(subject);
                            db.SaveChanges();
                            successCount++;
                            ModelState.Clear();
                            //}
                            //catch (Exception ex)
                            //{
                            //    errors.Add($"{ex.Message} - Row: {i + 2}");
                            //}
                        }
                    }
                    else
                    {
                        ImportFromExcelError importFromExcelError = new ImportFromExcelError();
                        importFromExcelError.Row = Resource.InvalidTemplate;
                        importFromExcelError.Errors = errors;
                        errorsList.Add(importFromExcelError);
                    }
                }
            }

            if (errorsList.Count > 0)
            {
                model.ErrorList = errorsList;
                model.UploadResult = $"{successCount} {Resource.outof} {dtRowsCount} {Resource.recordsuploaded}";
                return View("import", model);
            }
            TempData["NotifySuccess"] = Resource.RecordsImportedSuccessfully;
            //}
            //catch (Exception ex)
            //{
            //    TempData["NotifyFailed"] = Resource.FailedExceptionError;
            //    _logger.LogError(ex, $"{GetType().Name} Controller - {MethodBase.GetCurrentMethod().Name} Method");
            //}
            return RedirectToAction("index");
        }






    }
}
