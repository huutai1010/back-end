using AutoMapper;
using BLL.DTOs.Account;
using BLL.DTOs.Auth;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;
using Common.AgoraIO.Common;
using Common.AppConfiguration;
using Common.Constants;
using Common.Extensions;
using Common.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly AgoraSettings _agoraSettings;
        private readonly IAccountRepository _accountRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly INationalityRepository _nationalityRepository;
        private readonly IMapper _mapper;
        private readonly IJwtUtils _jwtUtils;
        private readonly IFcmTokenRepository _fcmTokenRepository;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IAgoraService _agoraService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            IOptions<AgoraSettings> agoraSettings,
            IAccountRepository accountRepository,
            ILanguageRepository languageRepository,
            INationalityRepository nationalityRepository,
            IMapper mapper,
            IJwtUtils jwtUtils,
            IFcmTokenRepository fcmTokenRepository,
            IRedisCacheService redisCacheService,
            IAgoraService agoraService,
            IUnitOfWork unitOfWork)
        {
            var agoraSettingsValue = agoraSettings.Value;
            _agoraSettings = agoraSettingsValue;
            _accountRepository = accountRepository;
            _languageRepository = languageRepository;
            _nationalityRepository = nationalityRepository;
            _mapper = mapper;
            _jwtUtils = jwtUtils;
            _fcmTokenRepository = fcmTokenRepository;
            _redisCacheService = redisCacheService;
            _agoraService = agoraService;
            _unitOfWork = unitOfWork;
        }

        public async Task<AccountResponse<AuthDto>> Login(LoginDto dto)
        {
            var validator = new LoginDtoValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid input", validationResult);
            }
            var account = await _accountRepository.FindByUsername(dto.Username + "@");
            if (account == null)
            {
                throw new BadRequestException("Invalid username or password");
            }

            var isValidPassword = PasswordExtension.VerifyPassword(dto.Password, account.Password);
            if (!isValidPassword)
            {
                throw new BadRequestException("Invalid username or password");
            }

            var accountDto = _mapper.Map<AuthDto>(account);

            accountDto.AccessToken = GenerateAccessTokenFromAuthentication(accountDto);

            return new AccountResponse<AuthDto>(accountDto);
        }
        public async Task<AccountResponse<AuthDto>> LoginByPhone(LoginByPhoneDto dto)
        {
            var validator = new LoginByPhoneDtoValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid input", validationResult);
            }
            var account = await _accountRepository.FindByPhone(dto.Phone);
            if (account == null)
            {
                throw new BadRequestException("invalid_phone_or_password");
            }

            var isValidPassword = PasswordExtension.VerifyPassword(dto.Password, account.Password);
            if (!isValidPassword)
            {
                throw new BadRequestException("invalid_phone_or_password");
            }
            var existingToken = await _fcmTokenRepository.FindByValue(dto.DeviceToken);
            await _fcmTokenRepository.DisableUserTokens(account.Id);

            if (existingToken == null)
            {
                var deviceToken = new FcmToken()
                {
                    AccountId = account.Id,
                    Token = dto.DeviceToken,
                    IsPrimary = true,
                };
                await _fcmTokenRepository.CreateAsync(deviceToken);
            }
            else if (existingToken.AccountId == account.Id)
            {
                await _fcmTokenRepository.DisableOtherUserToken(account.Id, existingToken.Token);
                existingToken.IsPrimary = true;
                await _fcmTokenRepository.UpdateAsync(existingToken);
            }
            else
            {
                existingToken.IsPrimary = false;
                await _fcmTokenRepository.UpdateAsync(existingToken);

                var deviceToken = new FcmToken()
                {
                    AccountId = account.Id,
                    Token = dto.DeviceToken,
                    IsPrimary = true,
                };
                await _fcmTokenRepository.CreateAsync(deviceToken);
            }
            var accountDto = _mapper.Map<AuthDto>(account);

            accountDto.AccessToken = GenerateAccessTokenFromAuthentication(accountDto);

            return new AccountResponse<AuthDto>(accountDto);
        }

        public async Task<AccountResponse<AccountListDto>> Register(AccountRegistrationDto accountRegistration)
        {
            var validator = new AccountRegistrationDtoValidator(_accountRepository);
            var validationResult = await validator.ValidateAsync(accountRegistration);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid data", validationResult);
            }
            var isEmailExist = await _accountRepository.IsEmailExist(accountRegistration.Email);
            if (isEmailExist)
            {
                throw new BadRequestException("Email already exist.");
            }
            var isPhoneExist = await _accountRepository.IsPhoneExist(accountRegistration.Phone);
            if (isPhoneExist)
            {
                throw new BadRequestException("Phone already exist.");
            }
            var accountEntity = _mapper.Map<Account>(accountRegistration);
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _accountRepository.CreateAsync(accountEntity);

                    await _agoraService.CreateAgoraUser(new AddUserAgora()
                    {
                        UserName = accountEntity.Email.Split('@')[0],
                        Password = accountRegistration.Password,
                        Nickname = accountRegistration.FirstName + " " + accountRegistration.LastName,
                    });

                    await transaction.CommitAsync();

                    //remove cache 
                    await _redisCacheService.RemoveAsync(RedisCacheKeys.DASHBOARD_LAGUAGE);
                    await _redisCacheService.RemoveAsync(RedisCacheKeys.CHART_USER);

                    var accountVm = _mapper.Map<AccountListDto>(accountEntity);

                    return new AccountResponse<AccountListDto>(accountVm);

                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync(ex.Message);
                    await transaction.RollbackAsync();
                    throw;
                }
                

            }   
            
        }

        public async Task<AccountResponse<AuthDto>> UpdateProfile(UpdateProfileDto updateProfile)
        {
            var validator = new UpdateProfileDtoValidator(_nationalityRepository);
            var validationResult = await validator.ValidateAsync(updateProfile);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid data", validationResult);
            }

            var accountEntity = await _accountRepository.FindByIdAsync(updateProfile.Id);
            if (accountEntity == null)
            {
                throw new NotFoundException($"Account '{updateProfile.Id}' not found.");
            }
            _mapper.Map(updateProfile, accountEntity);
            accountEntity.Status = 1;

            await _accountRepository.UpdateAsync(accountEntity);

            var result = _mapper.Map<AuthDto>(accountEntity);

            return new AccountResponse<AuthDto>(result);
        }

        public async Task<AccountResponse<Dictionary<string, string>>> ChangeUserLanguage(int userId, int languageId, string token)
        {
            // Validate user id not found
            var userEntity = await _accountRepository.FindByIdAsync(userId);
            if (userEntity == null)
            {
                throw new NotFoundException();
            }
            // Validate language id exist
            var languageEntity = await _languageRepository.FindByIdAsync(languageId);
            if (languageEntity == null)
            {
                throw new NotFoundException();
            }
            // Update user for language
            userEntity.ConfigLanguage = languageEntity;
            userEntity.UpdateTime = DateTime.Now;
            // Save
            await _accountRepository.UpdateAsync(userEntity);

            var claims = _jwtUtils.ValidateToken(token);
            var updatedClaims = new List<Claim>();
            foreach (var claim in claims)
            {
                if (claim.Type == "language_id")
                {
                    updatedClaims.Add(new Claim("language_id", languageEntity.Id.ToString()));
                }
                else if (claim.Type == "language_code")
                {
                    updatedClaims.Add(new Claim("language_code", languageEntity.LanguageCode.ToString()));
                }
                else
                {
                    updatedClaims.Add(claim);
                }
            }
            var newToken = _jwtUtils.GenerateAccessToken(updatedClaims);
            return new AccountResponse<Dictionary<string, string>>(new Dictionary<string, string>
                {
                    {"accessToken", newToken }
                });
        }

        private string GenerateAccessTokenFromAuthentication(AuthDto accountDto)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, accountDto.Id.ToString()),
                new Claim("firstname", accountDto.FirstName),
                new Claim("lastname", accountDto.LastName),
                new Claim(ClaimTypes.Email, accountDto.Email),
                new Claim("language_id", accountDto.LanguageId.ToString()),
                new Claim("language_code", accountDto.LanguageCode),
                new Claim(ClaimTypes.Role, accountDto.RoleId.ToString()),
            };
            if (accountDto.Image != null)
            {
                claims.Add(new Claim("image", accountDto.Image));
            }
            return _jwtUtils.GenerateAccessToken(claims);

        }
    }
}
