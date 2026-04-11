using JobBoard.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<JobPosting> Jobs => Set<JobPosting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.HasKey(job => job.Id);
            entity.Property(job => job.Title).HasMaxLength(120).IsRequired();
            entity.Property(job => job.Company).HasMaxLength(120).IsRequired();
            entity.Property(job => job.Location).HasMaxLength(120).IsRequired();
            entity.Property(job => job.WorkMode).HasMaxLength(40).IsRequired();
            entity.Property(job => job.CreatedBy).HasMaxLength(120).IsRequired();
        });
    }
}