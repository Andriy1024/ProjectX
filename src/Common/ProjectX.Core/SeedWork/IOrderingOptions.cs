namespace ProjectX.Core.SeedWork
{
    public interface IOrderingOptions
    {
        string OrderBy { get; }

        bool Descending { get; }
    }
}
