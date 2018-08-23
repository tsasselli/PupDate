using System;
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

        [HttpPost]
        
        public async Task<IActionResult> CreateMessage(int userId, CreateMessageDto createMessageDto)
        {

            createMessageDto.SenderId = userId;

            var recipient = await _repo.GetUser(createMessageDto.RecipientId);

            if (recipient == null)
        
                return BadRequest("Could not find user, please try again");
            var message = _mapper.Map<Message>(createMessageDto);

            _repo.Add(message);

            var returnedCreatedMessage = _mapper.Map<CreateMessageDto>(message);

            if (await _repo.SaveAll())
                return CreatedAtRoute("GetMessage", new {id = message.Id}, returnedCreatedMessage);

            throw new Exception("Creating the message failed to save to the database, please try again.");
        }
    }
}