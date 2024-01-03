using Aspose.Cells;
using AutoMapper;
using BLL.DTOs.Currency;
using BLL.DTOs.Place;
using BLL.DTOs.Place.PlaceItem;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;
using Common.Constants;
using Common.Distances;
using Common.Interfaces;
using Common.Models;
using DAL.Entities;
using DAL.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BLL.DTOs.Place.PlaceDescription;
using BLL.DTOs.Place.Excel;
using static BLL.DTOs.Place.Excel.ExcelPlaceDto;
using Common.Models.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using static BLL.DTOs.Category.CategoryUpdateDto;
using Microsoft.Identity.Client;

namespace BLL.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly IMarkPlaceRepository _markPlaceRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IPlaceItemRepository _placeItemRepository;
        private readonly IFirebaseStorageService _firebaseStorageService;
        
        private readonly IRedisCacheService _redisCacheService;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IRabbitmqService _rabbitmqService;
        private readonly IMapper _mapper;

        public PlaceService(IMarkPlaceRepository markPlaceRepository,
            IPlaceRepository placeRepository,
            IMapper mapper,
            IFirebaseStorageService firebaseStorageService,
            IRedisCacheService redisCacheService,IPlaceItemRepository placeItemRepository,
            ILanguageRepository languageRepository,
            IAzureStorageService azureStorageService,
            IRabbitmqService rabbitmqService)
        {
            _markPlaceRepository = markPlaceRepository;
            _placeRepository = placeRepository;
            _firebaseStorageService = firebaseStorageService;
            _redisCacheService = redisCacheService;
            _languageRepository = languageRepository;
            _placeItemRepository = placeItemRepository;
            _mapper = mapper;
            _azureStorageService = azureStorageService;
            _rabbitmqService = rabbitmqService;
        }

        public async Task<PlaceListResponse<List<TopPlaceDto>>> GetTopPlacesAsync(string languageCode, int topCount = 10)
        {
            List < TopPlaceDto >? result;
            result = await _redisCacheService.Get<List<TopPlaceDto>>(RedisCacheKeys.TOPPLACES +languageCode);

            if (result == null || !result.Any())
            {
                var entities = await _placeRepository.GetTopPlacesAsync(languageCode, topCount);
                result = _mapper.Map<List<TopPlaceDto>>(entities);
                await _redisCacheService.SaveCacheAsync(RedisCacheKeys.TOPPLACES +languageCode, result);
            }

            return new PlaceListResponse<List<TopPlaceDto>>(result);
        }

        public async Task<PlaceResponse<PlaceDetailDto>> GetDetailAsync(int placeId)
        {
            var placeResult = await _placeRepository.GetPlaceDetailAsync(placeId);

            if (placeResult == null)
            {
                throw new NotFoundException();
            }

            var placeResponse = _mapper.Map<PlaceDetailDto>(placeResult);

            if (placeResponse.PlaceDescriptions != null)
            {
                foreach (var placeDesc in placeResponse.PlaceDescriptions)
                {
                    var configLanguage = await _languageRepository.GetLanguageByLanguageCode(placeDesc.LanguageCode);
                    placeDesc.LanguageName = configLanguage.Name;
                    placeDesc.LanguageIcon = configLanguage.Icon;
                }
            }

            return new PlaceResponse<PlaceDetailDto>(placeResponse);
        }

        public async Task<PlaceListResponse<PagedResult<PlaceDto>>> GetListAsync(QueryParameters parameters)
        {
            var places = await _placeRepository.GetAsyncWithConditions<PlaceDto>(parameters, includeDeleted: true, queryConditions: query =>
            {
                return query
                    .Include(x => x.PlaceCategories)
                    .Include(x => x.PlaceDescriptions)
                    .OrderByDescending(x => x.CreateTime);
            });
            foreach (var place in places.Data)
            {
                foreach (var language in place.LanguageList)
                {
                    language.LanguageCode = language.LanguageCode.Trim();
                    language.Name = _languageRepository.GetLanguageByLanguageCode(language.LanguageCode).Result.Name;
                }
            }
            return new PlaceListResponse<PagedResult<PlaceDto>>(places);
        }

        public async Task<PlaceResponse<PlaceDto>> CreateAsync(CreatePlaceDto placeDto)
        {
            // model validation
            var validator = new CreatePlaceDtoValidator();
            var validationResult = await validator.ValidateAsync(placeDto);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid data", validationResult);
            }
            var placeCreate = _mapper.Map<Place>(placeDto);

            //list vocie file to create container
            List<string> voiceFileNames = new();

            // Check date time
            for (var i = 1; i < 8; i++)
            {
                PlaceTime? item = new();
                try
                {
                    item = placeCreate.PlaceTimes.SingleOrDefault(x => x.DaysOfWeek == i);
                }
                catch (Exception ex)
                {
                    throw new BadRequestException("date of week is duplicate!");
                }
                if (item is null)
                {
                    throw new BadRequestException("date of week is missing!");
                }
            }

            // Check for duplicate CategoryId
            bool hasDuplicateCategoryId = placeCreate.PlaceCategories
                .GroupBy(pd => pd.CategoryId)
                .Any(g => g.Count() > 1);

            if (hasDuplicateCategoryId)
            {
                throw new BadRequestException("Duplicate Category id found in the list");
            }

            // check category id is exist
            foreach (var item in placeCreate.PlaceCategories)
            {
                if (await _placeRepository.IsCategoryExist(item))
                {
                    throw new BadRequestException($"Category id ({item.CategoryId}) is not exist!");
                }
            };

            // Check for duplicate day of week
            bool hasDuplicateday = placeCreate.PlaceTimes
                .GroupBy(pd => pd.DaysOfWeek)
                .Any(g => g.Count() > 1);
            if (hasDuplicateday)
            {
                throw new BadRequestException("Duplicate day found in the list");
            }

            // check language code of place description is exist
            foreach (var item in placeCreate.PlaceDescriptions)
            {
                var check = await _languageRepository.LanguageCodeIsExist(item.LanguageCode);
                if(!check)
                {
                    throw new BadRequestException($"Language code {item.LanguageCode} is not support in system!");
                }
                if(item.VoiceFile != null)
                {
                    voiceFileNames.Add(Path.GetFileNameWithoutExtension(item.VoiceFile));
                }

                // create language for place item base on support language of place description
                foreach (var placeItem in placeCreate.PlaceItems)
                {
                    var itemDesc = placeItem.ItemDescriptions.FirstOrDefault(x => x.LanguageCode == item.LanguageCode);
                    if (itemDesc is null)
                    {
                        placeItem.ItemDescriptions.Add(new ItemDescription { LanguageCode = item.LanguageCode, NameItem = placeItem.Name });
                    }
                }
            }

            // check language code is duplicate for place description
            bool hasDuplicateLanguageCode = placeCreate.PlaceDescriptions
                .GroupBy(pd => pd.LanguageCode)
                .Any(g => g.Count() > 1);

            if (hasDuplicateLanguageCode)
            {
                throw new BadRequestException("Duplicate Language code in the place description!");
            }

            // check language code of Item description is exist
            foreach (var item in placeCreate.PlaceItems)
            {
                foreach (var itemDesc in item.ItemDescriptions)
                {
                    var check = await _languageRepository.LanguageCodeIsExist(itemDesc.LanguageCode);
                    if (!check)
                    {
                        throw new BadRequestException($"Language code in item: {itemDesc.LanguageCode} is not support in system!");
                    }
                }

                // check language code is duplicate for Item description
                bool hasDuplicateLanguageCodeItem = item.ItemDescriptions
                    .GroupBy(pd => pd.LanguageCode)
                    .Any(g => g.Count() > 1);

                if (hasDuplicateLanguageCodeItem)
                {
                    throw new BadRequestException("Duplicate Language code in the place item description!");
                }
            }

            var place = _placeRepository.CreatePlaceAsync(placeCreate, voiceFileNames);

            //remove cache
            if (place != null)
            {
                var languages = await _languageRepository.GetAllAsync();
                foreach (var language in languages)
                {
                    await _redisCacheService.RemoveAsync(RedisCacheKeys.TOPPLACES + language.LanguageCode);
                }
            }

            return new PlaceResponse<PlaceDto>(_mapper.Map<PlaceDto>(await place));
        }

        public async Task<PlaceResponse<PlaceDetailDto>> UpdateAsync(UpdatePlaceDto placeDto, int placeId)
        {
            // model validation
            var validator = new UpdatePlaceDtoValidator();
            var validationResult = await validator.ValidateAsync(placeDto);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid data", validationResult);
            }

            // map place dto to entity
            var placeUpdate = _mapper.Map<Place>(placeDto);

            //list vocie file to create container
            List<string> voiceFileNames = new();

            // Check date time
            for (var i = 1; i < 8; i++)
            {
                PlaceTime? item = new();
                try
                {
                     item = placeUpdate.PlaceTimes.SingleOrDefault(x => x.DaysOfWeek == i);
                }
                catch (Exception ex)
                {
                    throw new BadRequestException("date of week is duplicate!");
                }
                if (item is null)
                {
                    throw new BadRequestException("date of week is missing!");
                }
            }

            // Check Place Id
            var placeCheck = _placeRepository.GetPlaceById(placeId).Result;
            if (placeCheck is null)
            {
                throw new NotFoundException(nameof(Place), placeId);
            }
            else
            {
                placeUpdate.Id = placeId;
                placeUpdate.CreateTime = placeCheck.CreateTime;
                placeUpdate.Rate= placeCheck.Rate;
                placeUpdate.Status= placeCheck.Status;

                foreach(var placeDesc in placeCheck.PlaceDescriptions)
                {
                    var placeDescUpdate = placeUpdate.PlaceDescriptions.FirstOrDefault(x => x.LanguageCode == placeDesc.LanguageCode.Trim());
                    if (placeDescUpdate != null)
                    {
                        placeDescUpdate.CreateTime = placeDesc.CreateTime;
                    }
                }

                // check all place desc is deactive
                var checkDesc = placeUpdate.PlaceDescriptions.Where(x => x.Status == 0).ToList();
                if (checkDesc.Count() == placeUpdate.PlaceDescriptions.Count())
                {
                    placeUpdate.Status = 0;
                }

                _placeRepository.DetachedPlaceInstance(placeCheck);
            }

            // Check for duplicate CategoryId
            bool hasDuplicateCategoryId = placeUpdate.PlaceCategories
                .GroupBy(pd => pd.CategoryId)
                .Any(g => g.Count() > 1);

            if (hasDuplicateCategoryId)
            {
                throw new BadRequestException("Duplicate Category id found in the list");
            }

            // check category id is exist
            foreach (var item in placeUpdate.PlaceCategories)
            {
                item.PlaceId = placeId;
                if (await _placeRepository.IsCategoryExist(item))
                {
                    throw new BadRequestException($"Category id ({item.CategoryId}) is not exist!");
                }
            };


            foreach (var item in placeUpdate.PlaceImages)
            {
                item.PlaceId = placeId;
            };

            // Check for duplicate day of week
            bool hasDuplicateday = placeUpdate.PlaceTimes
                .GroupBy(pd => pd.DaysOfWeek)
                .Any(g => g.Count() > 1);

            if (hasDuplicateday)
            {
                throw new BadRequestException("Duplicate day found in the list");
            }

            foreach (var item in placeUpdate.PlaceTimes)
            {
                item.PlaceId = placeId;
            };

            // check language code of place description is exist
            foreach (var item in placeUpdate.PlaceDescriptions)
            {
                var checkCode = await _languageRepository.LanguageCodeIsExist(item.LanguageCode);
                if (!checkCode)
                {
                    throw new BadRequestException($"Language code {item.LanguageCode} is not support in system!");
                }

                if (item.VoiceFile != null)
                {
                    if (item.VoiceFile.Contains(".mp3"))
                    {
                        voiceFileNames.Add(Path.GetFileNameWithoutExtension(item.VoiceFile));
                        placeUpdate.Status = 1;
                        item.Status = 1;
                    }
                }

                // create language for place item base on support language of place description
                foreach (var placeItem in placeUpdate.PlaceItems)
                {
                    var itemDesc = placeItem.ItemDescriptions.FirstOrDefault(x => x.LanguageCode == item.LanguageCode);
                    if (itemDesc is null)
                    {
                        placeItem.ItemDescriptions.Add(new ItemDescription { LanguageCode = item.LanguageCode, NameItem = placeItem.Name });
                    }
                }
            }

            // check language code is duplicate
            bool hasDuplicateLanguageCode = placeUpdate.PlaceDescriptions
                .GroupBy(pd => pd.LanguageCode)
                .Any(g => g.Count() > 1);

            if (hasDuplicateLanguageCode)
            {
                throw new BadRequestException("Duplicate Language code in the place description!");
            }

            // check language code of Item description is exist
            foreach (var item in placeUpdate.PlaceItems)
            {
                foreach (var itemDesc in item.ItemDescriptions)
                {
                    var check2 = await _languageRepository.LanguageCodeIsExist(itemDesc.LanguageCode);
                    if (!check2)
                    {
                        throw new BadRequestException($"Language code in item: {itemDesc.LanguageCode} is not support in system!");
                    }
                }

                // check language code is duplicate for Item description
                bool hasDuplicateLanguageCodeItem = item.ItemDescriptions
                    .GroupBy(pd => pd.LanguageCode)
                    .Any(g => g.Count() > 1);

                if (hasDuplicateLanguageCodeItem)
                {
                    throw new BadRequestException("Duplicate Language code in the place Item description!");
                }
            }

            var check = await _placeRepository.UpdatePlaceAsync(placeUpdate, voiceFileNames);
            if (check == null)
            {
                throw new BadRequestException(nameof(placeUpdate));
            }
            else
            {
                //remove cache
                var languages = await _languageRepository.GetAllAsync();
                foreach (var language in languages)
                {
                    await _redisCacheService.RemoveAsync(RedisCacheKeys.TOPPLACES + language.LanguageCode);
                }
            }
            return await GetDetailAsync(placeId);
        }

        public async Task<bool> ChangeStatusAsync(int placeId, int status)
        {
            // validate status
            PlaceStatus placeStatus = (PlaceStatus)status;
            if (!Enum.IsDefined(typeof(PlaceStatus), placeStatus))
            {
                // The status variable is not a valid value in the LanguageStatus enum.
                throw new BadRequestException("Invalid Place Status: " + placeStatus);
            }

            var placeResult = await _placeRepository.GetPlaceDetailAsync(placeId);
            if (placeResult == null)
            {
                throw new NotFoundException();
            }

            if (placeResult.Status == 1)
            {
                var check = await _placeRepository.IsPlaceExistInTour(placeId);
                if (check)
                {
                    throw new BadRequestException("Place is exist in active itinerary!");
                }
            }

            if (status == 2)
            {
                if (await _placeRepository.IsplaceDescPrepare(placeId))
                {
                    throw new BadRequestException("Place description in place is converting can not active!");
                }

                if (!await _placeRepository.IsAnyPlaceDescActive(placeId))
                {
                    throw new BadRequestException("Has not any place description in place is active!");
                }
            }
            
            var result = await _placeRepository.ChangeStatusPlaceAsync(placeId, status);
            if (result)
            {
                var languages = await _languageRepository.GetAllAsync();
                foreach (var language in languages)
                {
                    await _redisCacheService.RemoveAsync(RedisCacheKeys.TOPPLACES + language.LanguageCode);
                }
            }
            return result;
        }

        public async Task<PlaceResponse<PlaceViewDto>> GetPlace(int placeId, int accountId, string languageCode)
        {

            var placeResult = await _placeRepository.GetPlaceViewByLanguage(placeId, languageCode);

            if (placeResult == null)
            {
                throw new NotFoundException();
            }

            var resultDto = _mapper.Map<PlaceViewDto>(placeResult);

            var checkFavorite = await _markPlaceRepository.IsPlaceFavorite(accountId, placeId);
            resultDto.IsFavorite = checkFavorite;

            List<CurrencyDto> exchangesData;

#if DEBUG
            var now = DateTime.Now.Millisecond;
            exchangesData = new List<CurrencyDto>{
                new CurrencyDto
                {
Timestamp = now,
Currency = "CNH", Value = 7.3101,
                },
                new CurrencyDto
                {
Timestamp = now,
Currency = "USD", Value = 1,
                },new CurrencyDto
                {
Timestamp = now,
Currency = "JPY", Value = 148.836175,
                },
                new CurrencyDto
                {
Timestamp = now,
Currency = "VND", Value = 24375,
                },
            };
#else
            exchangesData = await _redisCacheService.Get<List<CurrencyDto>>("currency");
#endif

            resultDto.Exchanges = new Dictionary<string, double>();
            exchangesData!.ForEach(item =>
            {
                var exchangePrice = Math.Round(item.Value * decimal.ToDouble(resultDto.Price), 3);
                resultDto.Exchanges.Add(item.Currency, exchangePrice);
            });

            return new PlaceResponse<PlaceViewDto>(resultDto);
        }


        public async Task<bool> ImportExcelForPlace(MemoryStream stream, List<IFormFile> images, List<IFormFile> voiceFiles)
        {
            try
            {
                if (stream == null)
                {
                    throw new ArgumentNullException(nameof(stream));
                }

                // load excel file 
                Workbook wb = new(stream);

                // get all worksheets
                WorksheetCollection collection = wb.Worksheets;

                // Get value from excel
                var placeList = ReadPlaceFromExcel(collection[0]);
                var placeCategoryList = ReadPlaceCategoryFromExcel(collection[1]);
                var placeTimeList = ReadPlaceTimeFromExcel(collection[2]);
                var PlaceItemList = ReadPlaceItemFromExcel(collection[3], images);
                var itemDescList = ReadItemDescFromExcel(collection[4], voiceFiles);
                var PlaceImageList = ReadPlaceImageFromExcel(collection[5], images);
                var placeDescOption = ReadPlaceDescFromExcel(collection[6], voiceFiles);

                foreach (var placeItem in PlaceItemList.Result)
                {
                    placeItem.ItemDescriptions = itemDescList.Result.Where(x => x.ExcelItemId == placeItem.ExcelItemId).ToList();
                }

                // add place relation for place
                foreach (var place in placeList.Result)
                {
                    place.PlaceCategories = placeCategoryList.Result.Where(x => x.PlaceExcelId == place.ExcelPlaceId).ToList();
                    place.PlaceImages = PlaceImageList.Result.Where(x => x.ExcelPlaceId == place.ExcelPlaceId).ToList();
                    place.PlaceTimes = placeTimeList.Result.Where(x => x.ExcelPlaceId == place.ExcelPlaceId).ToList();
                    place.PlaceDescriptions = placeDescOption.Result.excelPlaceDescs.Where(x => x.ExcelPlaceId == place.ExcelPlaceId).ToList();
                    place.PlaceItems = PlaceItemList.Result.Where(x => x.ExcelPlaceId == place.ExcelPlaceId).ToList();

                    // model validation
                    var validator = new ExcelPlaceDtoValidator();
                    var validationResult = await validator.ValidateAsync(place);
                    if (!validationResult.IsValid)
                    {
                        throw new BadRequestException("Invalid data", validationResult);
                    }
                }

                var placeEntityList = _mapper.Map<List<Place>>(placeList.Result);

                // add place list to database
                var task = _placeRepository.CreatePlaceExcelAsync(placeEntityList, placeDescOption.Result.containerName);
                if (await task == null)
                {
                    return false;
                }
                else
                {
                    var check = await PublistMessageConvertFile(placeDescOption.Result.VoiceFiles);
                    if (check)
                    {
                        var languages = await _languageRepository.GetAllAsync();
                        foreach (var language in languages)
                        {
                            await _redisCacheService.RemoveAsync(RedisCacheKeys.TOPPLACES + language.LanguageCode);
                        }
                    }
                    return check;
                }
            }
            catch (AggregateException ae)
            {
                ae.Flatten().Handle(e =>
                {
                    if (e is BadRequestException)
                    {
                        throw new BadRequestException(e.Message);
                    }
                    else if (e is NotFoundException)
                    {
                        throw new NotFoundException(e.Message);
                    }
                    else
                    {
                        throw e;
                    }
                });
                return false;
            }
        }

        public async Task<bool> PublistMessageConvertFile(List<IFormFile> files)
        {
            bool check = false;
            var fileMessageModel = await _azureStorageService.UploadFileToContainer(files);
            if (fileMessageModel != null)
            {
                 check = _rabbitmqService.PublidMessage(fileMessageModel);
            }
            return check;
        }

        //reading place item from excel
        public async  Task<List<ExcelPlaceItemDto>> ReadPlaceItemFromExcel(Worksheet ws, List<IFormFile> images)
        {
            List<ExcelPlaceItemDto> PlaceItems = new();
            string sheetName = ws.Name;

            if (!sheetName.Equals("PlaceItem"))
            {
                throw new BadRequestException("PlaceItem sheet not found!");
            }

            try
            {
                int rows = ws.Cells.MaxDataRow;
                for (int i = 1; i <= rows; i++)
                {
                    ExcelPlaceItemDto placeItem = new();

                    // get place item name
                    placeItem.Name = ws.Cells[i, 0].StringValue;

                    string placeName = ws.Cells[i, 1].StringValue;

                    // get place item beancon id
                    placeItem.BeaconId = ws.Cells[i, 2].StringValue;

                    // get BeaconMajorNumber
                    placeItem.BeaconMajorNumber = ws.Cells[i, 3].IntValue;

                    // get BeaconMinorNumber
                    placeItem.BeaconMinorNumber = ws.Cells[i, 4].IntValue;

                    // get item image url
                    string imageName = ws.Cells[i, 5].StringValue;
                    var imageFile = images.SingleOrDefault(x => x.FileName == imageName);
                    if (imageFile != null)
                    {
                        using var stream = new MemoryStream();
                        await imageFile.CopyToAsync(stream);
                        stream.Position = 0;
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFile.FileName);
                        placeItem.Url = await _firebaseStorageService.UploadImageFirebase(stream, $"place/PlaceItemImg/{placeName}/{ws.Cells[i, 0].StringValue}", fileNameWithoutExtension);
                    }
                    else
                    {
                        throw new Exception("File image not found!");
                    }

                    // get start time
                    placeItem.StartTime = TimeSpan.Parse(ws.Cells[i, 6].StringValue);

                    // get end time
                    placeItem.EndTime = TimeSpan.Parse(ws.Cells[i, 7].StringValue);

                    // get place id
                    placeItem.ExcelPlaceId = ws.Cells[i, 8].IntValue;

                    // get item id
                    placeItem.ExcelItemId = ws.Cells[i, 9].IntValue;

                    PlaceItems.Add(placeItem);
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
            
            return PlaceItems;
        }

        //reading place item description from excel
        public Task<List<ExcelItemDescDto>> ReadItemDescFromExcel(Worksheet ws, List<IFormFile> voiceFiles)
        {
            var uniqueNames = new HashSet<string>();
            List<ExcelItemDescDto> itemDescriptions = new();
            string sheetName = ws.Name;

            if (!sheetName.Equals("ItemDescription"))
            {
                throw new BadRequestException("ItemDescription sheet not found!");
            }

            try
            {
                int rows = ws.Cells.MaxDataRow;
                for (int i = 1; i <= rows; i++)
                {
                    ExcelItemDescDto itemDescription = new();

                    // get item id
                    itemDescription.ExcelItemId = ws.Cells[i, 4].IntValue;

                    // get language code
                    itemDescription.LanguageCode = ws.Cells[i, 3].StringValue;

                    // get language Name language
                    itemDescription.NameItem = ws.Cells[i, 2].StringValue;

                    itemDescriptions.Add(itemDescription);
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
            
            return Task.FromResult(itemDescriptions);
        }

        //reading place from excel
        public Task<List<ExcelPlaceDto>> ReadPlaceFromExcel(Worksheet ws)
        {
            List<ExcelPlaceDto> places = new();

            string sheetName = ws.Name;

            if (!sheetName.Equals("Place"))
            {
                throw new BadRequestException("Place sheet not found!");
            }
            try
            {
                int rows = ws.Cells.MaxDataRow;
                for (int i = 1; i <= rows; i++)
                {
                    ExcelPlaceDto place = new();
                    // get place name
                    place.Name = ws.Cells[i, 0].StringValue;

                    // get place address
                    place.Address = ws.Cells[i, 1].StringValue;

                    // get place longtitude
                    place.Longitude = (decimal)ws.Cells[i, 2].DoubleValue;

                    // get place latitude
                    place.Latitude = (decimal)ws.Cells[i, 3].DoubleValue;

                    // get place google place Id
                    place.GooglePlaceId = ws.Cells[i, 4].StringValue;

                    // get place ticket
                    place.EntryTicket = (decimal?)ws.Cells[i, 5].DoubleValue;

                    // get place hour
                    place.Hour = TimeSpan.Parse(ws.Cells[i, 6].StringValue);

                    // get place price
                    place.Price = (decimal?)ws.Cells[i, 7].DoubleValue;

                    // get place id
                    place.ExcelPlaceId = ws.Cells[i, 8].IntValue;

                    places.Add(place);
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
            

            return Task.FromResult(places);
        }

        //reading place category from excel
        public Task<List<ExcelPlaceCategoryDto>> ReadPlaceCategoryFromExcel(Worksheet ws)
        {
            List<ExcelPlaceCategoryDto> PlaceCategories = new();
            string sheetName = ws.Name;

            if (!sheetName.Equals("PlaceCategories"))
            {
                throw new BadRequestException("PlaceCategory sheet not found!");
            }

            try
            {
                int rows = ws.Cells.MaxDataRow;

                for (int i = 1; i <= rows; i++)
                {
                    ExcelPlaceCategoryDto placeCategory = new();

                    // get place id
                    placeCategory.PlaceExcelId = ws.Cells[i, 2].IntValue;

                    // get category id
                    placeCategory.CategoryId = ws.Cells[i, 3].IntValue;

                    PlaceCategories.Add(placeCategory);
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
            
            return Task.FromResult(PlaceCategories);
        }

        //reading place time from excel
        public Task<List<ExcelPlaceTimeDto>> ReadPlaceTimeFromExcel(Worksheet ws)
        {
            List<ExcelPlaceTimeDto> PlaceTimes = new();

            string sheetName = ws.Name;

            if (!sheetName.Equals("PlaceTime"))
            {
                throw new BadRequestException("PlaceTime sheet not found!");
            }

            try
            {
                int rows = ws.Cells.MaxDataRow;

                for (int i = 1; i <= rows; i++)
                {
                    ExcelPlaceTimeDto placeTime = new();

                    // get place id
                    placeTime.ExcelPlaceId = ws.Cells[i, 4].IntValue;

                    // get day id
                    placeTime.DaysOfWeek = ws.Cells[i, 5].IntValue;

                    // get open time
                    placeTime.OpenTime = TimeSpan.Parse(ws.Cells[i, 2].StringValue);

                    // get end time
                    placeTime.EndTime = TimeSpan.Parse(ws.Cells[i, 3].StringValue);

                    PlaceTimes.Add(placeTime);
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

            
            return Task.FromResult(PlaceTimes);
        }

        //reading place image from excel
        public async Task<List<ExcelPlaceImageDto>> ReadPlaceImageFromExcel(Worksheet ws, List<IFormFile> images)
        {
            List<ExcelPlaceImageDto> placeImages = new();
            string sheetName = ws.Name;

            if (!sheetName.Equals("PlaceImage"))
            {
                throw new BadRequestException("PlaceImage sheet not found!");
            }

            int rows = ws.Cells.MaxDataRow;

            try
            {
                for (int i = 1; i <= rows; i++)
                {
                    ExcelPlaceImageDto placeImage = new();

                    // get place id
                    placeImage.ExcelPlaceId = ws.Cells[i, 2].IntValue;

                    // get place image url
                    string imageName = ws.Cells[i, 1].StringValue;
                    var imageFile = images.SingleOrDefault(x => x.FileName == imageName);
                    if (imageFile != null)
                    {
                        using var stream = new MemoryStream();
                        await imageFile.CopyToAsync(stream);
                        stream.Position = 0;
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFile.FileName);
                        placeImage.Image = await _firebaseStorageService.UploadImageFirebase(stream, $"place/PlaceImg/{ws.Cells[i, 0].StringValue}", fileNameWithoutExtension);
                    }
                    else
                    {
                        throw new NotFoundException($"File image {imageName} not found!");
                    }

                    //add to list
                    placeImages.Add(placeImage);
                }
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                else
                {
                    throw new BadRequestException(ex.Message);
                }
            }

            return placeImages;
        }

        //reading place description from excel
        public async Task<ExcelPlaceDescOption> ReadPlaceDescFromExcel(Worksheet ws, List<IFormFile> voiceFiles)
        {
            var uniqueNames = new HashSet<string>();
            ExcelPlaceDescOption excelPlaceDescOption = new();
            List<IFormFile> voiceFileAdd = new();
            List<ExcelPlaceDescDto> excelPlaceDescDtoAdd = new();
            List<string> containerCreateName = new();

            string sheetName = ws.Name;

            if (!sheetName.Equals("PlaceDescription"))
            {
                throw new BadRequestException("PlaceDescription sheet not found!");
            }

            try
            {
                int rows = ws.Cells.MaxDataRow;

                for (int i = 1; i <= rows; i++)
                {
                    ExcelPlaceDescDto placeDescription = new();

                    // get place id
                    placeDescription.ExcelPlaceId = ws.Cells[i, 5].IntValue;

                    // get language code
                    placeDescription.LanguageCode = ws.Cells[i, 6].StringValue;

                    // get language voice file
                    if (ws.Cells[i, 2].Value != null)
                    {
                        var fileName = ws.Cells[i, 2].StringValue;
                        var voiceFile = voiceFiles.SingleOrDefault(x => x.FileName == fileName);
                        if (voiceFile != null)
                        {

                            var fileNameWithoutExtention = Path.GetFileNameWithoutExtension(voiceFile.FileName);
                            if (!uniqueNames.Add(fileName))
                            {
                                throw new BadRequestException($"file name: {fileNameWithoutExtention} is duplicate!");
                            }
                            containerCreateName.Add(fileNameWithoutExtention);
                            placeDescription.VoiceFile = fileName;
                            voiceFileAdd.Add(voiceFile);
                        }
                        else
                        {
                            throw new NotFoundException($"file name: {fileName} is not exist!");
                        }
                    }
                    else
                    {
                        throw new BadRequestException($"voice file in cell is blank!");
                    }

                    // get language Name language
                    placeDescription.Name = ws.Cells[i, 3].StringValue;

                    // get language Name Desc
                    placeDescription.Description = ws.Cells[i, 4].StringValue;

                    excelPlaceDescDtoAdd.Add(placeDescription);
                }
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                {
                    throw new NotFoundException(ex.Message);
                }
                else
                {
                    throw new BadRequestException(ex.Message);
                }
            }
            excelPlaceDescOption.excelPlaceDescs = excelPlaceDescDtoAdd;
            excelPlaceDescOption.VoiceFiles = voiceFileAdd;
            excelPlaceDescOption.containerName = containerCreateName;

            return excelPlaceDescOption;
        }

        public async Task<PlaceListResponse<List<TopPlaceDto>>> GetPlaceNearVisitor(GeoPoint currentLoc, string languageCode)
        {
            var entities = await _placeRepository.GetPlaceNearVisitor(currentLoc, languageCode);
            return new PlaceListResponse<List<TopPlaceDto>>(_mapper.Map<List<TopPlaceDto>>(entities));
        }
        public async Task<PlaceListResponse<List<SearchPlaceDto>>> SearchPlaces(int[] category, string languageCode)
        {
            var entities = await _placeRepository.SearchPlaces(category, languageCode);
            return new PlaceListResponse<List<SearchPlaceDto>>(_mapper.Map<List<SearchPlaceDto>>(entities));
        }

        public async Task<PlaceResponse<PlaceVoiceDto>> GetVoiceScreenData(int placeId, string languageCode)
        {

            var placeResult = await _placeRepository.GetVoiceScreenDataByLanguage(placeId, languageCode);

            if (placeResult == null)
            {
                throw new NotFoundException();
            }

            var resultDto = _mapper.Map<PlaceVoiceDto>(placeResult);
            return new PlaceResponse<PlaceVoiceDto>(resultDto);
        }

        public async Task<PlaceListResponse<PagedResult<TopPlaceDto>>> GetPlaces(QueryParameters parameters, string languageCode)
        {
            var entities = await _placeRepository.GetAsyncWithConditions<TopPlaceDto>(parameters, includeDeleted: false, queryConditions: query =>
            {
                return query
                        .Include(q => q.PlaceDescriptions.Where(x => x.LanguageCode == languageCode))
                        .Include(q => q.PlaceImages.Where(x => x.IsPrimary))
                        .Include(q => q.FeedBacks)
                        .Where(q => q.PlaceDescriptions.Any(x => x.LanguageCode == languageCode) && q.Status == (int)PlaceStatus.active)
                        .OrderByDescending(x=>x.Id);
            });
            var result = _mapper.Map<PagedResult<TopPlaceDto>>(entities);
            return new PlaceListResponse<PagedResult<TopPlaceDto>>(result);
        }

        public async Task<bool> ConvertMp3ToM3u8V2(List<IFormFile> mp3Files)
        {
            var check = await PublistMessageConvertFile(mp3Files);
            return check;
        }

        public async Task<PlaceItemsResponse<List<PlaceItemDto>>> GetBeaconsByPlaceId(int placeId, string languageCode)
        {
            var placeResult = await _placeItemRepository.GetAsyncWithConditions<PlaceItemDto>(queryConditions: query => query.Include(x=>x.ItemDescriptions.Where(q=>q.LanguageCode == languageCode)).Where(x => x.PlaceId == placeId));

            if (placeResult == null)
            {
                throw new NotFoundException();
            }
            return new PlaceItemsResponse<List<PlaceItemDto>>(placeResult);
        }

        public async Task<PlaceItemResponse<PlaceItemViewDto>> GetBeaconData(int placeItemId, string languageCode)
        {
            var placeItemResult = await _placeItemRepository.GetItemDescription(placeItemId, languageCode);

            if (placeItemResult == null)
            {
                throw new NotFoundException();
            }
            var result = _mapper.Map<PlaceItemViewDto>(placeItemResult);
            return new PlaceItemResponse<PlaceItemViewDto>(result);
        }
        
        public async Task<PlaceListResponse<List<TopPlaceDto>>> GetPlacesNearPlace(int placeId,string languageCode)
        {
            var placeResult = await _placeRepository.FindByIdAsync(placeId);

            if (placeResult == null)
            {
                throw new NotFoundException();
            }

            GeoPoint currentLoc = new GeoPoint
            {
                Latitude = Decimal.ToDouble(placeResult.Latitude),
                Longitude = Decimal.ToDouble(placeResult.Longitude)
            };
            var entities = await _placeRepository.GetPlacesNearPlace(placeId, currentLoc, languageCode);
            return new PlaceListResponse<List<TopPlaceDto>>(_mapper.Map<List<TopPlaceDto>>(entities));
        }

        public async Task<PlaceItemResponse<PlaceItemViewDto>> UpdatePlaceItemAsync(PlaceUpdateItemDto placeUpdateItemDto, int placeitemId)
        {
            // model validation
            var validator = new PlaceUpdateItemDtoValidator();
            var validationResult = await validator.ValidateAsync(placeUpdateItemDto);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid data", validationResult);
            }

            var placeItemUpdate = _mapper.Map<PlaceItem>(placeUpdateItemDto);

            var placeItem = await _placeItemRepository.GetPlaceitemById(placeitemId);
            if (placeItem == null)
            {
                throw new NotFoundException("Place item id not found!");
            }
            else
            {
                placeItemUpdate.Id = placeitemId;
                placeItemUpdate.PlaceId = placeItem.PlaceId;
                _placeItemRepository.DetachedPlaceInstance(placeItem);
            }

            foreach (var itemDesc in placeUpdateItemDto.ItemDescriptions)
            {
                var check2 = await _languageRepository.LanguageCodeIsExist(itemDesc.LanguageCode);
                if (!check2)
                {
                    throw new BadRequestException($"Language code in item: {itemDesc.LanguageCode} is not support in system!");
                }
            }

            // check language code is duplicate for Item description
            bool hasDuplicateLanguageCodeItem = placeUpdateItemDto.ItemDescriptions
                .GroupBy(pd => pd.LanguageCode)
                .Any(g => g.Count() > 1);

            if (hasDuplicateLanguageCodeItem)
            {
                throw new BadRequestException("Duplicate Language code in the place Item description!");
            }


            var result = await _placeItemRepository.UpdatePlaceItemAsync(placeItemUpdate);
            return new PlaceItemResponse<PlaceItemViewDto>(_mapper.Map<PlaceItemViewDto>(result));
        }

        public async Task<PlaceItemResponse<PlaceItemViewDto>> GetPlaceItemAsync(int placeItemId)
        {
            var placeItem = await _placeItemRepository.GetPlaceitemById(placeItemId);
            if(placeItem == null)
            {
                throw new NotFoundException("Place item id not found!");
            }


            return new PlaceItemResponse<PlaceItemViewDto>(_mapper.Map<PlaceItemViewDto>(placeItem));
        }
    }

}
