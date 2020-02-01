using System.ComponentModel.DataAnnotations;
using Common.QMessageModels.RequestMessages;
using Engine.Accounting.Resources;
using Engine.Common.Resources;

namespace Engine.Accounting.Login
{
    public class LoginRequest 
    {
        [Required(ErrorMessageResourceName = "FieldValueIncorrect",ErrorMessageResourceType = typeof(GlobalRes))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "FieldValueIncorrect", ErrorMessageResourceType = typeof(GlobalRes))]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}