using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MessagingApp.API.Data;
using MessagingApp.API.Dtos;
using MessagingApp.API.Helpers;
using MessagingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessagingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("/api/[controller]")]
    [ApiController]
    [Authorize()]
    public class UsersController : ControllerBase
    {
        private readonly IMessagingRepository _messagingRepo;
        private readonly IMapper _mapper;
        public UsersController(IMessagingRepository messagingRepo, IMapper mapper)
        {
            this._mapper = mapper;
            this._messagingRepo = messagingRepo;

        }
 
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)    //model binding takes care of this value in JSON format
        {
            PagedList<User> users = await _messagingRepo.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _messagingRepo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailsDto>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var userFromRepo = await _messagingRepo.GetUser(id);
            _mapper.Map(userForUpdateDto, userFromRepo);
            if(await _messagingRepo.SaveAll())
                return NoContent();
            
            throw new System.Exception($"Updating User {id} failed on save.");
        }

    }
}