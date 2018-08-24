using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PupDate.API.Data;
using PupDate.API.Dtos;
using PupDate.API.helpers;
using PupDate.API.Models;

namespace PupDate.API.Controllers
{
        [ServiceFilter(typeof(LogUserActivity))]
        [Authorize]
        [Route("api/users/{userId}/[controller]")]
        [ApiController]

        public class MessagesController : ControllerBase
        {
            private readonly IDatingRepository _repo;
            private readonly IMapper _mapper;
            public MessagesController(IDatingRepository repo, IMapper mapper)
            {
            _mapper = mapper;
            _repo = repo;
            }

            [HttpGet("{id}", Name = "GetMessage")]

            public async Task<IActionResult> GetMessage(int userId, int id)
            {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo == null)
                return NotFound();
            
            return Ok(messageFromRepo);
        }

        [HttpGet]

        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParameters messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageParams.UserId = userId;

            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);

            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize,
                messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]

        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messagesFromRepo = await _repo.GetMessageThread(userId, recipientId);

            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            return Ok(messageThread);
        }

        [HttpPost]
        
        public async Task<IActionResult> CreateMessage(int userId, CreateMessageDto createMessageDto)
        {
            var sender = await _repo.GetUser(userId);

            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            createMessageDto.SenderId = userId;

            var recipient = await _repo.GetUser(createMessageDto.RecipientId);

            if (recipient == null)
        
                return BadRequest("Could not find user, please try again");
            var message = _mapper.Map<Message>(createMessageDto);

            _repo.Add(message);

            if (await _repo.SaveAll()) {
                var returnedCreatedMessage = _mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new {id = message.Id}, returnedCreatedMessage);
            }
            throw new Exception("Creating the message failed to save to the database, please try again.");
        }

        [HttpPost("{id}")]

        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo.SenderId == userId)
            messageFromRepo.SenderDeleted = true;

            if (messageFromRepo.RecipientId == userId)
                messageFromRepo.RecipientDeleted = true;
            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
                _repo.Delete(messageFromRepo);
            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception("There was an error deleting the message");
        }
    }
}