using Lux_Lens.DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LuxLens.Api.Models;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LuxLens.Api.Controllers.AuthController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LensDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;  // Agrega esta línea

        public AuthController(LensDbContext dbContext, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _signInManager = signInManager;
            _userManager = userManager;  // Inyecta el administrador de usuarios
            _roleManager = roleManager;
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);

                if (signInResult.Succeeded)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, model.Email),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        // Puedes agregar propiedades de autenticación si es necesario
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    return Ok("Inicio de sesión exitoso");
                }
                else
                {
                    return BadRequest("Credenciales inválidas");
                }
            }
            return BadRequest(ModelState);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    // Agrega más propiedades de usuario según sea necesario
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Verifica si el rol existe en la base de datos
                    var roleExists = await _roleManager.RoleExistsAsync(model.UserType);

                    if (!roleExists)
                    {
                        // Si el rol no existe, crea el rol
                        await _roleManager.CreateAsync(new IdentityRole("ADMIN"));
                        await _roleManager.CreateAsync(new IdentityRole("USER"));
                        await _roleManager.CreateAsync(new IdentityRole("GUEST"));
                    }

                    // Asigna el rol al usuario recién creado
                    await _userManager.AddToRoleAsync(user, model.UserType);

                    return Ok("Usuario registrado exitosamente");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok("Cierre de sesión exitoso");
        }
    }
}
