using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Lopushok.Models;

public partial class LopushokContext : DbContext
{
    public LopushokContext()
    {
    }

    public LopushokContext(DbContextOptions<LopushokContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agent> Agents { get; set; }

    public virtual DbSet<Agentpriorityhistory> Agentpriorityhistories { get; set; }

    public virtual DbSet<Agenttype> Agenttypes { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Materialcounthistory> Materialcounthistories { get; set; }

    public virtual DbSet<Materialtype> Materialtypes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Productcosthistory> Productcosthistories { get; set; }

    public virtual DbSet<Productmaterial> Productmaterials { get; set; }

    public virtual DbSet<Productsale> Productsales { get; set; }

    public virtual DbSet<Producttype> Producttypes { get; set; }

    public virtual DbSet<Shop> Shops { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5438;Database=lopushok;Username=nastya;Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("agent_pkey");

            entity.ToTable("agent");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(300)
                .HasColumnName("address");
            entity.Property(e => e.Agenttypeid).HasColumnName("agenttypeid");
            entity.Property(e => e.Directorname)
                .HasMaxLength(100)
                .HasColumnName("directorname");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Inn)
                .HasMaxLength(12)
                .HasColumnName("inn");
            entity.Property(e => e.Kpp)
                .HasMaxLength(9)
                .HasColumnName("kpp");
            entity.Property(e => e.Logo)
                .HasMaxLength(100)
                .HasColumnName("logo");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .HasColumnName("title");

            entity.HasOne(d => d.Agenttype).WithMany(p => p.Agents)
                .HasForeignKey(d => d.Agenttypeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_agent_agenttype");
        });

        modelBuilder.Entity<Agentpriorityhistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("agentpriorityhistory_pkey");

            entity.ToTable("agentpriorityhistory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Agentid).HasColumnName("agentid");
            entity.Property(e => e.Changedate)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("changedate");
            entity.Property(e => e.Priorityvalue).HasColumnName("priorityvalue");

            entity.HasOne(d => d.Agent).WithMany(p => p.Agentpriorityhistories)
                .HasForeignKey(d => d.Agentid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_agentpriorityhistory_agent");
        });

        modelBuilder.Entity<Agenttype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("agenttype_pkey");

            entity.ToTable("agenttype");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Image)
                .HasMaxLength(100)
                .HasColumnName("image");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("material_pkey");

            entity.ToTable("material");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cost)
                .HasPrecision(10, 2)
                .HasColumnName("cost");
            entity.Property(e => e.Countinpack).HasColumnName("countinpack");
            entity.Property(e => e.Countinstock).HasColumnName("countinstock");
            entity.Property(e => e.Materialtypeid).HasColumnName("materialtypeid");
            entity.Property(e => e.Mincount).HasColumnName("mincount");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.Unit)
                .HasMaxLength(10)
                .HasColumnName("unit");

            entity.HasOne(d => d.Materialtype).WithMany(p => p.Materials)
                .HasForeignKey(d => d.Materialtypeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_material_materialtype");

            entity.HasMany(d => d.Suppliers).WithMany(p => p.Materials)
                .UsingEntity<Dictionary<string, object>>(
                    "Materialsupplier",
                    r => r.HasOne<Supplier>().WithMany()
                        .HasForeignKey("Supplierid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_materialsupplier_supplier"),
                    l => l.HasOne<Material>().WithMany()
                        .HasForeignKey("Materialid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_materialsupplier_material"),
                    j =>
                    {
                        j.HasKey("Materialid", "Supplierid").HasName("materialsupplier_pkey");
                        j.ToTable("materialsupplier");
                        j.IndexerProperty<int>("Materialid").HasColumnName("materialid");
                        j.IndexerProperty<int>("Supplierid").HasColumnName("supplierid");
                    });
        });

        modelBuilder.Entity<Materialcounthistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("materialcounthistory_pkey");

            entity.ToTable("materialcounthistory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Changedate)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("changedate");
            entity.Property(e => e.Countvalue).HasColumnName("countvalue");
            entity.Property(e => e.Materialid).HasColumnName("materialid");

            entity.HasOne(d => d.Material).WithMany(p => p.Materialcounthistories)
                .HasForeignKey(d => d.Materialid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_materialcounthistory_material");
        });

        modelBuilder.Entity<Materialtype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("materialtype_pkey");

            entity.ToTable("materialtype");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_pkey");

            entity.ToTable("product");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Articlenumber)
                .HasMaxLength(10)
                .HasColumnName("articlenumber");
            entity.Property(e => e.Image)
                .HasMaxLength(100)
                .HasColumnName("image");
            entity.Property(e => e.Mincostforagent)
                .HasPrecision(10, 2)
                .HasColumnName("mincostforagent");
            entity.Property(e => e.Productionpersoncount).HasColumnName("productionpersoncount");
            entity.Property(e => e.Productionworkshopnumber).HasColumnName("productionworkshopnumber");
            entity.Property(e => e.Producttypeid).HasColumnName("producttypeid");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");

            entity.HasOne(d => d.Producttype).WithMany(p => p.Products)
                .HasForeignKey(d => d.Producttypeid)
                .HasConstraintName("fk_product_producttype");
        });

        modelBuilder.Entity<Productcosthistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("productcosthistory_pkey");

            entity.ToTable("productcosthistory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Changedate)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("changedate");
            entity.Property(e => e.Costvalue)
                .HasPrecision(10, 2)
                .HasColumnName("costvalue");
            entity.Property(e => e.Productid).HasColumnName("productid");

            entity.HasOne(d => d.Product).WithMany(p => p.Productcosthistories)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_productcosthistory_product");
        });

        modelBuilder.Entity<Productmaterial>(entity =>
        {
            entity.HasKey(e => new { e.Productid, e.Materialid }).HasName("productmaterial_pkey");

            entity.ToTable("productmaterial");

            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Materialid).HasColumnName("materialid");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Material).WithMany(p => p.Productmaterials)
                .HasForeignKey(d => d.Materialid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_productmaterial_material");

            entity.HasOne(d => d.Product).WithMany(p => p.Productmaterials)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_productmaterial_product");
        });

        modelBuilder.Entity<Productsale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("productsale_pkey");

            entity.ToTable("productsale");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Agentid).HasColumnName("agentid");
            entity.Property(e => e.Productcount).HasColumnName("productcount");
            entity.Property(e => e.Productid).HasColumnName("productid");
            entity.Property(e => e.Saledate).HasColumnName("saledate");

            entity.HasOne(d => d.Agent).WithMany(p => p.Productsales)
                .HasForeignKey(d => d.Agentid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_productsale_agent");

            entity.HasOne(d => d.Product).WithMany(p => p.Productsales)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_productsale_product");
        });

        modelBuilder.Entity<Producttype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("producttype_pkey");

            entity.ToTable("producttype");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Shop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shop_pkey");

            entity.ToTable("shop");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(300)
                .HasColumnName("address");
            entity.Property(e => e.Agentid).HasColumnName("agentid");
            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .HasColumnName("title");

            entity.HasOne(d => d.Agent).WithMany(p => p.Shops)
                .HasForeignKey(d => d.Agentid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_shop_agent");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("supplier_pkey");

            entity.ToTable("supplier");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Inn)
                .HasMaxLength(12)
                .HasColumnName("inn");
            entity.Property(e => e.Qualityrating).HasColumnName("qualityrating");
            entity.Property(e => e.Startdate).HasColumnName("startdate");
            entity.Property(e => e.Suppliertype)
                .HasMaxLength(20)
                .HasColumnName("suppliertype");
            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .HasColumnName("title");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
