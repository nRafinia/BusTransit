using System.ComponentModel.DataAnnotations;
using Common.Resources;

namespace Common.Models
{
    public enum GlobalStatusCode
    {
        [Display(Name = "StatusCode_200", ResourceType = typeof(CommonRes))]
        Ok = 200,

        [Display(Name = "StatusCode_400", ResourceType = typeof(CommonRes))]
        ParameterInvalid = 400,

        [Display(Name = "StatusCode_403", ResourceType = typeof(CommonRes))]
        AccessDenied = 403,

        [Display(Name = "StatusCode_404", ResourceType = typeof(CommonRes))]
        NotFound = 404,

        [Display(Name = "StatusCode_500", ResourceType = typeof(CommonRes))]
        InternalServerError = 500,

    }
}