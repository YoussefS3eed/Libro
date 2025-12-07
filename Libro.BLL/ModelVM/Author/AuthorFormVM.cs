namespace Libro.BLL.ModelVM.Author
{
    public class AuthorFormVM
    {
        public int Id { get; set; }
        [MaxLength(100, ErrorMessage = "Max length Can not be more than {1} character!"), Display(Name = "Author")]
        [UniqueName(typeof(AuthorService))]
        public string Name { get; set; } = null!;
    }
}
