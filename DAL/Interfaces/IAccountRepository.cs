using Common.Models;
using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IAccountRepository : IBaseRepository<Account>
    {
        Task<Account?> FindByEmail(string email);
        Task<Account?> FindByUsername(string username);
        Task<Account?> FindByPhone(string phone);
        Task<bool> IsEmailExist(string email);
        Task<Account> GetVisitor(int id);
        Task<bool> DeactiveVisitor(int id);
        Task<bool> IsPhoneExist(string phone);
        Task<bool> DeactiveStaff(int id);
        Task<Account> GetStaff(int id);
    }
}
