using AutoMapper;

using Common.Interfaces;
using Common.Models;

using DAL.DatabaseContext;
using DAL.Entities;
using DAL.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService redisCacheService) : base(context, mapper, unitOfWork, redisCacheService)
        {
            _context = context;
        }

        public async Task<Account?> FindByEmail(string email)
        {
            var query = _context.Accounts
                .Include(q => q.Role)
                .Include(q => q.ConfigLanguage)
                .Where(x => x.Email == email);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Account?> FindByUsername(string username)
        {
            var query = _context.Accounts
                .Include(q => q.Role)
                .Include(q => q.ConfigLanguage)
                .Include(q => q.NationalCodeNavigation)
                .Where(x => x.Email.Contains(username) && x.Status != 0);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Account?> FindByPhone(string phone)
        {
            var query = _context.Accounts
                .Include(q => q.Role)
                .Include(q => q.ConfigLanguage)
                .Include(q => q.NationalCodeNavigation)
                .Where(x => x.Phone == phone && x.Status != 0);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<bool> IsEmailExist(string email)
        {
            return await _context.Accounts.AnyAsync(x => x.Email == email);
        }

        public async Task<Account> GetVisitor(int id)
        {
            var task = await _context.Accounts.Where(x => x.Id == id  && x.RoleId == 3).Include(x => x.Bookings).ThenInclude(x => x.Transactions).Include(x => x.NationalCodeNavigation).IgnoreQueryFilters().FirstOrDefaultAsync();
            return task;
        }

        public async Task<bool> DeactiveVisitor(int id)
        {
            var visitor = await _context.Accounts.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == id);
            if (visitor == null)
            {
                return false;
            }
            else if (visitor.Status == 1)
            {
                visitor.Status = 0;
            }
            else
            {
                visitor.Status = 1;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactiveStaff(int id)
        {
            var staff = await _context.Accounts.Where(x => x.RoleId == 2).IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == id);
            if (staff == null)
            {
                return false;
            }
            else if (staff.Status == 1)
            {
                staff.Status = 0;
            }
            else
            {
                staff.Status = 1;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsPhoneExist(string phone)
        {
            return await _context.Accounts.AnyAsync(x => x.Phone == phone);
        }

        public async Task<Account> GetStaff(int id)
        {
            var task = await _context.Accounts.Where(x => x.RoleId != 3).IgnoreQueryFilters().Where(x => x.Id == id).SingleOrDefaultAsync();
            return task;
        }
    }
}
