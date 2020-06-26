using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderManagement.Data.Models.ModelConfigs
{
    public class OrderModelConfig : IEntityTypeConfiguration<OrderModel>
    {
        public void Configure(EntityTypeBuilder<OrderModel> builder)
        {
            builder.ToTable("Order");

            builder.HasKey(model => model.Id);

            builder.Property(m => m.BuyerAddress).IsRequired();
            builder.Property(m => m.BuyerName).IsRequired();
            builder.Property(m => m.CreatedOn).IsRequired();
            builder.Property(m => m.TotalAmount).IsRequired();
            builder.Property(m => m.UpdatedOn).IsRequired();

            builder.Property(m => m.UpdatedOn).IsRowVersion();
        }
    }
}