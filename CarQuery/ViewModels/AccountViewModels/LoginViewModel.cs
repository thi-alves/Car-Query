using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CarQuery.ViewModels.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "É necessário informar o email")]
        [EmailAddress(ErrorMessage = "Informe um email válido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe a senha")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }
    }
}
