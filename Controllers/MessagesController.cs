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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    [Authorize()]
    public class MessagesController : ControllerBase
    {
        private readonly IMessagingRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IMessagingRepository repo, IMapper mapper)
        {
            this._mapper = mapper;
            this._repo = repo;

        }

        [HttpGet("/{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id){
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }

            Message messageFromRepo = await _repo.GetMessage(id);

            if(messageFromRepo == null){
                return NotFound();
            }

            return Ok(messageFromRepo);

        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto){
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userId;

            User recipient = await _repo.GetUser(messageForCreationDto.RecipientId);

            if(recipient == null)
                return BadRequest("Could not find user.");
            
            Message message = _mapper.Map<Message>(messageForCreationDto);                          //mapping the incoming Data Transfer Object to Message in order to save to database.

            _repo.Add(message);

            MessageForCreationDto messageToReturn = _mapper.Map<MessageForCreationDto>(message);      //mapping to MessageForCreationDto in order to return to the user. If we don't do this, this'll return the whole message object back to the user. 

            if(await _repo.SaveAll()){
                return CreatedAtRoute("GetMessage", new {id = message.Id}, messageToReturn);
            }

            throw new System.Exception("Creating message failed on save. ");

        }

    }
}