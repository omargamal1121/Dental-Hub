using System;
using System.Linq;
using System.Threading.Tasks;
using DentalHub.Application.Commands.Chat;
using DentalHub.Application.DTOs.Chat;
using DentalHub.Application.Interfaces;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;

namespace DentalHub.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StartConversationCommandResponse> StartConversationAsync(StartConversationCommand request)
        {
            try
            {
                var conversationRepo = _unitOfWork.GetRepository<Conversation>();

                var conversation = new Conversation
                {
                    Id = Guid.CreateVersion7(),
                    UserId = request.UserId,
                    CreateAt = DateTime.UtcNow
                };

                await conversationRepo.AddAsync(conversation);
                await _unitOfWork.SaveChangesAsync();

                return new StartConversationCommandResponse
                {
                    Success = true,
                    ConversationId = conversation.Id
                };
            }
            catch (Exception ex)
            {
                return new StartConversationCommandResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<SaveMessageCommandResponse> SaveMessageAsync(SaveMessageCommand request)
        {
            try
            {
                var conversationRepo = _unitOfWork.GetRepository<Conversation>();
                var messageRepo = _unitOfWork.GetRepository<Message>();

                var conversation = await conversationRepo.GetByIdAsync(request.ConversationId);
                
                if (conversation == null)
                {
                    conversation = new Conversation
                    {
                        Id = request.ConversationId,
                        UserId = request.UserId,
                        CreateAt = DateTime.UtcNow
                    };
                    await conversationRepo.AddAsync(conversation);
                }
                else if (conversation.UserId != request.UserId && !request.IsAiMessage) 
                {
                    return new SaveMessageCommandResponse { Success = false, Message = "Unauthorized to send to this conversation." };
                }

                var message = new Message
                {
                    Id = Guid.CreateVersion7(),
                    ConversationId = request.ConversationId,
                    Sender = request.IsAiMessage ? "AI" : "User",
                    Content = request.Content,
                    CreateAt = DateTime.UtcNow
                };

                await messageRepo.AddAsync(message);
                await _unitOfWork.SaveChangesAsync();

                return new SaveMessageCommandResponse
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new SaveMessageCommandResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<GetConversationsQueryResponse> GetConversationsAsync(GetConversationsQuery request)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<Conversation>();
                var spec = new DentalHub.Application.Specification.Comman.BaseSpecification<Conversation>(c => c.UserId == request.UserId);
                spec.ApplyOrderByDescending(c => c.CreateAt);

                var conversations = await repo.GetAllAsync(spec);

                return new GetConversationsQueryResponse
                {
                    Success = true,
                    Conversations = conversations.Select(c => new ConversationDto
                    {
                        Id = c.Id,
                        CreatedAt = c.CreateAt
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                return new GetConversationsQueryResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<GetMessagesQueryResponse> GetMessagesAsync(GetMessagesQuery request)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<Message>();
                // Include Conversation to be able to filter by its UserId
                var spec = new DentalHub.Application.Specification.Comman.BaseSpecification<Message>(
                    m => m.ConversationId == request.ConversationId && m.Conversation.UserId == request.UserId);
                
                spec.AddInclude(m => m.Conversation);
                spec.ApplyOrderBy(m => m.CreateAt);

                var messages = await repo.GetAllAsync(spec);

                return new GetMessagesQueryResponse
                {
                    Success = true,
                    Messages = messages.Select(m => new MessageDto
                    {
                        Id = m.Id,
                        Sender = m.Sender,
                        Content = m.Content,
                        CreatedAt = m.CreateAt
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                return new GetMessagesQueryResponse { Success = false, Message = ex.Message };
            }
        }
    }
}
