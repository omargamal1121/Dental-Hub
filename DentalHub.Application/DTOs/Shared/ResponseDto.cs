using System.Collections.Generic;

namespace DentalHub.Application.DTOs.Shared
{
	public class ResponseDto
	{
		public ResponseDto()
		{
			
		}
		public ResponseDto(string message, object? data = null, List<LinkDto>? links=null)
		{
			Message = message;
			Data = data ?? new object();
			Links = links ?? new List<LinkDto>();
		}
		public string Message { get; set; } = string.Empty;
		public object Data { get; set; }

		public List<LinkDto> Links { get; set; } = new List<LinkDto>();
	}
}
