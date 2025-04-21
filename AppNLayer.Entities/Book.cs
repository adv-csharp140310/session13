using System.ComponentModel.DataAnnotations;

namespace AppNLayer.Entities;
public class Book 
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public required string Title { get; set; }

    [MaxLength(100)]
    public string? Author { get; set; }

    [MaxLength(20)]
    public string? Isbn { get; set; }

    public int? PublicationYear { get; set; }

    public int CategoryId { get; set; }

    // Navigation Property
    public virtual Category Category { get; set; } = null!;
}
