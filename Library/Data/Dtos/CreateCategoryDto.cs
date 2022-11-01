namespace Library.Data
{
    public class CreateCategoryDto
    {
        public string Name { get; set; }

        public List<CreateBookInCategoryDto> Books { get; set; }
    }
}
