using System.ComponentModel.DataAnnotations;

namespace AppNLayer.Entities;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    // Collection Navigation Property
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}