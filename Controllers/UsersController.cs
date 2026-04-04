using LearningPlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace LearningPlatform.Controllers
{
    [ApiController]
    [Route("auth")]
    [Authorize]
    public class UsersController(JwtOptions jwtOptions, UserManager<ApplicationUser> userManager) : ControllerBase
    {

        //public ActionResult<string> AuthMethod(AuthenticationRequest request)
        //{
        //    var tokenHanler = new JwtSecurityTokenHandler();
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Issuer = jwtOptions.Issuer,
        //        Audience = jwtOptions.Audience,
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
        //            ,SecurityAlgorithms.HmacSha256),
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //            new Claim(ClaimTypes.Name, request.Username),
        //            new Claim(ClaimTypes.Email, request.Email)
        //        }),
        //        Expires = DateTime.UtcNow.AddHours(1),
        //    };
        //    var token = tokenHanler.CreateToken(tokenDescriptor);
        //    var accessToken = tokenHanler.WriteToken(token);

        //    return Ok(accessToken);
        //}
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterNewUser([FromBody] RegisterUserDto userDto)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = userDto.Username,
                    Email = userDto.Email
                };
                IdentityResult result = await userManager.CreateAsync(user, userDto.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, userDto.Role.ToString());
                    return Ok("User created successfully");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return BadRequest(ModelState);
                }
            }
            else
                return BadRequest(ModelState);
        }
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto userDto)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByNameAsync(userDto.UserName);
                if (user != null)
                {
                    if(await userManager.CheckPasswordAsync(user, userDto.Password))
                    {
                        var roles = await userManager.GetRolesAsync(user);
                        var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new Claim(ClaimTypes.Name, user.UserName?? ""),
                                new Claim(ClaimTypes.Email, user.Email?? "")
                            };

                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }
                        var tokenHanler = new JwtSecurityTokenHandler();
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Issuer = jwtOptions.Issuer,
                            Audience = jwtOptions.Audience,
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
                                , SecurityAlgorithms.HmacSha256),
                            Subject = new ClaimsIdentity(claims),
                            Expires = DateTime.UtcNow.AddMinutes(jwtOptions.Lifetime)
                        };
                        var token = tokenHanler.CreateToken(tokenDescriptor);
                        var accessToken = tokenHanler.WriteToken(token);

                        //return Ok(accessToken);
                        return Ok(new
                        {
                            token = accessToken,
                            expiresIn = jwtOptions.Lifetime * 60
                        });
                    }
                    else
                    {
                        return Unauthorized("Invalid username or password");
                    }
                }
                else
                {
                    return Unauthorized("Invalid username or password");
                }

            }
            return BadRequest(ModelState);
        }
    }
}
