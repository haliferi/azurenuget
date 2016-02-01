namespace AzureCommon
{
    public interface ISubscriptionConfig
    {
        string ClientId { get; }
        string Password { get; }
        string SubscriptionId { get; }
        string Tenant { get; }
        string UserName { get; }
    }
}