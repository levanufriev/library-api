using Library.Data;
using Library.Models;

namespace Library
{
    public static class Extensions
    {        
        public static BookDto AsDto(this Book book)
        {
            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ReleaseDate = book.ReleaseDate,
                CategoryName = book.Category.Name
            };
        }

        public static BookDto AsDto(this Book book, string categoryName)
        {
            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ReleaseDate = book.ReleaseDate,
                CategoryName = categoryName
            };
        }

        public static CategoryDto AsDto(this Category category)
        {
            var titlesList = new List<string>();
            foreach (var book in category.Books)
            {
                titlesList.Add(book.Title);
            }

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                BookTitles = titlesList
            };
        }
    }   
}

