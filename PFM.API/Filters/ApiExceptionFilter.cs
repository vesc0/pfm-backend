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
                case ValidationException fv:
                    status = StatusCodes.Status400BadRequest;

                    // build array of { tag, error, message }
                    var errorItems = fv.Errors.Select(f => new
                    {
                        tag = KebabCaseNamingPolicy.Instance.ConvertName(f.PropertyName),
                        error = f.ErrorCode,
                        message = f.ErrorMessage
                    })
                    .ToArray();

                    body = new { errors = errorItems };
                    break;

                case DomainException de:
                    status = 440; // business‐policy error
                    body = new
                    {
                        errors = new[] {
                            new {
                                tag     = (string?)null,
                                error   = "domain-exception",
                                message = de.Message
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
