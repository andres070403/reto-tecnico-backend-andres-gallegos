using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pedidos.Domain.Entities;
using Pedidos.Infrastructure.Persistence.Seed;

namespace Pedidos.Infrastructure.Persistence.Configurations;

public sealed class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos", t =>
        {
            t.HasCheckConstraint("CK_Pedidos_Total", "[Total] > 0");
            t.HasCheckConstraint("CK_Pedidos_Estado", "[Estado] IN ('Registrado','EnProceso','Completado','Cancelado')");
        });

        builder.HasKey(p => p.Id).HasName("PK_Pedidos");

        builder.Property(p => p.NumeroPedido)
            .HasMaxLength(Pedido.NumeroPedidoMaxLength)
            .IsUnicode(false)
            .IsRequired();

        builder.HasIndex(p => p.NumeroPedido)
            .IsUnique()
            .HasDatabaseName("UQ_Pedidos_NumeroPedido");

        builder.Property(p => p.Cliente)
            .HasMaxLength(Pedido.ClienteMaxLength)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(p => p.Fecha)
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(p => p.Total)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(p => p.Estado)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(p => p.Eliminado)
            .HasDefaultValue(false);

        builder.Property(p => p.UsuarioCreacionId)
            .IsRequired();

        builder.Property(p => p.FechaCreacion)
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(p => p.UsuarioModificacionId);

        builder.Property(p => p.FechaActualizacion)
            .HasColumnType("datetime");

        builder.HasOne(p => p.UsuarioCreacion)
            .WithMany()
            .HasForeignKey(p => p.UsuarioCreacionId)
            .HasConstraintName("FK_Pedidos_UsuarioCreacion")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(p => p.UsuarioModificacion)
            .WithMany()
            .HasForeignKey(p => p.UsuarioModificacionId)
            .HasConstraintName("FK_Pedidos_UsuarioModificacion")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasQueryFilter(p => !p.Eliminado);

        builder.HasData(SeedData.Pedidos);
    }
}
