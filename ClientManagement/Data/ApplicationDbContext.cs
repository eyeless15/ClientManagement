using ClientManagement.Models.DAO;
using Microsoft.EntityFrameworkCore;

namespace ClientManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

   
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contacts");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd();
                entity.Property(c => c.Phone).IsRequired().HasMaxLength(255);
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(c => c.UpdatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(c => c.Phone).HasDatabaseName("IX_Contact_Phone");
            });


            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd();


                entity.HasOne(c => c.Contact)
                      .WithMany() 
                      .HasForeignKey(c => c.ContactId) 
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(c => c.Name).IsRequired().HasMaxLength(255);
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(c => c.UpdatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(c => c.ContactId).HasDatabaseName("IX_Customer_ContactId");
            });
        }
    }
}
