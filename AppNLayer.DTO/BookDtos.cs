using System.ComponentModel.DataAnnotations;

namespace AppNLayer.DTO;

public record BookDto(
    int Id,
    string Title,
    string? Author,
    string? Isbn,
    int? PublicationYear,
    int CategoryId,
    string CategoryName
);

public record CreateBookDto(
    [Required]
    [StringLength(200)]
    string Title,

    [StringLength(100)]
    string? Author,

    [StringLength(20)]
    string? Isbn,

    int? PublicationYear,

    [Required]
    int CategoryId
);

public record UpdateBookDto(
    [Required]
    int Id,

    [Required]
    [StringLength(200)]
    string Title,

    [StringLength(100)]
    string? Author,

    [StringLength(20)]
    string? Isbn,

    int? PublicationYear,

    [Required]
    int CategoryId
);

