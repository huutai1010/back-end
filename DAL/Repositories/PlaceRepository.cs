using AutoMapper;
using Common.Constants;
using Common.Distances;
using Common.Interfaces;
using Common.Models;

using DAL.DatabaseContext;
using DAL.Entities;
using DAL.Interfaces;

using Microsoft.EntityFrameworkCore;

using System.Reflection;

namespace DAL.Repositories
{
    public class PlaceRepository : BaseRepository<Place>, IPlaceRepository
    {
        private readonly AppDbContext _context;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IMapper _mapper;
        private readonly IAzureStorageService _azureStorageService;

        public PlaceRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService redisCacheService, IAzureStorageService azureStorageService) : base(context, mapper, unitOfWork, redisCacheService)
        {
            _context = context;
            _mapper = mapper;
            _redisCacheService = redisCacheService;
            _azureStorageService = azureStorageService;
        }

        public async Task CreatePlaceImageTask(List<PlaceImage> placeImages, int placeId)
        {
            // Create Place Image
            bool isFirstElement = true;
            foreach (PlaceImage image in placeImages)
            {
                image.PlaceId = placeId;
                image.Status = 1;
                if (isFirstElement)
                {
                    image.IsPrimary = true;
                    isFirstElement = false;
                }
                else
                {
                    image.IsPrimary = false;
                }
            }
            await _context.PlaceImages.AddRangeAsync(placeImages);
        }

        public async Task CreatePlaceCategoryTask(List<PlaceCategory> placeCategories, int placeId)
        {
            // Create Place Category
            foreach (PlaceCategory category in placeCategories)
            {
                category.PlaceId = placeId;
                category.Status = 1;
            }
            await _context.PlaceCategories.AddRangeAsync(placeCategories);
        }

        public async Task CreatePlaceTimeTask(List<PlaceTime> placeTimes, int placeId)
        {
            // Create Place Time
            foreach (PlaceTime time in placeTimes)
            {
                time.PlaceId = placeId;
                time.Status = 1;
            }
            await _context.PlaceTimes.AddRangeAsync(placeTimes);
        }

        public async Task CreatePlaceDescriptionTask(List<PlaceDescription> placeDesc, int placeId)
        {
            // Create Place Desc
            foreach (PlaceDescription decs in placeDesc)
            {
                decs.PlaceId = placeId;
                decs.Status = 1;
                decs.CreateTime = DateTime.Now;
                decs.UpdateTime = DateTime.Now;
            }
            await _context.PlaceDescriptions.AddRangeAsync(placeDesc);
        }

        public async Task<Place> CreatePlaceAsync(Place place, List<string> voiceFileNames)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Create Place
                    var task = await _context.Places.AddAsync(place);

                    await _context.SaveChangesAsync();

                    var check = await _azureStorageService.CreateLeaseContainer(voiceFileNames);
                    if (check)
                    {
                        foreach (var name in voiceFileNames)
                        {
                            await _azureStorageService.DeleteAllBlobsAsync(name);
                        }
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }

                return place;
            }
        }

        public async Task<List<Place>> CreatePlaceExcelAsync(List<Place> places, List<string> containerName)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (Place place in places)
                    {

                        // Create Place
                        place.CreateTime = DateTime.Now;
                        place.Status = 1;
                        var task = await _context.Places.AddAsync(place);
                        await _context.SaveChangesAsync();
                    }
                    var check = await _azureStorageService.CreateLeaseContainer(containerName);
                    if (check)
                    {
                        foreach (var name in containerName)
                        {
                            await _azureStorageService.DeleteAllBlobsAsync(name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
                await transaction.CommitAsync();
                return places;
            }
        }

        public async Task<bool> ChangeStatusPlaceAsync(int placeId, int status)
        {
            bool check = true;
            var place = await _context.Places.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == placeId);
            if (place == null)
            {
                check = false;
            }
            else
            {
                place.Status = status;
            }
            await _context.SaveChangesAsync();
            return check;
        }

        public async Task<PagedResult<T>> GetListAsync<T>(QueryParameters queryParameters, string cacheKey)
        {
            List<Place>? items = await _redisCacheService.Get<List<Place>>(cacheKey);

            if (items is null)
            {
                items = await _context.Places.Include(x => x.PlaceCategories).Include(x => x.PlaceDescriptions).IgnoreQueryFilters().OrderByDescending(x => x.CreateTime).ToListAsync();
                await _redisCacheService.SaveCacheAsync(cacheKey, items);
            }

            var result = _mapper.Map<List<T>>(items);
            string? searchByPropName = queryParameters.SearchBy;
            string? searchText = queryParameters.Search;
            if (!string.IsNullOrEmpty(searchByPropName) && !string.IsNullOrEmpty(searchText))
            {
                var searchProp = typeof(T).GetProperty(searchByPropName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var search = queryParameters.Search?.ToLower();
                if (searchProp != null && !string.IsNullOrEmpty(search))
                {
                    result = result.Where(x =>
                        searchProp.GetValue(x, null)
                        ?.ToString()?.ToLower()
                        .Contains(search) ?? true)
                        .ToList();
                }
            }

            var totalSize = result.Count;
            var pageCount = (double)totalSize / queryParameters.PageSize;
            return new PagedResult<T>
            {
                Data = result
                        .Skip((queryParameters.PageNumber) * queryParameters.PageSize)
                        .Take(queryParameters.PageSize).ToList(),
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                PageCount = (int)Math.Ceiling(pageCount),
                TotalCount = totalSize,
            };
        }

        public async Task<Place> GetPlaceDetailAsync(int placeId)
        {
            var placeDetail = _context.Places
               .Include(x => x.PlaceTimes)
               .Include(x => x.PlaceCategories)
               .ThenInclude(y => y.Category)
               .Include(x => x.PlaceDescriptions)
               .Include(x => x.PlaceImages)
               .Include(x => x.PlaceItems)
               .ThenInclude(x => x.ItemDescriptions)
               .IgnoreQueryFilters()
               .FirstOrDefaultAsync(x => x.Id == placeId);

            return await placeDetail;
        }

        public async Task<Place> GetPlaceById(int placeId)
        {
            var placeDetail = _context.Places
               .Include(x => x.PlaceDescriptions)
               .IgnoreQueryFilters()
               .FirstOrDefaultAsync(x => x.Id == placeId);

            return await placeDetail;
        }

        public async Task<Place> GetPlaceViewByLanguage(int placeId, string languageCode)
        {
            var dayToQuery = (int)DateTime.Now.DayOfWeek + 1;
            var placeDetail = await _context.Places
              .Include(x => x.PlaceTimes.Where(x => x.DaysOfWeek == dayToQuery))
              .Include(x => x.PlaceDescriptions.Where(x => x.LanguageCode == languageCode))
              .Include(x => x.PlaceImages)
              .Include(x => x.FeedBacks.Where(x => x.IsPlace == true).OrderByDescending(x => x.CreateTime).Take(10))
              .ThenInclude(y => y.Account).ThenInclude(q => q.NationalCodeNavigation)
              .FirstOrDefaultAsync(x => x.Id == placeId);

            return placeDetail;
        }
        public async Task<Place> UpdatePlaceAsync(Place place, List<string> voiceFileNames)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // update place category
                    var placeCategory = await _context.PlaceCategories.Where(x => x.PlaceId == place.Id).ToListAsync();
                    if (placeCategory != null)
                    {
                        _context.PlaceCategories.RemoveRange(placeCategory);
                    }
                    await _context.PlaceCategories.AddRangeAsync(place.PlaceCategories);

                    // update place image
                    var placeImage = await _context.PlaceImages.Where(x => x.PlaceId == place.Id).ToListAsync();
                    if (placeImage != null)
                    {
                        _context.PlaceImages.RemoveRange(placeImage);
                    }
                    await _context.PlaceImages.AddRangeAsync(place.PlaceImages);

                    // update place desc
                    var placeDesc = await _context.PlaceDescriptions.Where(x => x.PlaceId == place.Id).IgnoreQueryFilters().ToListAsync();
                    if (placeDesc != null)
                    {
                        _context.PlaceDescriptions.RemoveRange(placeDesc);
                    }
                    await _context.PlaceDescriptions.AddRangeAsync(place.PlaceDescriptions);

                    // update place time
                    var placeTime = await _context.PlaceTimes.Where(x => x.PlaceId == place.Id).ToListAsync();
                    if (placeTime != null)
                    {
                        _context.PlaceTimes.RemoveRange(placeTime);
                    }
                    await _context.PlaceTimes.AddRangeAsync(place.PlaceTimes);

                    // update place item
                    var placeItem = await _context.PlaceItems.Where(x => x.PlaceId == place.Id).IgnoreQueryFilters().ToListAsync();
                    if (placeItem != null)
                    {
                        // update item desc
                        foreach (var item in placeItem)
                        {
                            var ItemDesc = await _context.ItemDescriptions.Where(x => x.PlaceItemId == item.Id).ToListAsync();
                            if (ItemDesc != null)
                            {
                                _context.ItemDescriptions.RemoveRange(ItemDesc);
                            }
                        }
                        _context.PlaceItems.RemoveRange(placeItem);
                    }
                    await _context.PlaceItems.AddRangeAsync(place.PlaceItems);

                    _context.Entry(place).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    var check = await _azureStorageService.CreateLeaseContainer(voiceFileNames);
                    if (check)
                    {
                        foreach (var name in voiceFileNames)
                        {
                            await _azureStorageService.DeleteAllBlobsAsync(name);
                        }
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
            return place;
        }

        public async Task<bool> IsLanguageIDExist(PlaceDescription placeDesc)
        {
            var check = _context.ConfigLanguages.FirstOrDefaultAsync(x => x.LanguageCode == placeDesc.LanguageCode);
            if (await check == null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsCategoryExist(PlaceCategory placeCate)
        {
            var check = _context.Categories.FirstOrDefaultAsync(x => x.Id == placeCate.CategoryId);
            if (await check == null)
            {
                return true;
            }
            return false;
        }

        public async Task<List<Place>> GetTopPlacesAsync(string languageCode, int topCount)
        {
            return await _context.Places
                .Include(q => q.PlaceDescriptions.Where(x => x.LanguageCode == languageCode))
                .Include(q => q.PlaceImages.Where(x => x.IsPrimary))
                .Include(q => q.FeedBacks)
                .OrderByDescending(o => o.Rate)
                .Where(q => q.PlaceDescriptions.Any(x => x.LanguageCode == languageCode) && q.Status == (int)PlaceStatus.active)
                .Take(topCount)
                .ToListAsync();
        }
        public async Task<List<Place>> GetPlaceNearVisitor(GeoPoint currentLoc, string languageCode)
        {
            var result = _context.Places
               .Include(x => x.FeedBacks)
                .Include(q => q.PlaceDescriptions.Where(x => x.LanguageCode == languageCode))
               .Include(q => q.PlaceImages.Where(x => x.IsPrimary))
               .Where(q => q.Price != 0 && q.PlaceDescriptions.Any(x => x.LanguageCode == languageCode) && q.Status == (int)PlaceStatus.active)
               .AsEnumerable()
               .OrderBy(o => DistanceCalculator.calculate(currentLoc, new GeoPoint { Latitude = Decimal.ToDouble(o.Latitude), Longitude = Decimal.ToDouble(o.Longitude) }))
               .Take(5)
               .ToList();
            return await Task.FromResult(result);
        }

        public async Task<List<Place>> SearchPlaces(int[] category, string languageCode)
        {
            var dayToQuery = (int)DateTime.Now.DayOfWeek + 1;
            return await _context.Places
                .Include(q => q.FeedBacks)
              .Include(q => q.PlaceDescriptions.Where(x => x.LanguageCode == languageCode))
             .Include(q => q.PlaceImages.Where(x => x.IsPrimary))
             .Include(q => q.PlaceCategories)
             .Where(x => x.PlaceCategories.Any(x => category.Contains(x.CategoryId)) && x.PlaceDescriptions.Any(x => x.LanguageCode == languageCode) && x.Status == (int)PlaceStatus.active)
              .Include(x => x.PlaceTimes.Where(x => x.DaysOfWeek == dayToQuery))
             .ToListAsync();
        }

        public async Task<Place> GetRatePlace(int? placeId)
        {
            var tourRate = await _context.Places.Where(x => x.Id == placeId).FirstAsync();
            return tourRate;
        }

        public async Task<List<Place>> GetListMultiplePlacesByListIds(List<int> placeIds)
        {
            var result = await _context.Places
                .Where(q => placeIds.Contains(q.Id)).ToListAsync();

            return result;
        }

        public void DetachedPlaceInstance(Place place)
        {
            _context.Entry(place).State = EntityState.Detached;
        }

        public async Task<Place> GetVoiceScreenDataByLanguage(int placeId, string languageCode)
        {
            var placeDetail = await _context.Places
              .Include(x => x.PlaceDescriptions.Where(x => x.LanguageCode == languageCode))
              .Include(x => x.PlaceImages)
              .FirstOrDefaultAsync(x => x.Id == placeId);
            return placeDetail;
        }

        public async Task<PlaceDescription> UpdatePlaceDescVoiceFile(MessageResponseModel responseModel)
        {
            var result = await _context.PlaceDescriptions.SingleOrDefaultAsync(x => x.VoiceFile == Path.GetFileNameWithoutExtension(responseModel.FileName));
            if (result != null)
            {
                result.VoiceFile = responseModel.Message;
                await _context.SaveChangesAsync();
            }
            return result;
        }

        public async Task<bool> IsPlaceExistInTour(int placeId)
        {
            var tour = await _context.ItineraryPlaces.Where(x => x.PlaceId == placeId).ToListAsync();
            if (tour == null || tour.Count == 0)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> IsPlaceExist(int placeId)
        {
            var place = await _context.Places.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == placeId);
            if (place == null)
            {
                return false;
            }
            return true;
        }

        public async Task<List<Place>> GetPlacesNearPlace(int currentPlaceId, GeoPoint currentLoc, string languageCode)
        {
            var result = _context.Places.Include(q => q.FeedBacks)
                .Include(q => q.PlaceDescriptions.Where(x => x.LanguageCode == languageCode))
               .Include(q => q.PlaceImages.Where(x => x.IsPrimary))
               .Where(q => q.PlaceDescriptions.Any(x => x.LanguageCode == languageCode) && q.Id != currentPlaceId && q.Status == (int)PlaceStatus.active)
               .AsEnumerable()
               .OrderBy(o => DistanceCalculator.calculate(currentLoc, new GeoPoint { Latitude = Decimal.ToDouble(o.Latitude), Longitude = Decimal.ToDouble(o.Longitude) }))
               .Take(5)
               .ToList();
            return await Task.FromResult(result);
        }

        public async Task<bool> UpdatePlaceDescStatus(string voiceName, int status)
        {
            try
            {
                var placeDesc = await _context.PlaceDescriptions.Where(x => x.VoiceFile == voiceName).SingleOrDefaultAsync();
                if (placeDesc != null)
                {
                    placeDesc.Status = status;
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> IsplaceDescPrepare(int placeId)
        {
            var check = true;
            var placeDesc = await _context.PlaceDescriptions.Where(x => x.Status == 1).FirstOrDefaultAsync(x => x.PlaceId == placeId);
            if (placeDesc is null)
            {
                check = false;
            }
            return check;
        }

        public async Task<bool> IsAnyPlaceDescActive(int placeId)
        {
            var check = true;
            var placeDesc = await _context.PlaceDescriptions.Where(x => x.Status == 2).FirstOrDefaultAsync(x => x.PlaceId == placeId);
            if (placeDesc is null)
            {
                check = false;
            }
            return check;
        }

        public async Task<PlaceDescription> GetPlaceNameByLanguageCode(int placeId, string languageCode)
        {
            var entity = _context.PlaceDescriptions.Where(x => x.LanguageCode == languageCode).FirstOrDefaultAsync(x => x.PlaceId == placeId);
            return await entity;
        }
}
}
