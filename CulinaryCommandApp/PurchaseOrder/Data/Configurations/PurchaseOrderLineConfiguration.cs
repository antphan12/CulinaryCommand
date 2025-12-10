using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CulinaryCommand.PurchaseOrder.Entities;

namespace CulinaryCommand.PurchaseOrder.Data.Configurations
{
    public class PurchaseOrderLineConfiguration : IEntityTypeConfiguration<CulinaryCommand.PurchaseOrder.Entities.PurchaseOrderLine>
    {
        public void Configure(EntityTypeBuilder<CulinaryCommand.PurchaseOrder.Entities.PurchaseOrderLine> builder)
        {
            builder.HasKey(line => line.Id);

            builder.Property(line => line.QuantityOrdered)
                   .HasPrecision(18, 3)
                   .IsRequired();

            builder.Property(line => line.QuantityReceived)
                   .HasPrecision(18, 3)
                   .HasDefaultValue(0m);

            builder.Property(line => line.UnitPrice)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.HasOne(line => line.Unit)
                   .WithMany()
                   .HasForeignKey(line => line.UnitId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(line => line.Ingredient)
                   .WithMany()
                   .HasForeignKey(line => line.IngredientId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(line => line.PurchaseOrder)
                   .WithMany(purchase_order => purchase_order.Lines)
                   .HasForeignKey(line => line.PurchaseOrderId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(line => line.PurchaseOrderId);
            builder.HasIndex(line => line.IngredientId);
            builder.HasIndex(line => line.UnitId);
        }
    }
}