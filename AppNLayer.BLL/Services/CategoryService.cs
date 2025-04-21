using AppNLayer.BLL.Mappers;
using AppNLayer.DAL.Repositories;
using AppNLayer.DTO;
using AppNLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppNLayer.BLL.Services;


public class CategoryService : ICategoryService
{
    private readonly IDataAccess _dataAccess;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(IDataAccess dataAccess, ILogger<CategoryService> logger)
    {
        _dataAccess = dataAccess;
        _logger = logger;
    }

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting category with ID {CategoryId}", id);
        var category = await _dataAccess.GetByIdAsync<Category>(id, cancellationToken);
        return category?.MapToDto();
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all categories");
        var categories = await _dataAccess.GetAllAsync<Category>(cancellationToken);
        return categories.MapToDto();
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto createDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createDto);
        _logger.LogInformation("Creating new {EntityType}", typeof(Category).Name);

        var category = createDto.MapFromCreateDto();

        await _dataAccess.AddAsync(category, cancellationToken);
        await _dataAccess.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created {EntityType} with Id: {Id}", typeof(Category).Name, category.Id);
        return category.MapToDto();
    }

    public async Task<bool> UpdateAsync(int id, UpdateCategoryDto updateDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(updateDto);
        _logger.LogInformation("Updating {EntityType} with Id: {Id}", typeof(Category).Name, id);

        if (id != updateDto.Id)
        {
            _logger.LogWarning("ID mismatch between route parameter and request body.");
            return false;
        }


        var existingCategory = await _dataAccess.GetByIdAsync<Category>(id, cancellationToken);
        if (existingCategory == null)
        {
            _logger.LogWarning("Update failed: {EntityType} with Id: {Id} not found.", typeof(Category).Name, id);
            return false;
        }

        existingCategory = updateDto.MapFromUpdateDto;

        _dataAccess.Update(existingCategory);
        await _dataAccess.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _dataAccess.GetByIdAsync<Category>(id, cancellationToken);
        if (category == null)
        {
            return false;
        }

        var hasBooks = _dataAccess.Find<Category>(c => c.Id == id && c.Books.Any()).Any();
        if (hasBooks)
        {
            _logger.LogWarning("Attempted to delete Category {CategoryId} which has associated Books.", category.Id);
            throw new InvalidOperationException($"Cannot delete category '{category.Name}' (ID: {category.Id}) because it has associated books.");
        }

        _dataAccess.Remove(category);
        try
        {
            await _dataAccess.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning("Error deleting category {id}: {messagge}", category.Id, ex.Message);
            return false;
        }
    }
}