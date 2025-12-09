using ApplicationLayer.DTOs;
using DomainLayer.Entities;
using SharedLayer.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Validation
{
    public class LanguageCodeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if(value is not IEnumerable translations)
            {
                return new ValidationResult("Invalid translations data.");
            }

            var language = typeof(LanguageCode);
            var codes = language.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                            .Where(f => f.IsLiteral && !f.IsInitOnly)
                            .Select(f => f.GetRawConstantValue()?.ToString())
                            .ToList();

            var providedCodes = translations.Cast<object>().Select(x => x.GetType().GetProperty("LanguageCode")?.GetValue(x)?.ToString()).ToList();

            var missingCodes = codes.Where(code => !providedCodes.Contains(code)).ToList();
            if(missingCodes.Any())
            {
                return new ValidationResult($"Missing translations for language codes: {string.Join(", ", missingCodes)}");
            }

            return ValidationResult.Success;
        }
    }
}
