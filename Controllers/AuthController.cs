using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MessagingApp.API.Data;
using MessagingApp.API.Dtos;
using MessagingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MessagingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //validate request

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username already exists.");

            var userToCreate = new User
            {

                Username = userForRegisterDto.Username

            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Username, userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            //Next, we are going to build up a token that we're then going to return to our user. Our token is going to contain two bits of information about the user. It's going to contain the user's id and the user's username. 
            //This token can be validated by our server without making a database call, we can add small pieces of information in here....so that when our server gets it, it does not need to look into our database for user's name and user's id. 

            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            //What We also need in our token is a key to sign our token. This part's going to be hashed, so it's not going to be readable inside our token itself.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));        //We need to encode the key into a byte-array because that's what the function SymmetricSecurityKey takes as argument. The "Token" key is the value of "Token" feild in appsettings.json file, the implementation of which is in the _config variable. 


            //Now that we have our key, we create our signing credentials. 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //next, we create a security token descriptor, which is going to contain our claims, expiry date and our signing credentials. 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            //We need a token handler. 
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);      //this token will contain the JWT token that we want to return to our client. 

            var user = _mapper.Map<UserForListDto>(userFromRepo);


            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user = user
            });

            //The key thing to note here is that, Without specifying the signature, we're able to see what's inside our token. So, we would want to be slightly careful about what we put inside our token.
            //We don't want to store any sensitive information that we wouldn't want the client to see. Because it's easy to decrypt it. but it doesn't mean that the user can make their own token and send it up
            //to the server, because the signature part is what the server uses to verify the token. The server uses that secret key that we specified on the Appsettings to ensure that the token is valid. 
            //So that way, we can ensure that the token hasn't been manipulated in some way. So, if we start changing some properties, everything changes in the token, including the signature and it wouldn't get past the server check. 
            //but regardless of the signature, we do want to be careful. 


        }
    }
}