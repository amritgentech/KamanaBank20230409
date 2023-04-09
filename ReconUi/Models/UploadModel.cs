using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using ReconUi.UiHelpers;

namespace ReconUi.Models
{
    public class UploadModel
    {
        public string drpSource { get; set; }
        public string drpSubSource { get; set; }
        public string SourceName { get; set; }
        public string SubSourceName { get; set; }
        public string SubChildSourceName { get; set; }

        [Required(ErrorMessage = "Please select  Source")]
        public int SourceId { get; set; }

        [Required(ErrorMessage = "Please select Sub Source")]
        public int SubSourceId { get; set; }
        [Required(ErrorMessage = "Please select Sub Child Source")]
        public int SubChildSourceId { get; set; }
        public bool hasSubSource { get; set; }
        public string SubSourceIdVal { get; set; }
        public bool hasSubChildSource { get; set; }
        public string SubChildSourceIdVal { get; set; }

        [Display(Name = "Browse File")]
//        [Required(ErrorMessage = "Please upload  file."), FileExtensionsCustome("xlsx,xls,txt,jrn,pdf,txt,TXT", ErrorMessage = "File is not in correct format.")]
        [Required(ErrorMessage = "Please upload  file."),FileType("xlsx,xls,txt,jrn,pdf,txt,TXT", ErrorMessage = "File is not in correct format.")]
        public HttpPostedFileBase[] Files { get; set; }

//        public string FileName
//        {
//            get
//            {
//                if (Files != null)
//                    return Files[Files.Length].FileName;
//                else
//                    return String.Empty;
//            }
//        }
        public bool? CheckReconActivate { get; set; }
    }
}