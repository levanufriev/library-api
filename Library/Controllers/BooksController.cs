using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [ApiController]
    [Route("books")]
    public class BooksController : Controller
    {
        private readonly IRepository<Book> bookRepository;
        private readonly IRepository<Category> categoryRepository;

        public BooksController(IRepository<Book> bookRepository, IRepository<Category> categoryRepository)
        {
            this.bookRepository = bookRepository;
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<BookDto>> GetBooksAsync()
        {
            return (await bookRepository.GetAllAsync(b => b.Category)).Select(book => book.AsDto());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBookAsync(int id)
        {
            var book = await bookRepository.GetAsync(id, b => b.Category);

            if (book == null)
            {
                return NotFound();
            }

            return book.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> CreateBookAsync(CreateBookDto book)
        {
            var category = (await categoryRepository.GetAsync(c => c.Name == book.CategoryName, c => c.Books)).FirstOrDefault();

            Book newBook = new()
            {
                Title = book.Title,
                Author = book.Author,
                ReleaseDate = book.ReleaseDate
            };

            if (category == null)
            {
                Category newCategory = new()
                {
                    Name = book.CategoryName
                };

                newCategory.Books.Add(newBook);

                await categoryRepository.CreateAsync(newCategory);
                return CreatedAtAction(nameof(GetBookAsync), new { Id = newBook.Id }, newBook.AsDto());
            }

            newBook.CategoryId = category.Id;
            category.Books.Add(newBook);
            await bookRepository.CreateAsync(newBook);
            string categoryName = (await categoryRepository.GetAsync(c => c.Id == newBook.CategoryId))
                .Select(c => c.Name).FirstOrDefault();

            return CreatedAtAction(nameof(GetBookAsync), new { Id = newBook.Id }, newBook.AsDto(categoryName));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookAsync(int id, CreateBookDto book)
        {
            var existingBook = await bookRepository.GetAsync(id);

            if (existingBook == null)
            {
                return NotFound();
            }

            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.ReleaseDate = book.ReleaseDate;

            var category = (await categoryRepository.GetAsync(c => c.Name == book.CategoryName, c => c.Books)).FirstOrDefault();

            if (category == null)
            {
                Category newCategory = new()
                {
                    Name = book.CategoryName
                };

                newCategory.Books.Add(existingBook);

                await categoryRepository.CreateAsync(newCategory);

                await bookRepository.UpdateAsync(existingBook);
            }
            else
            {
                existingBook.CategoryId = category.Id;
                await bookRepository.UpdateAsync(existingBook);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            var existingBook = await bookRepository.GetAsync(id);

            if (existingBook == null)
            {
                return NotFound();
            }

            await bookRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
