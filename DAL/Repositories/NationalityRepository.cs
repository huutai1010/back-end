using AutoMapper;
using Common.Interfaces;
using DAL.DatabaseContext;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class NationalityRepository : BaseRepository<Nationality>, INationalityRepository
    {
        private readonly AppDbContext _context;
        public NationalityRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService redisCacheService) : base(context, mapper, unitOfWork, redisCacheService)
        {
            _context = context;
        }

        public async Task<bool> ChangeStatusNationalityByLanguageCode(string languageCode)
        {
            var entity = await _context.Nationalities.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.NationalCode == languageCode);
            if (entity is null)
            {
                return false;
            }
            else if (entity.Status == 1)
            {
                entity.Status = 0;
                _context.SaveChanges();
            }
            else
            {
                entity.Status = 1;
                _context.SaveChanges();
            }
            return true;
        }

        public async Task<bool> NationCodeIsExist(string nationalCode)
        {
            var entity = await _context.Nationalities.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.NationalCode == nationalCode);
            if (entity is null)
            {
                return false;
            }
            return true;
        }

        public async Task<List<Nationality>> GetListNationalityAsync()
        {
            var nationalities = await _context.Nationalities.IgnoreQueryFilters().Include(x => x.Accounts).ToListAsync();
            return nationalities;
        }
    }
}
