using AutoMapper;

using BLL.DTOs.Account;
using BLL.DTOs.Place.MarkPlace;
using BLL.DTOs.Staff;
using BLL.DTOs.Visitor;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;

using Common.Constants;
using Common.Distances;
using Common.Extensions;
using Common.Interfaces;
using Common.Models;

using DAL.Entities;
using DAL.Interfaces;

using FirebaseAdmin.Messaging;

using FluentValidation;

using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMarkPlaceRepository _markPlaceRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly IAgoraService _agoraService;
        private readonly ILocationService _locationService;
        private readonly IFcmTokenRepository _fcmTokenRepository;
        private readonly INationalityRepository _nationalityRepository;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepository,
            IMapper mapper,
            IFcmTokenRepository fcmTokenRepository,
            IMarkPlaceRepository markPlaceRepository,
            IConversationRepository conversationRepository,
            IPlaceRepository placeRepository,
            IAgoraService agoraService,
            ILocationService locationService,
            INationalityRepository nationalityRepository,
            IMailService mailService
            )
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _fcmTokenRepository = fcmTokenRepository;
            _markPlaceRepository = markPlaceRepository;
            _conversationRepository = conversationRepository;
            _placeRepository = placeRepository;
            _agoraService = agoraService;
            _locationService = locationService;
            _nationalityRepository = nationalityRepository;
            _mailService = mailService;
        }

        public async Task<AccountResponse<AccountListDto>> GetAccountById(int id)
        {
            var result = await _accountRepository.FindByIdAsync(id);

            if (result == null)
            {
                throw new NotFoundException();
            }
            var data = _mapper.Map<AccountListDto>(result);
            return new AccountResponse<AccountListDto>(data);
        }

        public async Task<AccountListResponse<PagedResult<AccountListDto>>> GetAccountsAsync(QueryParameters queryParameters)
        {
            var accounts = await _accountRepository.GetAsync<AccountListDto>(queryParameters, orderBy: (account) => account.CreateTime);

            return new AccountListResponse<PagedResult<AccountListDto>>(accounts);
        }


        public async Task<PlaceListResponse<PagedResult<MarkPlaceDto>>> GetMarkPlaceWithAccountAsync(QueryParameters queryParameters, int accountId)
        {
            var places = await _markPlaceRepository.GetAsyncWithConditions<MarkPlaceDto>(queryParameters, queryConditions: query =>
            {
                return query
                    .Where(x => x.AccountId == accountId && x.Status == 1)
                    .Include(x => x.Place.PlaceImages);
            });
            return new PlaceListResponse<PagedResult<MarkPlaceDto>>(places);
        }

        public async Task<bool> PostMarkPlaceWithAccountAsync(int accountId, int placeId)
        {
            var placeExist = await _placeRepository.Exist(placeId);
            if (!placeExist)
            {
                throw new NotFoundException();
            }
            var markPlaceEntity = await _markPlaceRepository.FindByIdAsync(accountId, placeId);

            if (markPlaceEntity == null)
            {
                markPlaceEntity = new MarkPlace
                {
                    AccountId = accountId,
                    PlaceId = placeId,
                    Status = 1,
                };

                var task = await _markPlaceRepository.CreateAsync(markPlaceEntity);
                if (!task)
                {
                    throw new NotFoundException("Id Not Found!");
                }
                return true;
            }
            else
            {
                if (markPlaceEntity.Status == 0)
                {
                    markPlaceEntity.Status = 1;
                }
                else
                {
                    markPlaceEntity.Status = 0;
                }

                var task = await _markPlaceRepository.UpdateAsync(markPlaceEntity);
                if (!task)
                {
                    throw new NotFoundException("Id Not Found!");
                }
                return true;
            }
        }

        public async Task<string> SendNotification(int senderId, int receiverId, int? bookingId, int notificationType)
        {
            string? receiverFcmToken = await _fcmTokenRepository.GetFcmToken(receiverId) ?? throw new NotFoundException();
            Message message;
            var notificationTypeEnum = (NotificationTypes)Enum.ToObject(typeof(NotificationTypes), notificationType);

            switch (notificationTypeEnum)
            {
                case NotificationTypes.ContactRequest:
                    var senderAccount = await _accountRepository.FindByIdAsync(senderId) ?? throw new ForbiddenException();
                    var senderFullname = senderAccount.FirstName + " " + senderAccount.LastName;

                    message = new Message()
                    {
                        Notification = new Notification()
                        {
                            Title = senderFullname,
                            ImageUrl = senderAccount.Image,
                            Body = "notification_contact_body",

                        },
                        Data = new Dictionary<string, string>()
                    {
                        { "notificationType", ((int)notificationTypeEnum).ToString() },
                        { "senderId", $"{senderAccount.Id}" },
                        { "senderEmail", $"{senderAccount.Email}" },
                        { "senderPhone", $"{senderAccount!.Phone}" },
                        { "senderFirstName", $"{senderAccount.FirstName}" },
                        { "senderLastName", $"{senderAccount.LastName}" },
                        { "senderImage", $"{senderAccount.Image}" },
                    },
                        Token = receiverFcmToken,
                    };
                    break;
                case NotificationTypes.Chat:
                    senderAccount = await _accountRepository.FindByIdAsync(senderId) ?? throw new ForbiddenException();
                    senderFullname = senderAccount.FirstName + " " + senderAccount.LastName;
                    var channelId = await _conversationRepository.GetChannelToken(senderId, receiverId);
                    message = new Message()
                    {
                        Notification = new Notification()
                        {
                            Title = senderFullname,
                            Body = "notification_chat_body",
                        },
                        Data = new Dictionary<string, string>()
                    {
                        { "notificationType", ((int)notificationTypeEnum).ToString() },
                        { "senderId", $"{senderAccount.Id}" },
                        { "channelId", $"{channelId}" },
                        { "senderFirstName", $"{senderAccount.FirstName}" },
                        { "senderLastName", $"{senderAccount.LastName}" },
                        { "senderImage", $"{senderAccount.Image}" },
                    },
                        Token = receiverFcmToken,
                    };
                    break;
                case NotificationTypes.Call:
                    senderAccount = await _accountRepository.FindByIdAsync(senderId) ?? throw new ForbiddenException();
                    senderFullname = senderAccount.FirstName + " " + senderAccount.LastName;
                    channelId = await _conversationRepository.GetChannelToken(senderId, receiverId);
                    var callingToken = await _agoraService.GenerateCallingToken(receiverId, channelId, AgoraIO.Media.RtcTokenBuilder2.Role.ROLE_SUBSCRIBER);

                    message = new Message()
                    {
                        Notification = new Notification()
                        {
                            Title = senderFullname,
                            Body = "notification_call_body",
                        },
                        Data = new Dictionary<string, string>()
                        {
                            { "notificationType", ((int)notificationTypeEnum).ToString() },
                            { "senderId", $"{senderAccount.Id}" },
                            { "senderPhone", $"{senderAccount!.Phone}" },
                            { "senderFirstName", $"{senderAccount.FirstName}" },
                            { "senderLastName", $"{senderAccount.LastName}" },
                            { "senderImage", $"{senderAccount.Image}" },
                            { "channelId", channelId },
                            { "remoteUid", $"{senderId}" },
                            { "callingToken", callingToken },
                        },
                        Token = receiverFcmToken,
                    };
                    break;
                case NotificationTypes.PaymentSuccessful:
                    message = new Message()
                    {
                        Notification = new Notification()
                        {
                            Title = "payment_successful",
                            Body = "notification_payment_successful_body",
                        },
                        Data = new Dictionary<string, string>()
                    {
                        { "notificationType", ((int)notificationTypeEnum).ToString() },
                        { "bookingId", bookingId.ToString()! },

                    },
                        Token = receiverFcmToken,
                    };
                    break;
                case NotificationTypes.BookingCancelled:
                    message = new Message()
                    {
                        Notification = new Notification()
                        {
                            Title = "booking_cancelled",
                            Body = $"notification_booking_cancelled_body",
                        },
                        Data = new Dictionary<string, string>()
                    {
                        { "notificationType", ((int)notificationTypeEnum).ToString() },
                        { "bookingId", bookingId.ToString()! },
                    },
                        Token = receiverFcmToken,
                    };
                    break;
                default: throw new BadRequestException("Notification type invalid!");
            }
            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            return response;
        }

        public async Task<AccountListResponse<List<UserLocationData>>> GetAccountsNearby(int userId, string languageCode)
        {
            var locationData = await _locationService.GetUserLocationDatasAsync(userId, languageCode);

            var currentUserData = locationData.First(x => x.Id == userId);

            var orderedLocationData = locationData.Where(x => x.Id != userId);
            foreach (var x in orderedLocationData)
            {
                var distance = DistanceCalculator.calculate(new GeoPoint(currentUserData.Latitude, currentUserData.Longitude),
                                            new GeoPoint(x.Latitude, x.Longitude));
                x.Distance = Math.Round(distance, 0);
            }

            return new AccountListResponse<List<UserLocationData>>(orderedLocationData.OrderBy(x => x.Distance).ToList());
        }

        #region Account Operation
        public async Task<bool> UpdateOpAccount(string email, AccountUpdateDto opUpdateDto)
        {
            // model validation
            var validator = new AccountOpUpdateDtoValidator();
            var validationResult = await validator.ValidateAsync(opUpdateDto);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid data", validationResult);
            }

            var nationalExist = await _nationalityRepository.NationCodeIsExist(opUpdateDto.NationalCode);

            if (!nationalExist)
            {
                throw new BadRequestException("National code is not exist!");
            }

            var account = await _accountRepository.FindByEmail(email);
            if (account == null)
            {
                throw new BadRequestException("Account id not found!");
            }
            else
            {
                _mapper.Map(opUpdateDto, account);
                var task = _accountRepository.UpdateAsync(account);
                return await task;
            }
        }

        public async Task<bool> ChangePassword(string email, ChangePasswordDto changePasswordDto)
        {
            // model validation
            var validator = new ChangePasswordDtoValidator();
            var validationResult = await validator.ValidateAsync(changePasswordDto);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid data", validationResult);
            }

            var account = await _accountRepository.FindByEmail(email);

            if (account == null) throw new NotFoundException(email);

            var isValidPassword = PasswordExtension.VerifyPassword(changePasswordDto.OldPassword, account.Password);
            if (!isValidPassword)
            {
                throw new BadRequestException("Old password is wrong!");
            }

            _mapper.Map(changePasswordDto, account);
            var task = await _accountRepository.UpdateAsync(account);
            return task;
        }
        #endregion

        #region Super Admin
        public async Task<AccountListResponse<PagedResult<VisitorListDto>>> GetListVisitor(QueryParameters queryParameters)
        {
            var task = await _accountRepository.GetAsyncWithConditions<VisitorListDto>(queryParameters, includeDeleted: true, queryConditions: query =>
            {
                return query
                    .Where(x => x.RoleId == 3)
                    .Include(x => x.NationalCodeNavigation);
            });
            return new AccountListResponse<PagedResult<VisitorListDto>>(task);
        }

        public async Task<bool> UpdateStaffAccount(int staffId, StaffUpdateDto staffUpdateDto)
        {
            // model validation
            var validator = new StaffUpdateDtoValidator();
            var validationResult = await validator.ValidateAsync(staffUpdateDto);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid data", validationResult);
            }

            var nationalExist = await _nationalityRepository.NationCodeIsExist(staffUpdateDto.NationalCode);

            if (!nationalExist)
            {
                throw new BadRequestException("National code is not exist!");
            }

            var account = await _accountRepository.GetStaff(staffId);
            if (account == null || account.RoleId == 1)
            {
                throw new BadRequestException("Account id not found!");
            }
            else
            {
                _mapper.Map(staffUpdateDto, account);
                var task = _accountRepository.UpdateAsync(account);
                return await task;
            }
        }

        public async Task<AccountResponse<VisitorDetailDto>> GetVisitorDetail(int visitorId)
        {
            var task = await _accountRepository.GetVisitor(visitorId);
            if (task == null) throw new NotFoundException("Visitor Id not found!");

            return new AccountResponse<VisitorDetailDto>(_mapper.Map<VisitorDetailDto>(task));
        }

        public async Task<bool> DeactiveVisitorById(int id)
        {
            var task = await _accountRepository.DeactiveVisitor(id);
            if (!task)
            {
                throw new NotFoundException("Id Not Found!");
            }
            return true;
        }

        public async Task<AccountListResponse<PagedResult<StaffListDto>>> GetListStaff(QueryParameters queryParameters)
        {
            var task = await _accountRepository.GetAsyncWithConditions<StaffListDto>(queryParameters, includeDeleted: true, queryConditions: query => query.Where(x => x.RoleId != 3));
            return new AccountListResponse<PagedResult<StaffListDto>>(task);
        }

        public async Task<bool> DeactiveStaffById(int id)
        {
            var task = await _accountRepository.DeactiveStaff(id);
            if (!task)
            {
                throw new NotFoundException("Staff Id Not Found!");
            }
            return true;
        }

        public async Task<AccountResponse<StaffDetailDto>> GetStaffDetail(int staffId)
        {
            var task = await _accountRepository.GetStaff(staffId);
            if (task == null) throw new NotFoundException("Staff Id not found!");

            return new AccountResponse<StaffDetailDto>(_mapper.Map<StaffDetailDto>(task));
        }

        public async Task<bool> CreateNewStaff(CreateStaffDto staffDto)
        {
            // model validation
            var validator = new CreateStaffDtoValidator();
            var validationResult = await validator.ValidateAsync(staffDto);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid data", validationResult);
            }

            // valid role id 
            if (staffDto.RoleId != 1 && staffDto.RoleId != 2)
            {
                throw new BadRequestException("Role id not support in the system!");
            } 

            var isEmailExist = await _accountRepository.IsEmailExist(staffDto.Email);
            if (isEmailExist)
            {
                throw new BadRequestException("Email already exist.");
            }
            var isPhoneExist = await _accountRepository.IsPhoneExist(staffDto.Phone);
            if (isPhoneExist)
            {
                throw new BadRequestException("Phone already exist.");
            }
            var entity = _mapper.Map<Account>(staffDto);

            var task = _accountRepository.CreateAsync(entity);
            int atIndex = entity.Email.IndexOf('@');
            MailData mail = new MailData
            {
                EmailToName = entity.Email,
                FullName = entity.FirstName + " " + entity.LastName,
                UserName = entity.Email.Substring(0, atIndex),
                Password = staffDto.Password
            };
            var check = _mailService.SendMail(mail);

            return await task;
        }
        #endregion

    }
}
