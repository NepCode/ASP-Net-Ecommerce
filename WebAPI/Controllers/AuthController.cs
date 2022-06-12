using AutoMapper;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO.Address;
using WebAPI.DTO.User;
using WebAPI.Errors;
using WebAPI.Extensions;

namespace WebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseAPIController
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService, IMapper mapper, IPasswordHasher<User> passwordHasher)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }





        // @desc Login user
        // @route POST api/users/login
        // @access Public
        [HttpPost("login")]
        public async Task<ActionResult<UserReadDTO>> Login([FromBody] UserLoginDTO userInfo)
        {
            var user = await _userManager.FindByEmailAsync(userInfo.Email);
            if (User == null)
            {
                return BadRequest(new CodeErrorResponse(400));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, userInfo.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new CodeErrorResponse(401));
            }
            else
            {
                var roles = await _userManager.GetRolesAsync(user);
                //return _repository.BuildToken(userInfo, roles, user.Id);
                return new UserReadDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    Name = user.Name,
                    Token = _tokenService.GenerateAccessToken(user,roles),
                    RefreshToken = _tokenService.GenerateRefreshToken(user,roles),
                    Admin = roles.Contains("admin") ? true : false
                };
            }
        }




        // @desc Register user
        // @route POST api/users/register
        // @access Public
        [HttpPost("register")]
        public async Task<ActionResult<UserReadDTO>> Register([FromBody] UserCreateDTO userInfo)
        {
            var user = new User
            {
                Name = userInfo.Name,
                LastName = userInfo.LastName,
                UserName = userInfo.Username,
                Email = userInfo.Email
            };
            var result = await _userManager.CreateAsync(user, userInfo.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new CodeErrorResponse(400));
            }
            return new UserReadDTO
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
                Name = user.Name,
                Token = _tokenService.GenerateAccessToken(user,null),
                Admin = false
            };

        }



        // @desc Get logged in user Profile 
        // @route GET api/auth/profile
        // @access Private
        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UserReadDTO>> GetUser()
        {
            var user = await _userManager.SearchUserAsync(HttpContext.User);
            var roles = await _userManager.GetRolesAsync(user);
            return new UserReadDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Username = user.UserName,
                Token = _tokenService.GenerateAccessToken(user, roles),
                Admin = roles.Contains("admin") ? true : false
            };
        }



        // @desc Validate user email
        // @route GET api/auth/validateEmail
        // @access Public
        [HttpGet("validateEmail")]
        public async Task<ActionResult<bool>> validateEmail([FromQuery] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return false;

            return true;
        }





        // @desc get user's addresses
        // @route GET api/auth/addresses
        // @access Private
        [Authorize]
        [HttpGet("addresses")]
        public async Task<ActionResult<List<AddressReadDTO>>> GetAddress()
        {
            var user = await _userManager.SearchUserWithAddressAsync(HttpContext.User);
            return _mapper.Map<List<Address>, List<AddressReadDTO>>(user.Addresses);
        }





        // @desc Login user
        // @route POST api/users/refresh-token
        // @access Public
        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserReadDTO>> RefreshToken( [FromBody] UserLoginDTO userInfo, [FromHeader(Name = "refresh-token")] string refreshToken )
        {
            var user = await _userManager.FindByEmailAsync(userInfo.Email);
            if (user == null)
            {
                return BadRequest(new CodeErrorResponse(400));
            }
            var isValidRefreshToken = _tokenService.RefreshTokenValidator(refreshToken);
            if (isValidRefreshToken) {
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(new UserReadDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    Name = user.Name,
                    Token = _tokenService.GenerateAccessToken(user, roles),
                    RefreshToken = refreshToken,
                    Admin = roles.Contains("admin") ? true : false
                });
            }
            return Unauthorized(new CodeErrorResponse(401));
        }



    }
}
