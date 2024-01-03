using Common.Distances;
using Common.Models;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IPlaceRepository : IBaseRepository<Place>
    {
        Task<PagedResult<T>> GetListAsync<T>(QueryParameters queryParameters, string cacheKey);
        Task<Place> CreatePlaceAsync(Place place, List<string> voiceFileNames);
        Task<bool> ChangeStatusPlaceAsync(int placeId, int status);
        Task<Place> UpdatePlaceAsync(Place place, List<string> voiceFileNames);
        Task<Place> GetPlaceDetailAsync(int placeId);
        Task<List<Place>> CreatePlaceExcelAsync(List<Place> places, List<string> containerName);
        void DetachedPlaceInstance(Place place);
        Task<bool> IsPlaceExistInTour(int placeId);
        Task<bool> IsPlaceExist(int placeId);
        Task<bool> IsAnyPlaceDescActive(int placeId);
        Task<Place> GetPlaceById(int placeId);

        Task<Place> GetPlaceViewByLanguage(int placeId, string languageCode);
        Task<bool> IsLanguageIDExist(PlaceDescription placeDesc);
        Task<bool> IsCategoryExist(PlaceCategory placeCate);

        Task<List<Place>> GetPlaceNearVisitor(GeoPoint currentLoc, string languageCode);
        Task<List<Place>> GetTopPlacesAsync(string languageCode, int topCount);
        Task<List<Place>> GetListMultiplePlacesByListIds(List<int> placeIds);

        Task<List<Place>> SearchPlaces(int[] category, string languageCode);
        Task<Place> GetRatePlace(int? placeId);
        Task<Place> GetVoiceScreenDataByLanguage(int placeId, string languageCode);
        Task<PlaceDescription> UpdatePlaceDescVoiceFile(MessageResponseModel responseModel);
        Task<List<Place>> GetPlacesNearPlace(int currentPlaceId, GeoPoint currentLoc, string languageCode);
        Task<bool> UpdatePlaceDescStatus(string voiceName, int status);
        Task<bool> IsplaceDescPrepare(int placeId);
        Task<PlaceDescription> GetPlaceNameByLanguageCode(int placeId, string languageCode);
    }
}
