namespace ProjectX.FileStorage.Persistence.Database.Abstractions
{
    public interface IIdentifiable<TKey>
    {
        public TKey Id { get; }
    }
}
