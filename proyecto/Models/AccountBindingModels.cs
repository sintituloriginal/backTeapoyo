using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyecto.Models
{
    // Modelos usados como parámetros para las acciones de AccountController.

    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = "Token de acceso externo")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar la nueva contraseña")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }



        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string RUT { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string SEXO { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string TELEFONO { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string CELULAR { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string FECHA_NACIMIENTO { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string ESTADO { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string CORPORATIVO { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string GROUP_ID { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string INTENTOS { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string FULLNAME { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string AVATAR { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string SEGUNDO_NOMBRE { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string APELLIDO_MATERNO { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string APELLIDO_PATERNO { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string DIRECCION { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string CARGO { get; set; }

        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string EMPRESA { get; set; }

        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string USERNAME { get; set; }

        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string EMPRESA_DEFECTO { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string area { get; set; }
        
        public string areas { get; set; }

        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string ID { get; set; }

        [Column(TypeName = "VARCHAR2")]
        [StringLength(500)]
        public string token { get; set; }
    }

    public class RegisterBindingModel
    {
        
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        
        [Column(TypeName = "VARCHAR2")]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        [Display(Name = "Confirmar contraseña")]
        //[Compare("Password", ErrorMessage = "La contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string RUT { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string SEXO { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string TELEFONO { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string CELULAR { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string FECHA_NACIMIENTO { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string ESTADO { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string CORPORATIVO { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string GROUP_ID { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string INTENTOS { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string FULLNAME { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string AVATAR { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string SEGUNDO_NOMBRE { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string APELLIDO_MATERNO { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string APELLIDO_PATERNO { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string DIRECCION { get; set; }
        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string CARGO { get; set; }

        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string EMPRESA { get; set; }

        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string USERNAME { get; set; }

        [Column(TypeName = "VARCHAR2")]
        [StringLength(250)]
        public string EMPRESA_DEFECTO { get; set; }
        public string areas { get; set; }
    }

    public class RegisterExternalBindingModel
    {
       
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }

    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Proveedor de inicio de sesión")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Clave de proveedor")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "El número de caracteres de {0} debe ser al menos {2}.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar la nueva contraseña")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la contraseña de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}
