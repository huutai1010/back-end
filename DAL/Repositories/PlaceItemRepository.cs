using AutoMapper;
using Common.Interfaces;
using Common.Models;
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
    public class PlaceItemRepository : BaseRepository<PlaceItem>, IPlaceItemRepository
    {
        private readonly AppDbContext _context;

        public PlaceItemRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService cacheService) : base(context, mapper, unitOfWork, cacheService)
        {
            _context = context;
        }

        public async Task<PlaceItem> GetItemDescription(int placecItemId, string languageCode)
        {
            var result = _context.PlaceItems.Include(x => x.ItemDescriptions.Where(q => q.PlaceItemId == placecItemId && q.LanguageCode == languageCode)).FirstOrDefault(x=>x.Id == placecItemId);
            return result;
        }

        public async Task<PlaceItem> UpdatePlaceItemAsync(PlaceItem placeItem)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var ItemDesc = await _context.ItemDescriptions.Where(x => x.PlaceItemId == placeItem.Id).ToListAsync();
                    if (ItemDesc != null)
                    {
                        _context.ItemDescriptions.RemoveRange(ItemDesc);
                    }

                    await _context.ItemDescriptions.AddRangeAsync(placeItem.ItemDescriptions);

                    _context.Entry(placeItem).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
            return await GetPlaceitemById(placeItem.Id);
        }

        public async Task<PlaceItem> GetPlaceitemById(int placeItemId)
        {
            var result = await _context.PlaceItems.Include(x => x.ItemDescriptions).SingleOrDefaultAsync(x => x.Id == placeItemId);
            return result;
        }

        public void DetachedPlaceInstance(PlaceItem placeItem)
        {
            _context.Entry(placeItem).State = EntityState.Detached;
        }
    }
}
