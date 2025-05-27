using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CarQuery.ViewModels.AccountViewModels
{
    public class EditUserViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Informe um nome")]
        [MinLength(3, ErrorMessage = "O nome deve ter no mínimo {1} caracteres")]
        [MaxLength(25, ErrorMessage = "O nome não deve ultrapassar {1} caracteres")]
        [Display(Name = "Nome de usuário")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "É necessário informar o email")]
        [EmailAddress(ErrorMessage = "Informe um email válido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nova senha")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nova senha")]
        [Compare("NewPassword", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmNewPassword { get; set; }

        public EditUserViewModel() { }
        public EditUserViewModel(IdentityUser user)
        {
            UserId = user.Id;
            UserName = user.UserName;
            Email = user.Email;
        }
    }
}
