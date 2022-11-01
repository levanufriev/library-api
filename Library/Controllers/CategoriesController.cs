using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoriesController : Controller
    {
        private readonly IRepository<Book> bookRepository;
        private readonly IRepository<Category> categoryRepository;

        public CategoriesController(IRepository<Book> bookRepository, IRepository<Category> categoryRepository)
        {
            this.bookRepository = bookRepository;
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            return (await categoryRepository.GetAllAsync(c => c.Books)).Select(c => c.AsDto());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryAsync(int id)
        {
            var category = await categoryRepository.GetAsync(id, c => c.Books);

            if (category == null)
            {
                return NotFound();
            }

            return category.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategoryAsync(CreateCategoryDto category)
        {
            var books = await GetBooksList(category);

            Category newCategory = new()
            {
                Name = category.Name,
                Books = books
            };

            await categoryRepository.CreateAsync(newCategory);
            return CreatedAtAction(nameof(GetCategoryAsync), new { Id = newCategory.Id }, newCategory.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryAsync(int id, CreateCategoryDto category)
        {
            var existingCategory = await categoryRepository.GetAsync(id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            var books = await GetBooksList(category);

            existingCategory.Name = category.Name;
            existingCategory.Books = books;

            await categoryRepository.UpdateAsync(existingCategory);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            var existingCategory = await categoryRepository.GetAsync(id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            await categoryRepository.DeleteAsync(id);
            return NoContent();
        }

        private async Task<List<Book>> GetBooksList(CreateCategoryDto category)
        {
            var books = new List<Book>();
            foreach (var book in category.Books)
            {
                var existingBook = (await bookRepository
                    .GetAsync(b => b.Title == book.Title && b.Author == book.Author && b.ReleaseDate == book.ReleaseDate))
                    .FirstOrDefault();

                if (existingBook == null)
                {
                    Book newBook = new()
                    {
                        Title = book.Title,
                        Author = book.Author,
                        ReleaseDate = book.ReleaseDate
                    };

                    books.Add(newBook);
                }
                else
                {
                    books.Add(existingBook);
                }
            }

            return books;
        }
    }
}
