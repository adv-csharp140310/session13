using AppNLayer.DTO;
using AppNLayer.Entities;
using Riok.Mapperly.Abstractions;

namespace AppNLayer.BLL.Mappers;

[Mapper] 
public static partial class CategoryMapper 
{
    public static partial CategoryDto MapToDto(this Category category);
    public static partial IEnumerable<CategoryDto> MapToDto(this IEnumerable<Category> categories);
    public static partial Category MapFromCreateDto(this CreateCategoryDto dto);

    // For updating, Mapperly can generate a method that maps onto an existing object
    [MapperIgnoreTarget(nameof(Category.Id))] // Don't map Id during update
    [MapperIgnoreTarget(nameof(Category.Books))] // Don't map navigation properties by default
    //public static partial void MapFromUpdateDto(this UpdateCategoryDto dto, Category destination);
    public static partial Category MapFromUpdateDto(this UpdateCategoryDto dto);
}
