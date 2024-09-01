namespace Template.Netcode;

[AttributeUsage(AttributeTargets.Property)]
public class NetSendAttribute : Attribute
{
    public int Order { get; }

    public NetSendAttribute(int order)
    {
        Order = order;
    }
}
