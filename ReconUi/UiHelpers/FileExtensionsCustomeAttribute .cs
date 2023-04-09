using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ReconUi.UiHelpers
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FileExtensionsCustomeAttribute : ValidationAttribute
    {
        private List<string> AllowedExtensions { get; set; }

        public FileExtensionsCustomeAttribute(string fileExtensions)
        {
            AllowedExtensions = fileExtensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public override bool IsValid(object value)
        {
            HttpPostedFileBase file = value as HttpPostedFileBase;

            if (file != null)
            {
                var fileName = file.FileName;

                return AllowedExtensions.Any(y => fileName.EndsWith(y));
            }

            return false;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FileTypeAttribute : ValidationAttribute, IClientValidatable
    {
        private const string _DefaultErrorMessage = "Only the following file types are allowed: {0}";
        private IEnumerable<string> _ValidTypes { get; set; }

        public FileTypeAttribute(string validTypes)
        {
//            _ValidTypes = validTypes.Split(',').Select(s => s.Trim().ToLower());
            _ValidTypes = validTypes.Split(',').Select(s => s.Trim());
            ErrorMessage = string.Format(_DefaultErrorMessage, string.Join(" or ", _ValidTypes));
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IEnumerable<HttpPostedFileBase> files = value as IEnumerable<HttpPostedFileBase>;
            if (files != null)
            {
                foreach (HttpPostedFileBase file in files)
                {
                    if (file != null && !_ValidTypes.Any(e => file.FileName.EndsWith(e)))
                    {
                        return new ValidationResult(ErrorMessageString);
                    }
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ValidationType = "filetype",
                ErrorMessage = ErrorMessageString
            };
            rule.ValidationParameters.Add("validtypes", string.Join(",", _ValidTypes));
            yield return rule;
        }
    }
}