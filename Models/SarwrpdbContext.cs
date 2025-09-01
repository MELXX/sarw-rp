using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using sarw_rp.Models.DbModels;

namespace sarw_rp.Models;

public partial class SarwrpdbContext : DbContext
{

    public DbSet<Article> Articles { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public SarwrpdbContext()
    {
    }

    public SarwrpdbContext(DbContextOptions<SarwrpdbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
