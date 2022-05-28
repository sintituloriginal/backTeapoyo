using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Collections.ObjectModel;
using System.Threading;
using System;

namespace proyecto.Models
{
  
    // Para agregar datos de perfil al usuario, agregue más propiedades a la clase ApplicationUser. Para obtener más información, visite http://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public string RUT { get; set; }
        public string SEXO { get; set; }
        public string TELEFONO { get; set; }
        public string CELULAR { get; set; }
        public string FECHA_NACIMIENTO { get; set; }
        public string ESTADO { get; set; }
        public string CORPORATIVO { get; set; }
        public string GROUP_ID { get; set; }
        public string INTENTOS { get; set; }
        public string FULLNAME { get; set; }
        public string AVATAR { get; set; }
        public string SEGUNDO_NOMBRE { get; set; }
        public string APELLIDO_MATERNO { get; set; }
        public string APELLIDO_PATERNO { get; set; }
        public string DIRECCION { get; set; }
        public string CARGO { get; set; }
        public DateTime FECHA_CADUCIDAD { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Tenga en cuenta que el valor de authenticationType debe coincidir con el definido en CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Agregar aquí notificaciones personalizadas de usuario
            // Add more custom claims here if you want. 
            var claims = new Collection<Claim>
            {
                new Claim("group_id",this.GROUP_ID),
                new Claim("user_id",this.Id),
                new Claim("username",this.UserName),
                new Claim("fullname",this.FULLNAME),
            };
            userIdentity.AddClaims(claims);
            var principal = new ClaimsPrincipal(userIdentity);
            // Set current principal 
            Thread.CurrentPrincipal = principal;
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            //: base("SGI", throwIfV1Schema: false) // Local
            //: base("SGI_QA", throwIfV1Schema: false) // QA - LFE
            : base("base_datos3", throwIfV1Schema: false) // PROD - LFE
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("dbo");//tutelkan
            //modelBuilder.Entity<IdentityUser>().ToTable("WEB_USERS").Property(p => p.Id).HasColumnName("USER_ID");
            //modelBuilder.Entity<IdentityUser>().ToTable("WEB_USERS").Property(p => p.Email).HasColumnName("EMAIL");
            //modelBuilder.Entity<IdentityUser>().ToTable("WEB_USERS").Property(p => p.EmailConfirmed).HasColumnName("EMAILCONFIRMED");
            //modelBuilder.Entity<IdentityUser>().ToTable("WEB_USERS").Property(p => p.PasswordHash).HasColumnName("PASSWORDHASH");
            //modelBuilder.Entity<IdentityUser>().ToTable("WEB_USERS").Property(p => p.SecurityStamp).HasColumnName("SECURITYSTAMP");
            //modelBuilder.Entity<IdentityUser>().ToTable("WEB_USERS").Property(p => p.PhoneNumber).HasColumnName("PHONENUMBER");
            //modelBuilder.Entity<IdentityUser>().ToTable("WEB_USERS").Property(p => p.PhoneNumberConfirmed).HasColumnName("PHONENUMBERCONFIRMED");
            //modelBuilder.Entity<IdentityUser>().ToTable("WEB_USERS").Property(p => p.TwoFactorEnabled).HasColumnName("TWOFACTORENABLED");
            //modelBuilder.Entity<IdentityUser>().ToTable("WEB_USERS").Property(p => p.LockoutEndDateUtc).HasColumnName("LOCKOUTENDDATEUTC");
            //modelBuilder.Entity<IdentityUser>().ToTable("WEB_USERS").Property(p => p.LockoutEnabled).HasColumnName("LOCKOUTENABLED");
            //modelBuilder.Entity<IdentityUser>().ToTable("WEB_USERS").Property(p => p.AccessFailedCount).HasColumnName("ACCESSFAILEDCOUNT");
            // modelBuilder.Entity<IdentityUser>().ToTable("WEB_USERS").Property(p => p.UserName).HasColumnName("USERNAME");

            modelBuilder.Entity<ApplicationUser>().ToTable("WEB_USERS").Property(p => p.UserName).HasColumnName("USERNAME");
            modelBuilder.Entity<ApplicationUser>().ToTable("WEB_USERS").Property(p => p.Email).HasColumnName("EMAIL");
            modelBuilder.Entity<ApplicationUser>().ToTable("web_users").Property(p => p.Id).HasColumnName("USER_ID");
            modelBuilder.Entity<ApplicationUser>().ToTable("WEB_USERS").Property(p => p.EmailConfirmed).HasColumnName("EMAILCONFIRMED");
            modelBuilder.Entity<ApplicationUser>().ToTable("WEB_USERS").Property(p => p.PasswordHash).HasColumnName("PASSWORDHASH");
            modelBuilder.Entity<ApplicationUser>().ToTable("WEB_USERS").Property(p => p.SecurityStamp).HasColumnName("SECURITYSTAMP");
            modelBuilder.Entity<ApplicationUser>().ToTable("WEB_USERS").Property(p => p.PhoneNumber).HasColumnName("PHONENUMBER");
            modelBuilder.Entity<ApplicationUser>().ToTable("WEB_USERS").Property(p => p.PhoneNumberConfirmed).HasColumnName("PHONENUMBERCONFIRMED");
            modelBuilder.Entity<ApplicationUser>().ToTable("WEB_USERS").Property(p => p.TwoFactorEnabled).HasColumnName("TWOFACTORENABLED");
            modelBuilder.Entity<ApplicationUser>().ToTable("WEB_USERS").Property(p => p.LockoutEndDateUtc).HasColumnName("LOCKOUTENDDATEUTC");
            modelBuilder.Entity<ApplicationUser>().ToTable("WEB_USERS").Property(p => p.LockoutEnabled).HasColumnName("LOCKOUTENABLED");
            modelBuilder.Entity<ApplicationUser>().ToTable("WEB_USERS").Property(p => p.AccessFailedCount).HasColumnName("ACCESSFAILEDCOUNT");         
            modelBuilder.Entity<ApplicationUser>().ToTable("WEB_USERS").Property(p => p.FECHA_CADUCIDAD).HasColumnName("FECHA_CADUCIDAD");         
            //modelBuilder.Entity<ApplicationUser>().ToTable("web_users");
            modelBuilder.Entity<IdentityUserRole>().ToTable("web_users_roles");
            
            modelBuilder.Entity<IdentityUserClaim>().ToTable("web_users_claims");
            modelBuilder.Entity<IdentityRole>().ToTable("web_roles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("web_users_logins");

            
            modelBuilder.Properties<string>().Configure(x => x.HasColumnType("nvarchar"));


        }

    }
}