namespace Libro.DAL.Configurations
{
    internal class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.Title).HasMaxLength(500);
            builder.Property(x => x.Publisher).HasMaxLength(200);
            builder.Property(x => x.Hall).HasMaxLength(50);

            builder.HasIndex(x => new { x.Title , x.AuthorId})
                    .IsUnique();
        }
    }
}
