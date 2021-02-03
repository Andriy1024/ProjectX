namespace ProjectX.Common.SeedWork
{
    public interface IPaginationOptions
    {
        int Skip { get; }

        int Take { get; }
    }
}
