using System;

namespace OrderManagement.Data.Models.BaseModels
{
    public interface IUpdateAudit
    {
        DateTime UpdatedOn { get; }
    }
}