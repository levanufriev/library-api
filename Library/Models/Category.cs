using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Category : IEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
