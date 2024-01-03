using AutoMapper;
using BLL.DTOs.Nationality;
using BLL.Interfaces;
using BLL.Responses;
using Common.Constants;
using Common.Interfaces;
using DAL.Interfaces;

namespace BLL.Services
{
    public class NationalityService : INationalityService
    {
        private readonly INationalityRepository _nationalityRepository;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IMapper _mapper;

        public NationalityService(INationalityRepository nationalityRepository, IMapper mapper,IRedisCacheService redisCacheService)
        {
            _nationalityRepository = nationalityRepository;
            _redisCacheService = redisCacheService;
            _mapper = mapper;
        }

        public async Task<NationalitiesResponse<List<NationalityListDto>>> GetNationalitiesForAdmin()
        {

            var result = await _nationalityRepository.GetAsync<NationalityListDto>(includeDeleted: false, caching: false);

            return new NationalitiesResponse<List<NationalityListDto>>(result);
        }

        public async Task<NationalitiesResponse<List<NationalityListDto>>> GetNationalities()
        {
            var result = await _nationalityRepository.GetAsync<NationalityListDto>(includeDeleted: true, caching: true);

            return new NationalitiesResponse<List<NationalityListDto>>(result);
        }
    }
}
