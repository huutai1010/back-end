using AutoMapper;

using Common.Interfaces;

using DAL.DatabaseContext;
using DAL.Entities;
using DAL.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class FcmTokenRepository : BaseRepository<FcmToken>, IFcmTokenRepository
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public FcmTokenRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService redisCacheService) : base(context, mapper, unitOfWork, redisCacheService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task DisableOtherUserToken(int userId, string token)
        {
            var entity = await _context.FcmTokens.FirstOrDefaultAsync(x => x.AccountId != userId && x.Token == token);
            if (entity != null)
            {
                entity.IsPrimary = false;
                _context.Entry(entity).State = EntityState.Modified;
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DisableUserTokens(int userId)
        {
            var tokens = _context.FcmTokens.Where(x => x.AccountId == userId && x.IsPrimary);
            foreach (var tokenItem in tokens)
            {
                tokenItem.IsPrimary = false;
                _context.Entry(tokenItem).State = EntityState.Modified;
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<FcmToken?> FindByValue(string token)
        {
            return await _context.FcmTokens.FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task<string?> GetFcmToken(int id)
        {
            var task = await _context.FcmTokens
                .Where(x => x.AccountId == id && x.IsPrimary == true).FirstOrDefaultAsync();
            return task?.Token;
        }

        public async Task<FcmToken?> IsFcmTokenExist(string token, int id)
        {
            var task = _context.FcmTokens
                .Where(x => x.Token == token && x.AccountId == id).FirstOrDefaultAsync();
            if (task == null)
            {
                return null;
            }
            return await task;
        }
    }
}
