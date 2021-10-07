using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MessagingApp.API.Data;
using MessagingApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessagingApp.API.Controllers
{
    // [Authorize]
    [Route("[controller]")]
    [ApiController]
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
        public async Task<IActionResult> GetUsers()
        {
            var users = await _messagingRepo.GetUsers();
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _messagingRepo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailsDto>(user);
            return Ok(userToReturn);
        }
    }
}