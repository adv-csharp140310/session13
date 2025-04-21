using AppNLayer.DTO;

namespace AppNLayer.BLL.Services;
public interface ICategoryService
{
    Task<CategoryDto> CreateAsync(CreateCategoryDto createDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateCategoryDto updateDto, CancellationToken cancellationToken = default);
}