using BLL.DTOs.Category;
using BLL.Responses;
using Common.Models;

namespace BLL.Interfaces
{
    public interface ICategoryService
    {
        public Task<CategoryLanguageListResponse<List<CategoryViewDto>>> GetCategories(string languageCode);
        public Task<CategoryeListResponse<PagedResult<CategoryListDto>>> GetListAsync(QueryParameters queryParameters);
        public Task<bool> ChangeStatusAsync(int languageId);
        public Task<CategoryResponse<CategoryDetailDto>> GetDetailAsync(int categoryId);
        public Task<bool> CreateCategoryAsync(CategoryCreateDto categoryCreate);
        public Task<CategoryResponse<CategoryDetailDto>> UpdateCategoryAsync(CategoryUpdateDto categoryUpdate, int categoryId);
        public Task<CategoryeListResponse<List<CategoryListCreateDto>>> GetCategoriesWithoutPaging();
    }
}
