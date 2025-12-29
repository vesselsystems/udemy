using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookstoreApp.Api.Data;
using BookstoreApp.Api.Models.Authors;
using BookstoreApp.Api.Static;

namespace BookstoreApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthorsController : ControllerBase
{
    private readonly BookstoreDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(BookstoreDbContext context, IMapper mapper, ILogger<AuthorsController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorReadOnlyDto>>> GetAuthors()
    {
        try
        {
            if (_context.Authors == null)
            {
                _logger.LogWarning("Authors set is null in {Action}.", nameof(GetAuthors));
                return NotFound();
            }

            var authors = await _context.Authors.ToListAsync();
            var authorDtos = _mapper.Map<IEnumerable<AuthorReadOnlyDto>>(authors);

            return Ok(authorDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing {Action}.", nameof(GetAuthors));
            return StatusCode(500, Messages.Error500Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorReadOnlyDto>> GetAuthor(int id)
    {
        try
        {
            if (_context.Authors == null)
            {
                _logger.LogWarning("Authors set is null in {Action}.", nameof(GetAuthor));
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                _logger.LogWarning("Author not found in {Action} with id {AuthorId}.", nameof(GetAuthor), id);
                return NotFound();
            }

            var authorDto = _mapper.Map<AuthorReadOnlyDto>(author);

            return Ok(authorDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing {Action}.", nameof(GetAuthor));
            return StatusCode(500, Messages.Error500Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAuthor(int id, AuthorUpdateDto authorDto)
    {
        try
        {
            if (id != authorDto.Id)
            {
                _logger.LogWarning("Invalid record id in {Action}. Requested {RequestId}, payload {PayloadId}.", nameof(PutAuthor), id, authorDto.Id);
                return BadRequest();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                _logger.LogWarning("Author not found in {Action} with id {AuthorId}.", nameof(PutAuthor), id);
                return NotFound();
            }

            _mapper.Map(authorDto, author);

            _context.Entry(author).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AuthorExists(id))
            {
                _logger.LogWarning("Author not found in {Action} with id {AuthorId}.", nameof(PutAuthor), id);
                return NotFound();
            }

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing {Action}.", nameof(PutAuthor));
            return StatusCode(500, Messages.Error500Message);
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<AuthorReadOnlyDto>> PostAuthor(AuthorCreateDto authorDto)
    {
        try
        {
            if (_context.Authors == null)
            {
                _logger.LogWarning("Authors set is null in {Action}.", nameof(PostAuthor));
                return Problem("Entity set 'BookstoreDbContext.Authors' is null.");
            }

            var author = _mapper.Map<Author>(authorDto);
            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();

            var createdAuthor = _mapper.Map<AuthorReadOnlyDto>(author);
            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, createdAuthor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing {Action}.", nameof(PostAuthor));
            return StatusCode(500, Messages.Error500Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        try
        {
            if (_context.Authors == null)
            {
                _logger.LogWarning("Authors set is null in {Action}.", nameof(DeleteAuthor));
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                _logger.LogWarning("Author not found in {Action} with id {AuthorId}.", nameof(DeleteAuthor), id);
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing {Action}.", nameof(DeleteAuthor));
            return StatusCode(500, Messages.Error500Message);
        }
    }

    private bool AuthorExists(int id)
    {
        return (_context.Authors?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
