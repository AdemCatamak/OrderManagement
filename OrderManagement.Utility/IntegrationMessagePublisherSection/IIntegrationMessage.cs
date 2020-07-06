namespace OrderManagement.Utility.IntegrationMessagePublisherSection
{
    public interface IIntegrationMessage
    {
    }

    public interface IIntegrationEvent : IIntegrationMessage
    {
    }

    public interface IIntegrationCommand : IIntegrationMessage
    {
    }
}