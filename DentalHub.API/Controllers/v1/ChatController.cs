using Asp.Versioning;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using DentalHub.Application.Commands.Chat;
using DentalHub.Application.DTOs.Chat;

namespace DentalHub.API.Controllers.v1
{

    [Authorize]
    [ApiVersion("1.0")]
    public class ChatController : BaseController
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst("uid")?.Value 
                ?? string.Empty;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartConversation()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID could not be found from the token.");
            }

            var command = new StartConversationCommand { UserId = userId };
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { ConversationId = result.ConversationId });
        }

        [HttpPost("message")]
        public async Task<IActionResult> SaveMessage([FromBody] SaveMessageDto request)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID could not be found from the token.");
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Content is required.");
            }

            var command = new SaveMessageCommand
            {
                UserId = userId,
                ConversationId = request.ConversationId,
                Content = request.Content,
                IsAiMessage = request.IsAiMessage
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok();
        }
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID could not be found from the token.");
            }

            var command = new GetConversationsQuery { UserId = userId };
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Conversations);
        }

        [HttpGet("conversation/{conversationId}")]
        public async Task<IActionResult> GetMessages(Guid conversationId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID could not be found from the token.");
            }

            var command = new GetMessagesQuery { UserId = userId, ConversationId = conversationId };
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Messages);
        }
    }
}





