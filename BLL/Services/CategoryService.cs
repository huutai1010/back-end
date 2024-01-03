using AutoMapper;
using BLL.DTOs.Category;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Responses;
using Common.Constants;
using Common.Interfaces;
using Common.Models;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BLL.DTOs.Category.CategoryCreateDto;
using static BLL.DTOs.Category.CategoryUpdateDto;

namespace BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IRedisCacheService redisCacheService)
        {
            _categoryRepository = categoryRepository;
            _redisCacheService = redisCacheService;
            _mapper = mapper;
        }
        public async Task<CategoryLanguageListResponse<List<CategoryViewDto>>> GetCategories(string languageCode)
        {
            List<CategoryViewDto>? result;
            result = await _redisCacheService.Get<List<CategoryViewDto>>(RedisCacheKeys.CATEGORIES +"-" + languageCode);

            if (result == null || !result.Any())
            {
                var entities = await _categoryRepository.GetCategories(languageCode);
                result = _mapper.Map<List<CategoryViewDto>>(entities);
                await _redisCacheService.SaveCacheAsync(RedisCacheKeys.CATEGORIES +"-"+ languageCode, result);
            }

            return new CategoryLanguageListResponse<List<CategoryViewDto>>(result);
        }

        public async Task<CategoryeListResponse<List<CategoryListCreateDto>>> GetCategoriesWithoutPaging()
        {
            var result = _categoryRepository.GetAllWithoutPagingAsync();
            return new CategoryeListResponse<List<CategoryListCreateDto>>(_mapper.Map<List<CategoryListCreateDto>>(await result) );
        }

        #region admin
        public async Task<CategoryeListResponse<PagedResult<CategoryListDto>>> GetListAsync([FromQuery]QueryParameters queryParameters)
        {
            var result = await _categoryRepository.GetAsyncWithConditions<CategoryListDto>(queryParameters, includeDeleted: true, queryConditions: query =>
            {
                return query
                    .Include(x => x.CategoryLanguages);
            });
            return new CategoryeListResponse<PagedResult<CategoryListDto>>(result);
        }

        public async Task<bool> ChangeStatusAsync(int languageId)
        {
            var result = await _categoryRepository.ChangeStatusAsync(languageId);
            if (!result)
            {
                throw new NotFoundException("category id not found!");
            }
            return result;
        }

        public async Task<CategoryResponse<CategoryDetailDto>> GetDetailAsync(int categoryId)
        {
            var result = await _categoryRepository.GetDetailAsync(categoryId);
            if (result is null)
            {
                throw new NotFoundException($"category with id: {categoryId} not found!");
            }

            return new CategoryResponse<CategoryDetailDto>(_mapper.Map<CategoryDetailDto>(result));
        }

        public async Task<bool> CreateCategoryAsync(CategoryCreateDto categoryCreate)
        {
            // model validation
            var validator = new CategoryCreateDtoValidator();
            var validationResult = await validator.ValidateAsync(categoryCreate);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid data", validationResult);
            }

            var checkName = await _categoryRepository.CategoryNameIsExist(categoryCreate.Name);
            if (checkName)
            {
                throw new BadRequestException("Category name is exist!");
            }

            var check = await _categoryRepository.CreateAsync(_mapper.Map<Category>(categoryCreate));
            if (!check)
            {
                throw new BadRequestException("Create Category false!");
            }
            return check;
        }

        public async Task<CategoryResponse<CategoryDetailDto>> UpdateCategoryAsync(CategoryUpdateDto categoryUpdate, int categoryId)
        {
            // model validation
            var validator = new CategoryUpdateDtoValidator();
            var validationResult = await validator.ValidateAsync(categoryUpdate);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid data", validationResult);
            }
            var cateUpdate = _mapper.Map<Category>(categoryUpdate);

            var categoryExist = await _categoryRepository.GetDetailAsync(categoryId);
            if (categoryExist is null)
            {
                throw new NotFoundException("Category not found!");
            }
            else
            {
                cateUpdate.Status = categoryExist.Status;
                cateUpdate.CreateTime = categoryExist.CreateTime;
                _categoryRepository.DetachedPlaceInstance(categoryExist);
            }

            var checkName = await _categoryRepository.CategoryNameIsExist(categoryUpdate.Name, categoryId);
            if (checkName)
            {
                throw new BadRequestException("Category name is exist!");
            }

            cateUpdate.Id= categoryId;

            foreach(var cateLanguage in cateUpdate.CategoryLanguages)
            {
                cateLanguage.CategoryId = categoryId;
            }

            var category = await _categoryRepository.UpdateCategoryAsync(cateUpdate);
            return new CategoryResponse<CategoryDetailDto>(_mapper.Map<CategoryDetailDto>(category));
        }
        #endregion
    }
}
