using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IFcmTokenRepository : IBaseRepository<FcmToken>
    {
        Task<string?> GetFcmToken(int id);
        Task<FcmToken?> IsFcmTokenExist(string token, int id);
        Task<FcmToken?> FindByValue(string token);
        Task DisableUserTokens(int userId);
        Task DisableOtherUserToken(int userId, string token);
    }
}
