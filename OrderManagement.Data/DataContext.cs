using System.Collections.Generic;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Data.Models;
using OrderManagement.Data.Models.ModelConfigs;

namespace OrderManagement.Data
{
    public class DataContext : SagaDbContext
    {
        public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new OrderModelConfig(); }
        }

        public virtual DbSet<OrderModel> OrderModels { get; set; }
    }
}