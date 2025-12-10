using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CulinaryCommand.PurchaseOrder.Entities;

namespace CulinaryCommand.PurchaseOrder.Data.Configurations
{
    public class PurchaseOrderConfiguration : IEntityTypeConfiguration<CulinaryCommand.PurchaseOrder.Entities.PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<CulinaryCommand.PurchaseOrder.Entities.PurchaseOrder> builder)
        {
            builder.HasKey(purchase_order => purchase_order.Id);

            builder.HasIndex(purchase_order => purchase_order.OrderNumber).IsUnique();

            builder.HasOne(purchase_order => purchase_order.Location)
                   .WithMany()
                   .HasForeignKey(purchase_order => purchase_order.LocationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(purchase_order => purchase_order.VendorName)
                   .HasMaxLength(256)
                   .IsRequired();
        }
    }
}