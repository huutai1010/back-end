using AutoMapper;

using Common.Constants;
using Common.Interfaces;
using Common.Models;

using DAL.DatabaseContext;
using DAL.Entities;
using DAL.Interfaces;

using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;
using System.Reflection;

namespace DAL.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisCacheService _cacheService;
        private readonly string _redisKey;
        private readonly string _redisKeyIncludeDeleted;

        public BaseRepository(AppDbContext context, IMapper mapper, IUnitOfWork unitOfWork, IRedisCacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _redisKey = typeof(T).Name.ToLower();
            _redisKeyIncludeDeleted = _redisKey + RedisCacheKeys.PREFIX_ALL;
        }
        public async Task<bool> CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
            bool success = await _unitOfWork.SaveChangesAsync();
            if (success)
            {
                await _cacheService.RemoveAsync(_redisKey);
                await _cacheService.RemoveAsync(_redisKeyIncludeDeleted);
            }
            return success;
        }

        public async Task<bool> CreateManyAsync(params T[] entities)
        {
            await _context.AddRangeAsync(entities);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result)
            {
                await _cacheService.RemoveAsync(_redisKey);
                await _cacheService.RemoveAsync(_redisKeyIncludeDeleted);
            }
            return result;     
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            _context.Remove(entity);
            var success = await _unitOfWork.SaveChangesAsync();
            if (success)
            {
                await _cacheService.RemoveAsync(_redisKey);
                await _cacheService.RemoveAsync(_redisKeyIncludeDeleted);
            }
            return success;    
        }

        public async Task<bool> Exist(params object[] keys)
        {
            return await _context.Set<T>().FindAsync(keys) != null;
        }

        public async Task<T?> FindByIdAsync(params object[] id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            bool success = await _unitOfWork.SaveChangesAsync();
            if (success)
            {
                await _cacheService.RemoveAsync(_redisKey);
                await _cacheService.RemoveAsync(_redisKeyIncludeDeleted);
            }
            return success;
        }

        public async Task<PagedResult<TResult>> GetAsync<TResult>(QueryParameters queryParameters,
            bool caching = false,
            bool includeDeleted = false,
            Expression<Func<T, object>>? orderBy = null,
            bool descending = true)
        {
            List<TResult>? items;
            int totalSize;
            double pageCount;

            items = await GetAsync<TResult>(includeDeleted, caching, orderBy, descending);


            string? searchByPropName = queryParameters.SearchBy;
            string? searchText = queryParameters.Search;
            if (!string.IsNullOrEmpty(searchByPropName) && !string.IsNullOrEmpty(searchText))
            {
                var searchProp = typeof(TResult).GetProperty(searchByPropName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var search = queryParameters.Search?.ToLower();
                if (searchProp != null && !string.IsNullOrEmpty(search))
                {
                    items = items.Where(x => 
                        searchProp.GetValue(x, null)
                        ?.ToString()?.ToLower()
                        .Contains(search) ?? true)
                        .ToList();
                }
            }

            totalSize = items.Count;
            pageCount = (double)totalSize / queryParameters.PageSize;

            return new PagedResult<TResult>
            {
                Data = items
                        .Skip((queryParameters.PageNumber) * queryParameters.PageSize)
                        .Take(queryParameters.PageSize).ToList(),
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                PageCount = (int)Math.Ceiling(pageCount),
                TotalCount = totalSize,
            };
        }

        public async Task<List<TResult>> GetAsync<TResult>(bool includeDeleted = false,
            bool caching = false,
            Expression<Func<T, object>>? orderBy = null,
            bool descending = true)
        {
            string redisKeyToUse = "";
            if (caching)
            {
                redisKeyToUse = _redisKey;
                if (includeDeleted)
                {
                    redisKeyToUse = _redisKeyIncludeDeleted;
                }
                List<T>? cachedData = await _cacheService.Get<List<T>>(redisKeyToUse);
                if (cachedData != null)
                {
                    return _mapper.Map<List<TResult>>(cachedData);
                }
            }
            var query = _context.Set<T>().AsQueryable();
            List<T> queryResult;
            if (includeDeleted)
            {
                query = query.IgnoreQueryFilters();
            }
            if (orderBy != null)
            {
                if (descending)
                {
                    query = query.OrderByDescending(orderBy);
                }
                else
                {
                    query = query.OrderBy(orderBy);
                }
                queryResult = await query.ToListAsync();
            }
            else
            {
                var createTimePropertyInfo = typeof(T).GetProperty("CreateTime", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var idPropertyInfo = typeof(T).GetProperty("Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var queryEnumerable = query
                    .AsEnumerable();
                if (createTimePropertyInfo != null)
                {
                    queryEnumerable = queryEnumerable.OrderByDescending(o => createTimePropertyInfo.GetValue(o, null));
                }
                if (idPropertyInfo != null)
                {
                    queryEnumerable = queryEnumerable.OrderByDescending(o => idPropertyInfo.GetValue(o, null));
                }
                queryResult = queryEnumerable.ToList();
            }

            if (caching)
            {
                await _cacheService.SaveCacheAsync(redisKeyToUse, queryResult);
            }
            return _mapper.Map<List<TResult>>(queryResult);
        }

        public async Task<PagedResult<TResult>> GetAsyncWithConditions<TResult>(QueryParameters queryParameters, bool includeDeleted = false, Func<IQueryable<T>, IQueryable<T>>? queryConditions = null)
        {
            List<TResult>? items;
            int totalSize;
            double pageCount;
            
            items = await GetAsyncWithConditions<TResult>(includeDeleted, queryConditions);

            string? searchByPropName = queryParameters.SearchBy;
            string? searchText = queryParameters.Search;
            if (!string.IsNullOrEmpty(searchByPropName) && !string.IsNullOrEmpty(searchText))
            {
                var searchProp = typeof(TResult).GetProperty(searchByPropName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var search = queryParameters.Search?.ToLower();
                if (searchProp != null && !string.IsNullOrEmpty(search))
                {
                    items = items.Where(x =>
                        searchProp.GetValue(x, null)
                        ?.ToString()?.ToLower()
                        .Contains(search) ?? true)
                        .ToList();
                }
            }
            totalSize = items.Count;
            pageCount = (double)totalSize / queryParameters.PageSize;
            return new PagedResult<TResult>
            {
                Data = items
                        .Skip((queryParameters.PageNumber) * queryParameters.PageSize)
                        .Take(queryParameters.PageSize).ToList(),
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                PageCount = (int)Math.Ceiling(pageCount),
                TotalCount = totalSize,
            };
        }

        public async Task<List<TResult>> GetAsyncWithConditions<TResult>(bool includeDeleted = false, Func<IQueryable<T>, IQueryable<T>>? queryConditions = null)
        {
            List<TResult>? result;
            var query = _context.Set<T>().AsQueryable();
            if (includeDeleted)
            {
                query = query.IgnoreQueryFilters();
            }

            if (queryConditions != null)
            {
                query = queryConditions(query);
            }

            result = _mapper.Map<List<TResult>>(query);
            //  result = await query.ProjectTo<TResult>(_mapper.ConfigurationProvider).ToListAsync(); this code is can not map include entity
            return result;
        }

        public void DetachedPlaceInstance(T entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }
    }
}
