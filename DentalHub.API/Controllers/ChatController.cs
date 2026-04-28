using DentalHub.Application.DTOs.Chat;
using DentalHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DentalHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("next")]
        public IActionResult Next([FromBody] ChatRequestDto request)
        {
            // Null state handling is done inside the service layer
            if (request == null)
            {
                // Ensure request object exists even if body is completely empty
                request = new ChatRequestDto();
            }

            try
            {
                var response = _chatService.ProcessNext(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors gracefully
                return StatusCode(500, new { message = "An error occurred during chat processing.", error = ex.Message });
            }
        }
    }
}
