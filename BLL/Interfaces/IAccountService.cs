using BLL.DTOs.Account;
using BLL.DTOs.Auth;
using BLL.DTOs.Place;
using BLL.DTOs.Place.MarkPlace;
using BLL.DTOs.Service;
using BLL.DTOs.Staff;
using BLL.DTOs.Visitor;
using BLL.Responses;

using Common.Models;
using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IAccountService
    {
        Task<AccountListResponse<PagedResult<AccountListDto>>> GetAccountsAsync(QueryParameters queryParameters);
        Task<AccountListResponse<List<UserLocationData>>> GetAccountsNearby(int userId, string languageCode);

        Task<AccountResponse<AccountListDto>> GetAccountById(int id);
        Task<string> SendNotification(int senderId, int receiverId, int? bookingId, int notificationType);
        Task<PlaceListResponse<PagedResult<MarkPlaceDto>>> GetMarkPlaceWithAccountAsync(QueryParameters queryParameters ,int accountId);
        Task<bool> PostMarkPlaceWithAccountAsync(int accountId, int placeId);
        Task<bool> UpdateOpAccount(string email, AccountUpdateDto opUpdateDto);
        Task<bool> ChangePassword(string email, ChangePasswordDto changePasswordDto);

        #region visitor
        Task<AccountListResponse<PagedResult<VisitorListDto>>> GetListVisitor(QueryParameters queryParameters);
        Task<AccountResponse<VisitorDetailDto>> GetVisitorDetail(int visitorId);
        Task<bool> DeactiveVisitorById(int id);
        #endregion

        #region staff
        Task<bool> CreateNewStaff(CreateStaffDto staffDto);
        Task<AccountListResponse<PagedResult<StaffListDto>>> GetListStaff(QueryParameters queryParameters);
        Task<AccountResponse<StaffDetailDto>> GetStaffDetail(int staffId);
        Task<bool> UpdateStaffAccount(int staffId, StaffUpdateDto staffUpdateDto);
        Task<bool> DeactiveStaffById(int id);
        #endregion
    }
}
