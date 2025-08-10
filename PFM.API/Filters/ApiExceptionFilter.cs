using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PFM.API.Serialization;
using PFM.Domain.Exceptions;

namespace PFM.API.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var ex = context.Exception;
            object body;
            int status;

            switch (ex)
            {
                // 400 — validation (FluentValidation or manually thrown)
                case ValidationException fv:
                    status = StatusCodes.Status400BadRequest;

                    var errorItems = fv.Errors.Select(f => new
                    {
                        tag     = string.IsNullOrWhiteSpace(f.PropertyName) ? null
                                  : KebabCaseNamingPolicy.Instance.ConvertName(f.PropertyName),
                        error   = "validation-error",
                        message = f.ErrorMessage
                    }).ToArray();

                    body = new { errors = errorItems.Length > 0 ? errorItems : new[] {
                        new { tag = (string?)null, error = "validation-error", message = fv.Message }
                    }};
                    break;

                // 440 — business policy with machine code from the spec
                case BusinessRuleException bre:
                    status = 440;
                    body = new
                    {
                        errors = new[] {
                            new {
                                tag     = bre.Tag,
                                error   = bre.Code,
                                message = bre.Message
                            }
                        }
                    };
                    break;

                default:
                    status = StatusCodes.Status500InternalServerError;
                    body = new
                    {
                        errors = new[] {
                            new {
                                tag     = (string?)null,
                                error   = "internal-error",
                                message = "An unexpected error occurred."
                            }
                        }
                    };
                    break;
            }

            context.Result = new ObjectResult(body) { StatusCode = status };
            context.ExceptionHandled = true;
        }
    }
}
