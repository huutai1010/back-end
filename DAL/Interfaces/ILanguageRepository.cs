using DAL.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ILanguageRepository : IBaseRepository<ConfigLanguage>
    {
        Task<List<ConfigLanguage>> GetLanguageStatictical();
        Task<bool> LanguageCodeIsExist(string languageCode);
        Task<bool> ChangeStatusLanguage(int languageid, int status);
        void DetachedLanguageInstance(ConfigLanguage language);
        Task<ConfigLanguage> GetLanguageById(int id);
        Task<ConfigLanguage> GetLanguageByLanguageCode(string languageCode);
        Task<List<ConfigLanguage>> GetAsync(int status);
        Task<List<ConfigLanguage>> GetAllAsync();
        Task<bool> LanguageCodeUpdateIsExist(string languageCode, int languageId);
    }
}
