using BookstoreManager.Communication.Request;
using BookstoreManager.Communication.Response;
using BookstoreManager.Communication.Update;
using BookstoreManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookstoreManagerController : ControllerBase
{
    private static readonly List<Book> _books = new List<Book>();

    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisterBookJason), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Create([FromBody] RequestRegisterBookJson request) //Criar//
    {
        // Validar request (regras de negócio)
        if (request == null)
        {
            return BadRequest("Dados Inválidos.");
        }

        if (string.IsNullOrWhiteSpace(request.Title) ||
            string.IsNullOrWhiteSpace(request.Author) ||
            string.IsNullOrWhiteSpace(request.Genre))
        {
            return BadRequest("Título, Autor e Gênero são obrigatórios.");
        }

        if (request.Price <=0)
        {
            return BadRequest("O preço deve ser maior que zero.");
        }

        if (request.Stock <0)
        {
            return BadRequest("O estoque não pode ser negativo.");
        }

        // Mapear entidade do domínio
        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            Genre = request.Genre,
            Price = request.Price,
            Stock = request.Stock,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        // Mapear entidade de resposta
        var response = new ResponseRegisterBookJason
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            Price = book.Price,
            Stock = book.Stock,
            CreatedAt = DateTime.UtcNow,
        };

        //Retorno Created 201 apontando para o endpoint de busca por ID
        return CreatedAtAction(nameof(GetById), new {id = response.Id}, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<RequestRegisterBookJson>), StatusCodes.Status200OK)]
    public IActionResult GetAll() //Listar//
    {
        var books = new List<RequestRegisterBookJson>();

        var response = books.Select(Book => new ResponseRegisterBookJason
        {
            Id = Book.Id,
            Title = Book.Title,
            Author = Book.Author,
            Genre = Book.Genre,
            Price = Book.Price,
            Stock = Book.Stock,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        return Ok(response);
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(ResponseRegisterBookJason), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById([FromRoute] Guid id) //Obter por ID//
    {
        var book = _books.FirstOrDefault(book => book.Id == id);

        if (book == null)
        {
            return NotFound(new { Message = "Livro não encontrado"});
        }

        var response = new ResponseRegisterBookJason
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            Price = book.Price,
            Stock = book.Stock,
            CreatedAt = DateTime.UtcNow
        };

        return Ok(response);
    }

    [HttpPut]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update([FromRoute] Guid id, [FromBody] RequestUpdateBookJson request) //Atualizar//
    {
        if (request == null)
        {
            return BadRequest("Dados Inválidos.");
        }

        if (string.IsNullOrEmpty(request.Title) ||
            string.IsNullOrEmpty(request.Author) ||
            string.IsNullOrEmpty(request.Genre))
        {
            return BadRequest("Título, Autor e Gênero são obrigatórios.");
        }

        return NoContent();
    }

    [HttpDelete]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete([FromRoute] Guid id) //Deletar//
    {
        var book = _books.FirstOrDefault(b => b.Id == id);

        if (book == null)
        {
            return NotFound(new { Message = "Livro não encontrado."});
        }

        _books.Remove(book);

        return NoContent();
    }
}