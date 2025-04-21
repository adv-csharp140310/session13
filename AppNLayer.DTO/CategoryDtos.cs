using System.ComponentModel.DataAnnotations;

namespace AppNLayer.DTO;

public record CategoryDto(
    int Id,
    string Name
);

public record CreateCategoryDto(
    [Required]
    [StringLength(100)]
    string Name
);

public record UpdateCategoryDto(
    [Required]
    int Id,

    [Required]
    [StringLength(100)]
    string Name
);
