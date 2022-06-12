using AutoMapper;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO.Role;
using WebAPI.DTO.User;
using WebAPI.Errors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : BaseAPIController
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService, IMapper mapper, IPasswordHasher<User> passwordHasher, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _roleManager = roleManager;
        }







        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }





        // @desc Update role user
        // @route PUT api/users/roles/{id}
        // @access Private
        [Authorize(Roles = "admin")]
        [HttpPut("roles/{id}")]
        public async Task<ActionResult<UserReadDTO>> UpdateRole(string id, RoleUpdateDTO roleParams)
        {
            var role = await _roleManager.FindByNameAsync(roleParams.Name);

            if (role == null)
            {
                return NotFound(new CodeErrorException(404, "role not found "));
            }


            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(new CodeErrorException(404, "user not found "));
            }

            var userDTO = _mapper.Map<User, UserReadDTO>(user);
            if (roleParams.Status)
            {
                var result = await _userManager.AddToRoleAsync(user, roleParams.Name);
                if (result.Succeeded)
                {
                    userDTO.Admin = true;
                }

                if (result.Errors.Any())
                {
                    if (result.Errors.Where(x => x.Code == "UserAlreadyInRole").Any())
                    {
                        userDTO.Admin = true;
                    }
                }
            }
            else
            {
                var result = await _userManager.RemoveFromRoleAsync(user, roleParams.Name);
                if (result.Succeeded)
                {
                    userDTO.Admin = false;
                }
            }

            if (userDTO.Admin)
            {
                var roles = new List<string>();
                roles.Add("admin");
                userDTO.Token = _tokenService.GenerateAccessToken(user, roles);
            }
            else
            {
                userDTO.Token = _tokenService.GenerateAccessToken(user, null);
            }

            return userDTO;


        }




    }
}
