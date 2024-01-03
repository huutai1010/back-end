using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Interfaces;
using Common.Models;
using DAL.DatabaseContext;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class MarkPlaceRepository : BaseRepository<MarkPlace>, IMarkPlaceRepository
    {

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCacheService _redisCacheService;
        public MarkPlaceRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService redisCacheService) : base(context, mapper, unitOfWork, redisCacheService)
        {
            _context = context;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _redisCacheService = redisCacheService;
        }

        public async Task<bool> IsPlaceFavorite(int accountId, int placeId)
        {
            return await _context.MarkPlaces.AnyAsync(x => x.PlaceId == placeId && x.AccountId == accountId && x.Status == 1);
        }
    }
}
