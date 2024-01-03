using AutoMapper;

using Common.Interfaces;

using DAL.DatabaseContext;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class LanguageRepository : BaseRepository<ConfigLanguage>, ILanguageRepository
    {
        private readonly AppDbContext _context;
        public LanguageRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService redisCacheService) : base(context, mapper, unitOfWork, redisCacheService)
        {
            _context = context;
        }

        public async Task<ConfigLanguage> GetLanguageById(int id)
        {
            ConfigLanguage? configLanguage = await _context.ConfigLanguages.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id== id);
            return configLanguage;
        }

        public void DetachedLanguageInstance(ConfigLanguage language)
        {
            _context.Entry(language).State = EntityState.Detached;
        }

        public async Task<List<ConfigLanguage>> GetLanguageStatictical()
        {
            var languages = _context.ConfigLanguages
                .Include(x => x.Accounts)
                .ToListAsync();

            return await languages;
        }

        public async Task<bool> LanguageCodeIsExist(string languageCode)
        {
            var entity = await _context.ConfigLanguages.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.LanguageCode == languageCode);
            if (entity == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> LanguageCodeUpdateIsExist(string languageCode, int languageId)
        {
            var language = await _context.ConfigLanguages.Where(x => x.LanguageCode == languageCode && x.Id != languageId).SingleOrDefaultAsync();
            if (language == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> ChangeStatusLanguage(int languageid, int status)
        {
            var category = await _context.ConfigLanguages
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(x => x.Id == languageid);
            if (category == null)
            {
                return false;
            }
            else
            {
                category.Status = status;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ConfigLanguage> GetLanguageByLanguageCode(string languageCode)
        {
            ConfigLanguage? configLanguage = await _context.ConfigLanguages.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.LanguageCode == languageCode);
            return configLanguage;
        }

        public async Task<List<ConfigLanguage>> GetAsync(int status)
        {
            var languages = _context.ConfigLanguages.Where(x =>x.Status == status)
                   .ToListAsync();

            return await languages;
        }

        public async Task<List<ConfigLanguage>> GetAllAsync()
        {
            var languages = _context.ConfigLanguages.ToListAsync();

            return await languages;
        }
    }
}
