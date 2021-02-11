namespace ProjectX.Core.SeedWork
{
    public interface IPaginationOptions
    {
        int Skip { get; }

        int Take { get; }
    }
}
