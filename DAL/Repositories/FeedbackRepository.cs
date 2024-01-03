using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    public class FeedbackRepository : BaseRepository<FeedBack>, IFeedbackRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCacheService _redisCacheService;

        public FeedbackRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService redisCacheService) : base(context, mapper, unitOfWork, redisCacheService)
        {
            _context = context;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _redisCacheService = redisCacheService;
        }
        public async Task<PagedResult<FeedbackListDto>> GetFeedbackAsync<FeedbackListDto>(QueryParameters queryParameters, int id, bool isplace)
        {
            var query = _context.Set<FeedBack>().AsQueryable();
           
            var totalSize = await query.CountAsync();

            query = _context.Set<FeedBack>();
            if (isplace)
            {
                query = query.Where(x => x.PlaceId == id);
            }
            else
            {
                query = query.Where(x => x.ItineraryId == id);

            }
            var items = await query.Skip((queryParameters.PageNumber) * queryParameters.PageSize).Take(queryParameters.PageSize)
                .ProjectTo<FeedbackListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var pageCount = (double)totalSize / queryParameters.PageSize;

            return new PagedResult<FeedbackListDto>
            {
                Data = items,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                PageCount = (int)Math.Ceiling(pageCount),
                TotalCount = totalSize,
            };
        }

        public async Task<List<double>> GetFeedbackAvg(int tagetId, bool isPlace)
        {
            if (isPlace)
            {
                var placeRate = await _context.FeedBacks.Where(x => x.PlaceId == tagetId && isPlace == true).Select(x => x.Rate ?? 0).ToListAsync();
                return placeRate;
            }
            var tourRate = await _context.FeedBacks.Where(x => x.ItineraryId == tagetId && isPlace == false).Select(x=>x.Rate ?? 0).ToListAsync();
            return tourRate;
        }

        public async Task<bool> IsExistFeedback(int feedbackId)
        {
            try
            {
                var feedback = await _context.FeedBacks.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == feedbackId);
                if(feedback == null)
                {
                    return false;
                }
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }  

        public async Task<bool> ChangeStatusFeedback(int feedbackId)
        {
            var feedback = await _context.FeedBacks.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == feedbackId);
            if(feedback != null)
            {
                if(feedback.Status == 1)
                {
                    feedback.Status = 0;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    feedback.Status = 1;
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<FeedBack> GetFeedbackDetailById(int id)
        {
            var feedback = await _context.FeedBacks
                .Include(x => x.Account)
                .ThenInclude(x => x.NationalCodeNavigation)
                .Include(x => x.Place)
                .ThenInclude(x => x.PlaceImages)
                .Include(x => x.Itinerary)
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(x => x.Id == id);

            return feedback;
        }
    }
}
