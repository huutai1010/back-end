using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Interfaces;
using Common.Models;
using DAL.DatabaseContext;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace DAL.Repositories
{
    public class ConversationRepository : BaseRepository<Conversation>, IConversationRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ConversationRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService redisCacheService) : base(context, mapper, unitOfWork, redisCacheService)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<string> GetChannelToken(int AccountOne, int AccountTwo)
        {
            return (await _context.Conversations.FirstAsync(x => x.AccountOneId == AccountOne && x.AccountTwoId == AccountTwo)).ChannelId;
        }

        public async Task<Conversation> GetConversation(string accountOneUsername, string accountTwoUsername)
        {
            return await _context.Conversations
                .Include(q => q.AccountOne).Include(q => q.AccountTwo)
                .FirstOrDefaultAsync(x => x.AccountOne.Email.Contains(accountOneUsername) 
                                        && x.AccountTwo.Email.Contains(accountTwoUsername));
        }

        public async Task<PagedResult<ConversationListDto>> GetConversationsAsync<ConversationListDto>(QueryParameters queryParameters, int userId)
        {
            var query = _context.Set<Conversation>().AsQueryable();

            var totalSize = await query.Where(x => x.AccountOneId == userId)
                .CountAsync();

            query = _context.Set<Conversation>();
            query = query.Include(q => q.AccountOne).Include(q => q.AccountTwo).Where(x => x.AccountOneId == userId);


            var items = await query.Skip((queryParameters.PageNumber) * queryParameters.PageSize).Take(queryParameters.PageSize)
                .ProjectTo<ConversationListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var pageCount = (double)totalSize / queryParameters.PageSize;

            return new PagedResult<ConversationListDto>
            {
                Data = items,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                PageCount = (int)Math.Ceiling(pageCount),
                TotalCount = totalSize,
            };
        }
    
        public async Task<bool> IsConversationExist(int AccountOne, int AccountTwo)
        {
            return await _context.Conversations.Where(x => (x.AccountOneId == AccountOne && x.AccountTwoId == AccountTwo) ||
                (x.AccountOneId == AccountTwo && x.AccountTwoId == AccountOne)).CountAsync() == 2;
        }
    }
}

