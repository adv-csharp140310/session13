using AppNLayer.DTO;

namespace AppNLayer.BLL.Services;
public interface IBookService
{
    Task<BookDto?> CreateAsync(CreateBookDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<BookDto>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<BookDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateBookDto dto, CancellationToken cancellationToken = default);
}