using AppNLayer.BLL.Mappers;
using AppNLayer.DAL.Repositories;
using AppNLayer.DTO;
using AppNLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppNLayer.BLL.Services;

public class BookService : IBookService
{
    private readonly IDataAccess _dataAccess;
    private readonly ILogger<BookService> _logger;

    public BookService(IDataAccess dataAccess, ILogger<BookService> logger)
    {
        _dataAccess = dataAccess;
        _logger = logger;
    }

    public async Task<IEnumerable<BookDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all books");
        var books = await _dataAccess.Find<Book>(noTracking: true)
                                    .Include(b => b.Category)
                                    .ToListAsync(cancellationToken);
        return books.MapToDto();
    }

    public async Task<BookDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting book with ID {BookId}", id);
        var book = await _dataAccess.Find<Book>(b => b.Id == id)
                        .Include(b => b.Category)
                        .FirstOrDefaultAsync(cancellationToken);
        return book?.MapToDto();
    }

    public async Task<IEnumerable<BookDto>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting books for category ID {CategoryId}", categoryId);

        var books = await _dataAccess.Find<Book>(b => b.CategoryId == categoryId, noTracking: true)
                        .Include(b => b.Category) // Include Category for mapping
                        .ToListAsync(cancellationToken);

        return books.MapToDto();
    }


    public async Task<BookDto?> CreateAsync(CreateBookDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new book with Title: {Title}", dto.Title);

        var categoryExists = await _dataAccess.GetByIdAsync<Category>(dto.CategoryId, cancellationToken);
        if (categoryExists == null)
        {
            _logger.LogWarning("Attempted to create book with non-existent Category ID: {CategoryId}", dto.CategoryId);
            return null; // Or throw specific validation exception
        }

        var newBook = dto.MapFromCreateDto();
        await _dataAccess.AddAsync(newBook, cancellationToken);
        await _dataAccess.SaveChangesAsync(cancellationToken);

        var createdBookWithCategory = await _dataAccess.Find<Book>(b => b.Id == newBook.Id)
                        .Include(b => b.Category)
                        .FirstOrDefaultAsync(cancellationToken);

        return createdBookWithCategory?.MapToDto();
    }

    public async Task<bool> UpdateAsync(int id, UpdateBookDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating book with ID {BookId}", id);
        var existingBook = await _dataAccess.GetByIdAsync<Book>(id, cancellationToken); // Find by ID, tracking enabled

        if (existingBook == null)
        {
            _logger.LogWarning("Book with ID {BookId} not found for update", id);
            return false;
        }

        existingBook = dto.MapFromUpdateDto();
        _dataAccess.Update(existingBook);
        try
        {
            await _dataAccess.SaveChangesAsync(cancellationToken);
            return true;

        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency conflict updating Book ID {BookId}", id);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes while updating Book ID {BookId}", id);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting book with ID {BookId}", id);
        var bookToDelete = await _dataAccess.GetByIdAsync<Book>(id, cancellationToken);

        if (bookToDelete == null)
        {
            _logger.LogWarning("Book with ID {BookId} not found for deletion", id);
            return false;
        }

        _dataAccess.Remove(bookToDelete);
        await _dataAccess.SaveChangesAsync(cancellationToken);
        return true;
    }
}