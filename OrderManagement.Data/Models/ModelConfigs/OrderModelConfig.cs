using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderManagement.Data.Models.ModelConfigs
{
    public class OrderModelConfig : SagaClassMap<OrderModel>
    {
        protected override void Configure(EntityTypeBuilder<OrderModel> builder, ModelBuilder model)
        {
            builder.ToTable("Order");

            builder.Property(m => m.BuyerAddress).IsRequired();
            builder.Property(m => m.BuyerName).IsRequired();
            builder.Property(m => m.CreatedOn).IsRequired();
            builder.Property(m => m.TotalAmount).IsRequired();
            builder.Property(m => m.UpdatedOn).IsRequired();

            builder.Property(m => m.RowVersion).IsRowVersion();

            builder.Ignore(m => m.OrderId);
        }
    }
}