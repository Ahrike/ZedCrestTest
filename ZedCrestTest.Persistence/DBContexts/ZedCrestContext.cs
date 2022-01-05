using Domain;
using Microsoft.EntityFrameworkCore;


namespace ZedCrestTest.Persistence.DBContexts
{
    public partial class ZedCrestContext : DbContext
    {
        public ZedCrestContext()
        {
        }

        public ZedCrestContext(DbContextOptions<ZedCrestContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserDocument> UserDocuments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.EmailAddress, "EmailAddress_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.UserName, "UserName_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(45);
            });

            modelBuilder.Entity<UserDocument>(entity =>
            {
                entity.HasKey(e => e.DocumentId)
                    .HasName("PRIMARY");

                entity.ToTable("UserDocument");

                entity.HasIndex(e => e.BatchDocumentReference, "BatchDocumentReference_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.DocumentPublicId, "DocumentPublicId_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.UserId, "UserId_idx");

                entity.Property(e => e.BatchDocumentReference)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DocumentPublicId)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.DocumentTitle)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.DocumentUrl)
                    .IsRequired()
                    .HasColumnType("LONGTEXT");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserDocuments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UserId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
