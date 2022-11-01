using Library.Models;

namespace Library.Data
{
    public record CreateBookDto
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string CategoryName { get; set; }
    }
}
