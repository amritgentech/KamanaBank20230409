using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BAL;
using Db.Model;
using ReconUi.Models;
using Db;
using System.Net;
using static BAL.ReconBAL;
using ReconUi.Model;
using Db.Enum;
using Helper.GlobalHelpers;
using ReconUi.Models.DigitalBanking;

namespace ReconUi.Controllers
{
    [Authorize]
    public class FileUploadController : BaseController
    {
        #region FileUpload
        // GET: FileUpload
        public ActionResult Index(UploadInitializeModel initializeModel)
        {
            string isReconActive = ReconBAL.IsReconActive();
            ViewBag.DrpSourceData = ReconBAL.GetSourceList();
            ViewBag.CheckReconActivate = isReconActive;
            ViewBag.UploadInitializeModel = initializeModel;
            if (isReconActive.Equals("True"))
            {
                //                ModelState.AddModelError("", "Please wait....Recon is ongoing.");
            }
            return View();
        }
        protected bool HasSubSource(int sourceId)
        {
            SubSource _SubSource = new SubSource();
            _SubSource = ReconBAL.GetSubSource(sourceId);
            if (_SubSource != null)
            {
                return true;
            }
            return false;
        }
        [HttpGet]
        public ActionResult GetSubSource(int? sourceId)
        {
            if (sourceId != null)
            {
                List<SelectListItem> SubSourceList = ReconBAL.GetSubSourceList(sourceId);
                return Json(SubSourceList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }
        protected bool HasSubChildSource(int subSourceId)
        {
            SubChildSource _SubChildSource = new SubChildSource();
            _SubChildSource = ReconBAL.GetSubChildSource(subSourceId);
            if (_SubChildSource != null)
            {
                return true;
            }
            return false;
        }
        [HttpGet]
        public ActionResult GetSubChildSource(int? subSourceId)
        {
            if (subSourceId != null)
            {
                List<SelectListItem> SubChildSourceList = ReconBAL.GetSubChildSourceList(subSourceId);
                return Json(SubChildSourceList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }
        [ActionName("UploadFiles")]
        [HttpPost]
        public ActionResult Upload(UploadModel uploadModel)
        {
            UploadInitializeModel sourceInitializeModel = new UploadInitializeModel();
            ViewBag.DrpSourceData = ReconBAL.GetSourceList();
            List<HttpPostedFileBase> totalfiles = new List<HttpPostedFileBase>();
            try
            {
                if (uploadModel.SubSourceName.Equals("Please Select Sub Source"))
                {
                    uploadModel.SubSourceName = string.Empty;
                }
                uploadModel.hasSubSource = true;
                if (!HasSubSource(uploadModel.SourceId))
                {
                    uploadModel.hasSubSource = false;
                    ModelState.Remove("SubSourceId");
                    uploadModel.SubSourceName = null;
                }
                if (!HasSubChildSource(uploadModel.SubSourceId))
                {
                    uploadModel.hasSubChildSource = false;
                    ModelState.Remove("SubChildSourceId");
                    uploadModel.SubChildSourceName = null;
                }
                ModelState.Remove("hasSubSource");
                ModelState.Remove("hasSubChildSource");

                String ROOT_FOLDER_LOCATION = ConfigurationManager.AppSettings["ROOT_FOLDER_LOCATION"].ToString();
                string subsourceName = string.Empty, subChildsourceName = string.Empty;

                sourceInitializeModel.SourceId = GetSourceIdByName(uploadModel.SourceName);
                if (!String.IsNullOrEmpty(uploadModel.SubSourceName))
                {
                    sourceInitializeModel.subSourceId = GetSubSourceIdByName(uploadModel.SubSourceName);
                    subsourceName = "\\" + uploadModel.SubSourceName;

                    if (uploadModel.SourceName.Equals("EJOURNAL") &&
                        (uploadModel.SubSourceName.Equals("DIEBOLD") || uploadModel.SubSourceName.Equals("NCR") ||
                         uploadModel.SubSourceName.Equals("WINCOR") || uploadModel.SubSourceName.Equals("VORTEX")))
                    {
                        ModelState.Remove("Files");
                    }
                }
                if (!String.IsNullOrEmpty(uploadModel.SubChildSourceName))
                {
                    sourceInitializeModel.subChildSourceId = uploadModel.SubChildSourceId;
                    subChildsourceName = "\\" + uploadModel.SubChildSourceName;
                }
                if (ModelState.IsValid)
                {
                    foreach (HttpPostedFileBase file in uploadModel.Files)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        if (uploadModel.SourceName.ToLower().Equals("visa"))
                        {
                            if (!fileName.Contains(uploadModel.SubSourceName))
                            {
                                ModelState.AddModelError("", "File Format Not Matched. Try again.");
                                return View("Index", uploadModel);
                            }
                        }
                    }

                    if (Request.Files.Count > 0)
                    {
                        if (uploadModel.SourceName.ToLower().Equals("visa"))
                        {
                            totalfiles =
                                uploadModel.Files.Where(
                                    x => x.ContentLength > 3240).ToList(); // filter use less file in case of visa

                            if (totalfiles.Count() <= 0)
                            {
                                ModelState.AddModelError("", "There is no file to parse or unused file uploaded..");
                                return View("Index", uploadModel);
                            }
                        }
                        else
                        {
                            totalfiles =
                                uploadModel.Files.ToList();
                        }

                        var taskId = 0;
                        //task started for parsing..
                        taskId = ExecuteReconParser(totalfiles.Count); //initialize reconparser.exe

                        int milliseconds = 2000;
                        Thread.Sleep(milliseconds); // wait program to initiate reconparser.exe

                        foreach (HttpPostedFileBase file in totalfiles)
                        {
                            if (uploadModel.SourceName.Equals("VISA"))
                            {
                                Thread.Sleep(1000);
                            }

                            //if procerss accidently closed then stop copying bulk files to the server..
                            var IsReconProcessAlive = Process.GetProcessById(taskId).HasExited;

                            if (IsReconProcessAlive)
                            {
                                break;
                            }

                            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var fExt = Path.GetExtension(file.FileName);

                            var fullPath = ROOT_FOLDER_LOCATION + uploadModel.SourceName + subsourceName +
                                           subChildsourceName + "\\" + fileName + "Append" +
                                           Guid.NewGuid() +
                                           DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss") + fExt;
                            file.SaveAs(fullPath);

                            try
                            {
                                SaveActivityLogData("FileUpload", "Upload", fileName + fExt + " To " + fullPath);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error Occured during file upload.", e.Message);
                            }
                        }

                        IsReconParserProcessActive(taskId);
                        uploadModel.CheckReconActivate = null;
                    }
                    return RedirectToAction("Index", sourceInitializeModel); //after upload file redirect to index with initial data..

                }
                return View("Index", uploadModel);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Folder/File format not matched..");
                GlobalHelper.StepLogging(e.Message + Environment.NewLine + e.InnerException);
                return View("Index", uploadModel);
            }
        }

        private static void IsReconParserProcessActive(int taskId)
        {
            for (int i = 0; i >= 0; i++) // just loop until reconparser.exe not completed..
            {
                var process = Process.GetProcessById(taskId);
                var conditions = process.HasExited;

                if (conditions)
                {
                    break;
                }
            }
        }

        [HttpPost]
        public JsonResult ResetReconActivateFlag()
        {
            ReconBAL.ResetReconActivateFlag();
            return Json(null);
        }
        public int ExecuteReconParser(int FileCount)
        {
            if (Process.GetProcessesByName("reconparser").Length > 0)
            {
                Process existingProcess = Process.GetProcessesByName("reconparser")[0];
                existingProcess.Kill();
            }
            var args = new string[2];
            var path = AppDomain.CurrentDomain.BaseDirectory + "ReconParserExecutable\\reconparser.exe";
            Process process = Process.Start(path, FileCount.ToString());
            int id = process.Id;
            Process tempProc = Process.GetProcessById(id);
            return id;
        }
        #endregion

        #region FileUploadedHistory
        [HttpGet]
        public ActionResult GetUploadHistoryBySourceId(int SourceId, int? subSourceId, int? subChildSourceId)
        {
            return View("~/Views/FileUpload/_FileUploadedHistory.cshtml", ReconBAL.GetUploadedFiles(SourceId, subSourceId, subChildSourceId));
        }

        [HttpGet]
        public JsonResult DeleteFileHistory(int fileId)
        {
            ReconBALDbOperation reconBalDbOperation = new ReconBALDbOperation();
            var filePath = ReconBAL.GetFileNameById(fileId);

            reconBalDbOperation.DeleteUploadedFileTransactions(fileId);  //DELETE ..

            ReconBAL.DeleteUploadedFile(fileId);  //DELETE ..

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewTransactions(int fileId)
        {
            ViewBag.HistoryFileName = GetFileName(fileId);
            var category = GetCategory(fileId);
            var subsource = GetSubSourceName(fileId);
            if (subsource.ToLower().Equals("nostro"))
            {
                var NostrotranList = GetListNostroTransaction(fileId);
                List<NostroTransactionViewModel> lstTransactionViewModels = new List<NostroTransactionViewModel>();
                foreach (var item in NostrotranList)
                {
                    NostroTransactionViewModel transactionViewModel = new NostroTransactionViewModel();
                    transactionViewModel.TransactionId = item.TransactionId;
                    transactionViewModel.TransactionDate = item.TransactionDate.ToString("yyyyMMdd");
                    transactionViewModel.ValueDate = item.ValueDate.ToString("yyyyMMdd");
                    transactionViewModel.TransactionAmount = item.TransactionAmount;
                    transactionViewModel.BalanceAmount = item.BalanceAmount;
                    transactionViewModel.Description = item.Description;
                    lstTransactionViewModels.Add(transactionViewModel);

                }
                return View("~/Views/FileUpload/_NostroViewTransactions.cshtml", lstTransactionViewModels);
            }
            else if (subsource.ToLower().Equals("mirror"))
            {
                var MirrortranList = GetListMirrorTransaction(fileId);
                List<MirrorCbsTransactionViewModel> lstTransactionViewModels = new List<MirrorCbsTransactionViewModel>();
                foreach (var item in MirrortranList)
                {
                    MirrorCbsTransactionViewModel transactionViewModel = new MirrorCbsTransactionViewModel();
                    transactionViewModel.TransactionDate = item.TransactionDate.ToString("yyyyMMdd");
                    transactionViewModel.Particulars = item.Particulars;
                    transactionViewModel.TransactionAmount = item.TransactionAmount;
                    transactionViewModel.DebitOrCredit = item.DebitOrCredit;
                    transactionViewModel.ReferenceNumber = item.ReferenceNumber;
                    transactionViewModel.Balance = item.Balance;
                    lstTransactionViewModels.Add(transactionViewModel);

                }
                return View("~/Views/FileUpload/_MirrorCbsViewTransactions.cshtml", lstTransactionViewModels);
            }
            else if (subsource.ToLower().Equals("cbs"))
            {
                var CbstranList = GetListCbsTransaction(fileId);
                List<MirrorCbsTransactionViewModel> lstTransactionViewModels = new List<MirrorCbsTransactionViewModel>();
                foreach (var item in CbstranList)
                {
                    MirrorCbsTransactionViewModel transactionViewModel = new MirrorCbsTransactionViewModel();
                    transactionViewModel.TransactionDate = item.TransactionDate.ToString("yyyyMMdd");
                    transactionViewModel.Particulars = item.Particulars;
                    transactionViewModel.TransactionAmount = item.TransactionAmount;
                    transactionViewModel.DebitOrCredit = item.DebitOrCredit;
                    transactionViewModel.ReferenceNumber = item.ReferenceNumber;
                    transactionViewModel.Balance = item.Balance;
                    lstTransactionViewModels.Add(transactionViewModel);
                }
                return View("~/Views/FileUpload/_MirrorCbsViewTransactions.cshtml", lstTransactionViewModels);
            }
            else if (subsource.ToLower().Equals("esewa"))
            {
                var tranList = GetListEsewaTransaction(fileId);
                List<EsewaTransactionViewModel> lstTransactionViewModels = new List<EsewaTransactionViewModel>();
                foreach (var ob in tranList)
                {
                    EsewaTransactionViewModel transactionViewModel = new EsewaTransactionViewModel();
                    transactionViewModel.EsewaId = ob.EsewaId;
                    transactionViewModel.TransactionDate = ob.TransactionDate.ToString("yyyyMMdd");
                    transactionViewModel.TransactionAmount = ob.TransactionAmount;
                    transactionViewModel.UniqueId = ob.UniqueId;
                    transactionViewModel.Status = ob.Status;
                    transactionViewModel.Remark = ob.Remark;

                    lstTransactionViewModels.Add(transactionViewModel);
                }
                return View("~/Views/FileUpload/_EsewaViewTransactions.cshtml", lstTransactionViewModels);
            }
            else if (subsource.ToLower().Equals("mobileibft"))
            {
                var tranList = GetListMobileIbftTransaction(fileId);
                List<MobileIbftTransactionViewModel> lstTransactionViewModels = new List<MobileIbftTransactionViewModel>();
                foreach (var ob in tranList)
                {
                    MobileIbftTransactionViewModel transactionViewModel = new MobileIbftTransactionViewModel();
                    transactionViewModel.Id = ob.Id;
                    transactionViewModel.TransactionDate = ob.TransactionDate.ToString("yyyyMMdd");
                    transactionViewModel.TransactionAmount = ob.TransactionAmount;
                    transactionViewModel.ChargeAmount = ob.ChargeAmount;
                    transactionViewModel.TransactionTraceId = ob.TransactionTraceId;
                    transactionViewModel.FonepayTransactionStatus = ob.FonepayTransactionStatus;

                    lstTransactionViewModels.Add(transactionViewModel);
                }
                return View("~/Views/FileUpload/_MobileIbftViewTransactions.cshtml", lstTransactionViewModels);
            }
            else if (subsource.ToLower().Equals("internetibft"))
            {
                var tranList = GetListInternetIbftTransaction(fileId);
                List<InternetIbftTransactionViewModel> lstTransactionViewModels = new List<InternetIbftTransactionViewModel>();
                foreach (var ob in tranList)
                {
                    InternetIbftTransactionViewModel transactionViewModel = new InternetIbftTransactionViewModel();
                    transactionViewModel.TransactionDate = ob.TransactionDate.ToString("yyyyMMdd");
                    transactionViewModel.TransactionAmount = ob.TransactionAmount;
                    transactionViewModel.ChargeAmount = ob.ChargeAmount;
                    transactionViewModel.OrginatingUniqueId = ob.OrginatingUniqueId;
                    transactionViewModel.TransactionStatus = ob.TransactionStatus;
                    transactionViewModel.TraceId = ob.TraceId;
                    lstTransactionViewModels.Add(transactionViewModel);
                }
                return View("~/Views/FileUpload/_InternetIbftViewTransactions.cshtml", lstTransactionViewModels);
            }
            else
            {
                var tranList = GetListTransaction(fileId);
                List<TransactionViewModel> lstTransactionViewModels = new List<TransactionViewModel>();
                foreach (var ob in tranList)
                {
                    TransactionViewModel transactionViewModel = new TransactionViewModel();
                    transactionViewModel.TransactionDate = ob.TransactionDate.ToString("yyyyMMdd");
                    transactionViewModel.CardNo = ob.CardNo;
                    transactionViewModel.TraceNo = ob.TraceNo;
                    transactionViewModel.AuthCode = ob.AuthCode;
                    transactionViewModel.ResponseCode = ob.ResponseCode;
                    transactionViewModel.ResponseCodeDescription = ob.ResponseCodeDescription;
                    transactionViewModel.ReferenceNo = ob.ReferenceNo;
                    transactionViewModel.TerminalId = ob.TerminalId;
                    transactionViewModel.TransactionAmount = ob.TransactionAmount;
                    transactionViewModel.AccountNo = ob.AccountNo;
                    transactionViewModel.Currency = ob.Currency;
                    transactionViewModel.CBSValueDate = ob.CBSValueDate;
                    transactionViewModel.AdviseDate = ob.AdviseDate;
                    transactionViewModel.AdviseDate = ob.AdviseDate;
                    transactionViewModel.TransactionStatus = Enum.GetName(typeof(TransactionStatus), ob.TransactionStatus);

                    lstTransactionViewModels.Add(transactionViewModel);
                }
                return View("~/Views/FileUpload/_ViewTransactions.cshtml", lstTransactionViewModels);
            }
            return View("Index");
        }
        #endregion
    }
}