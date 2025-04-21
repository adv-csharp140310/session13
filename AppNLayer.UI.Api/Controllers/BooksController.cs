using AppNLayer.BLL.Services;
using AppNLayer.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AppNLayer.UI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController(IBookService bookService, ILogger<BooksController> logger) : ControllerBase
{
    private readonly IBookService _bookService = bookService;
    private readonly ILogger<BooksController> _logger = logger;

    [HttpGet]
    [ProducesResponseType<IEnumerable<BookDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks(CancellationToken cancellationToken)
    {
        var books = await _bookService.GetAllAsync(cancellationToken);
        return Ok(books);
    }

    [HttpGet("{id:int}", Name = "GetBookById")]
    [ProducesResponseType<BookDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> GetBookById(int id, CancellationToken cancellationToken)
    {
        var book = await _bookService.GetByIdAsync(id, cancellationToken);
        if (book == null)
        {
            _logger.LogWarning("Book with ID {BookId} not found.", id);
            return NotFound();
        }
        return Ok(book);
    }

    [HttpGet("ByCategory/{categoryId:int}")]
    [ProducesResponseType<IEnumerable<BookDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByCategory(int categoryId, CancellationToken cancellationToken)
    {
        var books = await _bookService.GetByCategoryIdAsync(categoryId, cancellationToken);
        return Ok(books);
    }


    [HttpPost]
    [ProducesResponseType<BookDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookDto createDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var createdBook = await _bookService.CreateAsync(createDto, cancellationToken);
            return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook);            
        }
        catch (KeyNotFoundException ex) // Catch specific exception for missing category
        {
            _logger.LogWarning(ex, "Attempted to create book with non-existent Category ID {CategoryId}.", createDto.CategoryId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating book with title {BookTitle}", createDto.Title);
            return BadRequest(new { message = $"Error creating book: {ex.Message}" });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto updateDto, CancellationToken cancellationToken)
    {
        if (id != updateDto.Id)
        {
            return BadRequest("ID mismatch between route parameter and request body.");
        }
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var success = await _bookService.UpdateAsync(id, updateDto, cancellationToken);
            if (!success)
            {
                _logger.LogWarning("Attempted to update non-existent book with ID {BookId}", id);
                return NotFound(new { message = $"Book with ID {id} not found." });
            }
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Attempted to update book {BookId} with non-existent Category ID {CategoryId}.", id, updateDto.CategoryId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating book with ID {BookId}", id);
            return BadRequest(new { message = $"Error updating book: {ex.Message}" });
        }
    }


    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBook(int id, CancellationToken cancellationToken)
    {
        var success = await _bookService.DeleteAsync(id, cancellationToken);
        if (!success)
        {
            _logger.LogWarning("Attempted to delete non-existent book with ID {BookId}", id);
            return NotFound();
        }
        return NoContent();
    }
}
