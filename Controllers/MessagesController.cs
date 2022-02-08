using System;
using System.Collections;
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

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery] MessageParams messageParams){
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)){
                return Unauthorized();
            }

            messageParams.UserId = userId;

            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);

            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize,
             messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);
            
            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId){
            if( userId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                return Unauthorized();
            
            var messageThreadFromRepo = await _repo.GetMessageThread(userId, recipientId);

            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageThreadFromRepo);

            return Ok(messageThread);

        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto){

            User sender = await _repo.GetUser(userId);

            if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userId;

            User recipient = await _repo.GetUser(messageForCreationDto.RecipientId);

            if(recipient == null)
                return BadRequest("Could not find user.");
            
            Message message = _mapper.Map<Message>(messageForCreationDto);                          //mapping the incoming Data Transfer Object to Message in order to save to database.

            _repo.Add(message);

            if(await _repo.SaveAll()){
                 MessageToReturnDto messageToReturn = _mapper.Map<MessageToReturnDto>(message);      //mapping to MessageForCreationDto in order to return to the user. If we don't do this, this'll return the whole message object back to the user. 
                return CreatedAtRoute("GetMessage", new {id = message.Id}, messageToReturn);
            }

            throw new System.Exception("Creating message failed on save. ");

        }



        [HttpPost("{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId, int userId){
             if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var messageFromRepo = await _repo.GetMessage(messageId);

            if(messageFromRepo.SenderId == userId){
                messageFromRepo.SenderDeleted = true;
            }
            if(messageFromRepo.RecipientId == userId){
                messageFromRepo.RecipientDeleted = true;
            }

            if(messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
                _repo.Delete(messageFromRepo);
            
            if(await _repo.SaveAll()){
                return NoContent();
            }
            
            throw new System.Exception("Error deleting the message");

        }


        [HttpPost("{messageId}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int messageId){
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var message = await _repo.GetMessage(messageId);

            if(message.RecipientId !=userId)
                return Unauthorized();
            
            message.IsRead = true;

            message.DateRead = DateTime.Now;

            await _repo.SaveAll();

            return NoContent();

        }
    }
}