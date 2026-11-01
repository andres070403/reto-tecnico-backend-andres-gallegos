using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pedidos.Domain.Entities;
using Pedidos.Infrastructure.Persistence.Seed;

namespace Pedidos.Infrastructure.Persistence.Configurations;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios", t => t.HasCheckConstraint("CK_Usuarios_Rol", "[Rol] IN ('Admin','User')"));

        builder.HasKey(u => u.Id).HasName("PK_Usuarios");

        builder.Property(u => u.Nombre)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("UQ_Usuarios_Email");

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.Rol)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.Activo)
            .HasDefaultValue(true);

        builder.Property(u => u.FechaCreacion)
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");

        builder.HasData(SeedData.Usuarios);
    }
}
