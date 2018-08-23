using System;

namespace PupDate.API.Dtos
{
    public class CreateMessageDto
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public DateTime MessageSent { get; set; }
        
        public string Content { get; set; }
        public CreateMessageDto()
        {
            MessageSent = DateTime.Now;
        }
    }
}