using AppNLayer.DTO;
using AppNLayer.Entities;
using Riok.Mapperly.Abstractions;

namespace AppNLayer.BLL.Mappers;


[Mapper]
public static partial class BookMapper 
{
    [MapProperty(nameof(Book.Category.Name), nameof(BookDto.CategoryName))]
    public static partial BookDto MapToDto(this Book book);

    public static partial IEnumerable<BookDto> MapToDto(this IEnumerable<Book> books);

    [MapperIgnoreTarget(nameof(Book.Category))] // Don't map navigation property from DTO
    public static partial Book MapFromCreateDto(this CreateBookDto dto);

    [MapperIgnoreTarget(nameof(Book.Id))]
    [MapperIgnoreTarget(nameof(Book.Category))]
    //public static partial void MapFromUpdateDto(this UpdateBookDto dto, Book destination);
    public static partial Book MapFromUpdateDto(this UpdateBookDto dto);
}
