using AgoraIO.Media;
using AutoMapper;
using BLL.DTOs.Conversation;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;
using Common.Interfaces;
using Common.Models;
using DAL.Entities;
using DAL.Interfaces;
using System.Text;

namespace BLL.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IAgoraService _agoraService;
        private readonly IMapper _mapper;

        public ConversationService(IConversationRepository conversationRepository, IAgoraService agoraService, IMapper mapper)
        {
            _conversationRepository = conversationRepository;
            _agoraService = agoraService;
            _mapper = mapper;
        }

        public async Task<ConversationResponse<VideoCallDto>> GenerateVideoCall(int callerId, int receiverId, RtcTokenBuilder2.Role callerRole)
        {
            string channelName = await _conversationRepository.GetChannelToken(callerId, receiverId);            

            var result = await _agoraService.GenerateCallingToken(callerId, channelName, callerRole);

            return new ConversationResponse<VideoCallDto>(new VideoCallDto
            {
                Token = result,
                ReceivingUserId = receiverId,
                ChannelName = channelName,
            });
        }

        public async Task<ConversationListDto> GetConversation(string accountOneUsername, string accountTwoUsername)
        {
            var entity = await _conversationRepository.GetConversation(accountOneUsername, accountTwoUsername); 
            if (entity == null)
            {
                throw new NotFoundException("Conversation not found!");
            }

            return _mapper.Map<ConversationListDto>(entity);
        }

        public async Task<ConversationListResponse<PagedResult<ConversationListDto>>> GetConversations(QueryParameters queryParameters, int userId)
        {
            var accounts = await _conversationRepository.GetConversationsAsync<ConversationListDto>(queryParameters, userId);
            accounts.Data.ForEach(conv =>
            {
                conv.AccountOneUsername = conv.AccountOneUsername.Split("@")[0];
                conv.AccountTwoUsername = conv.AccountTwoUsername.Split("@")[0];
            });

            return new ConversationListResponse<PagedResult<ConversationListDto>>(accounts);
        }
        public async Task PostConversation(AddConversationDto conversationDto)
        {
            var validator = new AddConversationDtoValidator();
            var validationResult = await validator.ValidateAsync(conversationDto);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid data", validationResult);
            }
            var result = await _conversationRepository.IsConversationExist(conversationDto.AccountOneId, conversationDto.AccountTwoId);
            if (result)
            {
                throw new NotFoundException("Conversation not found!");
            }

            var channelId = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{conversationDto.AccountOneId}_{conversationDto.AccountTwoId}"));
            
            var conversation = _mapper.Map<Conversation>(conversationDto);
            conversation.ChannelId = channelId;


            var reversedConversation = new Conversation
            {
                AccountOneId = conversationDto.AccountTwoId,
                AccountTwoId = conversationDto.AccountOneId,
                ChannelId = channelId,
                Status = 1,

            };

            await _conversationRepository.CreateManyAsync(conversation, reversedConversation);
        }

        public async Task SendChatMessage(string fromUsername, string toUsername, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }
            await _agoraService.SendChatMessage(fromUsername, toUsername, content);
        }
    }
}
