using System;

namespace OrderManagement.Data.Models.BaseModels
{
    public interface ICreateAudit
    {
        DateTime CreatedOn { get; }
    }
}