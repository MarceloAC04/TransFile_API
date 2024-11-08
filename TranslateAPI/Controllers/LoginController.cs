using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TranslateAPI.Domains;
using TranslateAPI.Services;
using TranslateAPI.ViewModels;

namespace TranslateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;

        public LoginController(MongoDbService mongoDbService)
        {
            _users = mongoDbService.GetDatabase.GetCollection<User>("user");
        }

        [HttpPost]
        public async Task<ActionResult<User>> Login(Login user)
        {
            try
            {
                //busca usuário por email e senha 
                User userSearch = await _users.Find(p => p.Email == user.Email).FirstOrDefaultAsync();

                //caso não encontre
                if (userSearch == null || !Criptografia.HashComparer(user.Password!, userSearch.Password!))
                {
                    //retorna 401 - sem autorização
                    return StatusCode(401, "Email ou senha inválidos!");
                }


                //caso encontre, prossegue para a criação do token

                //informações que serão fornecidas no token
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, userSearch.Email!),
                    new Claim(JwtRegisteredClaimNames.Name,userSearch.Name!),
                    new Claim(JwtRegisteredClaimNames.Jti, userSearch.Id!),
                };

                //chave de segurança
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("transfile-webapi-chave-symmetricsecuritykey"));

                //credenciais
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                //token
                var myToken = new JwtSecurityToken(
                        issuer: "TransFile-WebAPI",
                        audience: "TransFile-WebAPI",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: creds
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(myToken)
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
