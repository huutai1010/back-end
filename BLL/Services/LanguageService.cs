using AutoMapper;
using BLL.DTOs;
using BLL.DTOs.Language;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;

using Common.Constants;
using Common.Interfaces;
using Common.Models;

using DAL.Entities;
using DAL.Interfaces;

using Newtonsoft.Json;

using System.Net.Http.Headers;

namespace BLL.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly INationalityRepository _nationalityRepository;
        private readonly IMapper _mapper;
        private readonly IRedisCacheService _cacheService;

        public LanguageService(ILanguageRepository languageRepository, IMapper mapper, IRedisCacheService cacheService, INationalityRepository nationalityRepository)
        {
            _languageRepository = languageRepository;
            _mapper = mapper;
            _cacheService = cacheService;
            _nationalityRepository = nationalityRepository;
        }

        public async Task<LanguageListResponse<List<LanguageDto>>> GetLanguages()
        {
            List<LanguageDto>? result;
            result = await _cacheService.Get<List<LanguageDto>>(RedisCacheKeys.LANGUAGES);

            if (result == null || !result.Any())
            {
                var rs = await _languageRepository.GetAsync(2);
                result = _mapper.Map<List<LanguageDto>>(rs);
                await _cacheService.SaveCacheAsync(RedisCacheKeys.LANGUAGES, result);
            }

            return new LanguageListResponse<List<LanguageDto>>(result);
        }
        public async Task<LanguageResponse<LanguageDto>> GetLanguageById(int id)
        {
            var itemDetail = await _languageRepository.FindByIdAsync(id);
            if (itemDetail == null)
            {
                throw new NotFoundException();
            }
            return new LanguageResponse<LanguageDto>(_mapper.Map<LanguageDto>(itemDetail));
        }
        public async Task<LanguageResponse<LanguageDto>> GetLanguageDetail(int id)
        {
            var itemDetail = await _languageRepository.GetLanguageById(id);
            if (itemDetail == null)
            {
                throw new NotFoundException();
            }
            return new LanguageResponse<LanguageDto>(_mapper.Map<LanguageDto>(itemDetail));
        }
        public async Task<LanguageResponse<LanguageDto>> PutLanguage(int id, LanguageDto languageDto)
        {
            var entity = await _languageRepository.FindByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException();
            }
            languageDto.CreateTime = entity.CreateTime;
            _mapper.Map(languageDto, entity);
            entity.UpdateTime = DateTime.Now;
            entity.Id = id;
            await _languageRepository.UpdateAsync(entity);

            // remove cache 
            await _cacheService.RemoveAsync(RedisCacheKeys.NATIONALITY);
            await _cacheService.RemoveAsync(RedisCacheKeys.LANGUAGES);

            return new LanguageResponse<LanguageDto>(_mapper.Map<LanguageDto>(entity));
        }
        public async Task<LanguageResponse<LanguageDto>> PostLanguages(AddLanguageDto languageDto)
        {
            languageDto.LanguageCode = languageDto.LanguageCode.Trim();
            // check language code is exist in configure language
            bool check = await _languageRepository.LanguageCodeIsExist(languageDto.LanguageCode);
            if (check)
            {
                throw new BadRequestException($"Language code {languageDto.LanguageCode} is exist!");
            }

            // check language code is exist in nationality
            bool checkNation = await _nationalityRepository.NationCodeIsExist(languageDto.LanguageCode);
            if (!checkNation)
            {
                throw new NotFoundException($"Language code {languageDto.LanguageCode} is not support in system!");
            }

            var entity = _mapper.Map<ConfigLanguage>(languageDto);
            entity.Status = 1;
            entity.CreateTime = DateTime.Now;
            await _languageRepository.CreateAsync(entity);

            // update status for national code 
            bool checkUpdate = await _nationalityRepository.ChangeStatusNationalityByLanguageCode(languageDto.LanguageCode);
            if (!checkUpdate)
            {
                await _languageRepository.DeleteAsync(entity);
                throw new BadRequestException("Update status nationality false!");
            }

            // remove cache 
            await _cacheService.RemoveAsync(RedisCacheKeys.NATIONALITY);
            await _cacheService.RemoveAsync(RedisCacheKeys.LANGUAGES);

            return new LanguageResponse<LanguageDto>(_mapper.Map<LanguageDto>(entity));

        }
        public async Task<LanguageResponse<LanguageDto>> DeleteLanguages(int id)
        {
            var entity = await _languageRepository.FindByIdAsync(id);
            if (entity == null)
            {
                throw new NotFoundException();
            }
            entity.Status = 0;

            entity.UpdateTime = DateTime.Now;
            await _languageRepository.UpdateAsync(entity);

            return new LanguageResponse<LanguageDto>(_mapper.Map<LanguageDto>(entity));
        }

        public async Task<StaticticalResponse<List<LanguageViewOpDto>>> GetLanguageStatictical()
        {
            List<LanguageViewOpDto>? items = await _cacheService.Get<List<LanguageViewOpDto>>(RedisCacheKeys.DASHBOARD_LAGUAGE);
            if (items == null)
            {
                var languages = _languageRepository.GetLanguageStatictical();

                var result = _mapper.Map<List<LanguageViewOpDto>>(await languages);

                int total = 0;

                foreach (var item in result)
                {
                    if (item.Quantity != null)
                    {
                        total += (int)item.Quantity;
                    }
                }

                foreach (var item in result)
                {
                    if (item.Quantity.HasValue)
                    {
                        var ratio = ((decimal)item.Quantity / (decimal)total) * 100;
                        item.Ratio = Math.Round(ratio, 1);
                    }
                }
                items = result;
                await _cacheService.SaveCacheAsync(RedisCacheKeys.DASHBOARD_LAGUAGE, items);
            }

            return new StaticticalResponse<List<LanguageViewOpDto>>(items);
        }


        #region Admin
        public async Task<LanguageListResponse<PagedResult<LanguageDto>>> GetListLanguage(QueryParameters queryParameters, bool includeDeleted)
        {
            PagedResult<LanguageDto> result = new();
            if (includeDeleted)
            {
                result = await _languageRepository.GetAsync<LanguageDto>(queryParameters, includeDeleted: includeDeleted, caching: false);
            }
            else
            {
                result = await _languageRepository.GetAsyncWithConditions<LanguageDto>(queryParameters, queryConditions: query => query.Where(x => x.Status != 0));
            }

            return new LanguageListResponse<PagedResult<LanguageDto>>(result);
        }

        public async Task<bool> ChangeStatusLanguage(int languageId, int status)
        {
            // validate status
            LanguageStatus languageStatus = (LanguageStatus)status;
            if (!Enum.IsDefined(typeof(LanguageStatus), languageStatus))
            {
                // The status variable is not a valid value in the LanguageStatus enum.
                throw new BadRequestException("Invalid LanguageStatus: " + languageStatus);
            }

            var result = await _languageRepository.ChangeStatusLanguage(languageId, status);
            if (!result)
            {
                throw new NotFoundException("language id not found!");
            }

            // remove cache 
            await _cacheService.RemoveAsync(RedisCacheKeys.NATIONALITY);
            await _cacheService.RemoveAsync(RedisCacheKeys.LANGUAGES);
            return result;
        }

        public async Task<LanguageResponse<LanguageDto>> UpdateLanguageAsync(AddLanguageDto languageDto, int languageId)
        {
            // model validation
            var validator = new AddLanguageDtoValidator();
            var validationResult = await validator.ValidateAsync(languageDto);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid data", validationResult);
            }
            var languageUpdate = _mapper.Map<ConfigLanguage>(languageDto);

            // check language id is exist
            var languageCheck = await _languageRepository.GetLanguageById(languageId);
            if (languageCheck is null)
            {
                throw new NotFoundException($"Language id {languageId} not found!");
            }
            else
            {
                var check = await _languageRepository.LanguageCodeUpdateIsExist(languageDto.LanguageCode, languageId);
                if (check)
                {
                    throw new BadRequestException($"Language code {languageDto.LanguageCode} is exist!");
                }

                languageUpdate.UpdateTime = DateTime.Now;
                languageUpdate.CreateTime = languageCheck.CreateTime;
                languageUpdate.Id = languageId;
                languageUpdate.Status = languageCheck.Status;
                _languageRepository.DetachedLanguageInstance(languageCheck);
            }

            // update langauge
            await _languageRepository.UpdateAsync(languageUpdate);

            // remove cache 
            await _cacheService.RemoveAsync(RedisCacheKeys.NATIONALITY);
            await _cacheService.RemoveAsync(RedisCacheKeys.LANGUAGES);

            return new LanguageResponse<LanguageDto>(_mapper.Map<LanguageDto>(languageUpdate));

        }

        public async Task<LanguageCodeResponse<List<LanguageCodeDto>>> GetListLanguageCode()
        {
            string URL = "https://global.metadapi.com/lang/v1/";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Set the custom header
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "37efedc127cf403fb139d6ec92a6c8d1");

            var responseLanguage = new ResponseLanguageCodeDto();

            // List data response.
            HttpResponseMessage response = client.GetAsync("languages").Result;
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                var dataObjects = await response.Content.ReadAsStringAsync();

                responseLanguage = JsonConvert.DeserializeObject<ResponseLanguageCodeDto>(dataObjects);
            }
            else
            {
                throw new BadRequestException("Error calling web api!");
            }

            client.Dispose();
            return new LanguageCodeResponse<List<LanguageCodeDto>>((List<LanguageCodeDto>)responseLanguage.data);
        }
        #endregion
    }
}
