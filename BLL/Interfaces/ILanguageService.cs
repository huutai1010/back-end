using BLL.DTOs.Language;
using BLL.Responses;
using Common.Models;

namespace BLL.Interfaces
{
        public interface ILanguageService
        {
                Task<LanguageListResponse<List<LanguageDto>>> GetLanguages();
                Task<LanguageResponse<LanguageDto>> GetLanguageById(int id);
                Task<LanguageResponse<LanguageDto>> GetLanguageDetail(int id);
                Task<LanguageResponse<LanguageDto>> PutLanguage(int id, LanguageDto languageDto);
                Task<LanguageResponse<LanguageDto>> PostLanguages(AddLanguageDto languageDto);
                Task<LanguageResponse<LanguageDto>> DeleteLanguages(int id);
                Task<StaticticalResponse<List<LanguageViewOpDto>>> GetLanguageStatictical();
                Task<LanguageListResponse<PagedResult<LanguageDto>>> GetListLanguage(QueryParameters queryParameters, bool includeDeleted);
                Task<LanguageCodeResponse<List<LanguageCodeDto>>> GetListLanguageCode();
                Task<bool> ChangeStatusLanguage(int languageId, int status);
                Task<LanguageResponse<LanguageDto>> UpdateLanguageAsync(AddLanguageDto languageDto, int languageId);
        }
}
