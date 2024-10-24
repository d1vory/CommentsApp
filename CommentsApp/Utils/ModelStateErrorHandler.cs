using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace PhotoRater.Utils;

public static class ModelStateErrorHandler
{
    public static Dictionary<string, string> GetModelErrors(this ModelStateDictionary errDictionary)
    {
        var errors = new Dictionary<string, string>();
        foreach (var item in errDictionary.Where(k => k.Value.Errors.Count > 0))
        {
            var er = string.Join(", ", item.Value.Errors.Select(e => e.ErrorMessage).ToArray());
            errors.Add(item.Key, er);
        }
        return errors;
    }

    public static string StringifyModelErrors(this ModelStateDictionary errDictionary)
    {
        var errorsBuilder = new StringBuilder();
        var errors = errDictionary.GetModelErrors();
        foreach (var item in errors)
        {
            errorsBuilder.AppendFormat($"{item.Key}: {item.Value}");
        }
        return errorsBuilder.ToString();
    }
    
}