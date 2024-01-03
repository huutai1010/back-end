using BLL.DTOs.Place;
using BLL.DTOs.Place.PlaceItem;
using BLL.Responses;
using Common.Distances;
using Common.Models;
using Microsoft.AspNetCore.Http;

namespace BLL.Interfaces
{
    public interface IPlaceService
    {
        public Task<PlaceListResponse<PagedResult<PlaceDto>>> GetListAsync(QueryParameters parameters);
        public Task<PlaceResponse<PlaceDetailDto>> GetDetailAsync(int placeId);
        public Task<PlaceResponse<PlaceViewDto>> GetPlace(int placeId, int accountId, string languageCode);
        Task<PlaceResponse<PlaceDto>> CreateAsync(CreatePlaceDto placeDto);
        Task<PlaceResponse<PlaceDetailDto>> UpdateAsync(UpdatePlaceDto placeDto, int placeId);
        Task<bool> ChangeStatusAsync(int placeId, int status);
        Task<bool> ImportExcelForPlace(MemoryStream stream, List<IFormFile> images, List<IFormFile> voiceFiles);
        Task<PlaceListResponse<List<TopPlaceDto>>> GetPlaceNearVisitor(GeoPoint currentLoc, string languageCode);
        Task<PlaceListResponse<List<TopPlaceDto>>> GetTopPlacesAsync(string languageCode, int topCount);
        Task<PlaceListResponse<List<SearchPlaceDto>>> SearchPlaces(int[] category, string languageCode);
        public Task<PlaceResponse<PlaceVoiceDto>> GetVoiceScreenData(int placeId, string languageCode);
        public Task<PlaceListResponse<PagedResult<TopPlaceDto>>> GetPlaces(QueryParameters parameters, string languageCode);
        public Task<PlaceItemsResponse<List<PlaceItemDto>>> GetBeaconsByPlaceId(int placeId, string languageCode);
        public Task<PlaceItemResponse<PlaceItemViewDto>> GetBeaconData(int placeItemId, string languageCode);
        Task<PlaceListResponse<List<TopPlaceDto>>> GetPlacesNearPlace(int placeId, string languageCode);
        Task<PlaceItemResponse<PlaceItemViewDto>> UpdatePlaceItemAsync(PlaceUpdateItemDto placeUpdateItemDto, int placeitemId);
        Task<PlaceItemResponse<PlaceItemViewDto>> GetPlaceItemAsync(int placeItemId);
        Task<bool> ConvertMp3ToM3u8V2(List<IFormFile> mp3Files);
    }
}
