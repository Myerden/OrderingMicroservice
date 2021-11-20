using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerService.Test
{
    public static class Extensions
    {
        //https://stackoverflow.com/a/49393360/4133449
        public static void ValidateModel<T>(this ControllerBase controller, T model)
        {
            if (model == null) return;

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, context, results, true))
            {
                controller.ModelState.Clear();
                foreach (ValidationResult result in results)
                {
                    var key = result.MemberNames.FirstOrDefault() ?? "";
                    controller.ModelState.AddModelError(key, result.ErrorMessage);
                }
            }
        }
    }
}
