using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LibraryWebApplication
{
    public partial class LibraryContext : DbContext
    {
        public LibraryContext()
        {
        }

        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Authors> Authors { get; set; }
        public virtual DbSet<Authorship> Authorship { get; set; }
        public virtual DbSet<BookCategory> BookCategory { get; set; }
        public virtual DbSet<BookReading> BookReading { get; set; }
        public virtual DbSet<Books> Books { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Readers> Readers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=localhost; Database=Library; Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Authors>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnName("date_of_birth")
                    .HasColumnType("date");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("full_name")
                    .HasMaxLength(50);


            });

            modelBuilder.Entity<Authorship>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Authorship)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("FK_Authorship_Authors");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.Authorship)
                    .HasForeignKey(d => d.BookId)
                    .HasConstraintName("FK_Authorship_Books");
            });

            modelBuilder.Entity<BookCategory>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.BookCategory)
                    .HasForeignKey(d => d.BookId)
                    .HasConstraintName("FK_BookCategory_Categories");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.BookCategory)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_BookCategory_Categories1");
            });

            modelBuilder.Entity<BookReading>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateOfIssue)
                    .HasColumnName("date_of_issue")
                    .HasColumnType("date");

                entity.Property(e => e.ReturnDate)
                    .HasColumnName("return_date")
                    .HasColumnType("date");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.BookReading)
                    .HasForeignKey(d => d.BookId)
                    .HasConstraintName("FK_BookReading_Books");

                entity.HasOne(d => d.Reader)
                    .WithMany(p => p.BookReading)
                    .HasForeignKey(d => d.ReaderId)
                    .HasConstraintName("FK_BookReading_Readers2");
            });

            modelBuilder.Entity<Books>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);



                entity.Property(e => e.NumberOfPages).HasColumnName("number_of_pages");

                entity.Property(e => e.YearOfPublication).HasColumnName("year_of_publication");
            });

            modelBuilder.Entity<Categories>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasColumnName("category_name")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Readers>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("full_name")
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
