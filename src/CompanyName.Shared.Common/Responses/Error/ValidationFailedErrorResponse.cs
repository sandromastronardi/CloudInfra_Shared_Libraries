using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CompanyName.Shared.Common.Responses.Error
{
    public class ValidationFailedErrorResponse : BadRequestErrorResponse
    {
        public ValidationFailedErrorResponse(IEnumerable<ValidationResult> validationResults) : this(null, validationResults){}
        public ValidationFailedErrorResponse(string errorMessage, IEnumerable<ValidationResult> validationResults)
            : base(validationResults.SelectMany(result => result.MemberNames.Any()?
            result.MemberNames.Select(memberName => Notification.Error(string.IsNullOrEmpty(memberName)?result.ErrorMessage: $"{memberName}: {result.ErrorMessage}")):
            new[] { Notification.Error(result.ErrorMessage) }
                  ))
        { 
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                this.StatusDescription = errorMessage;
            }
        }
    }
}