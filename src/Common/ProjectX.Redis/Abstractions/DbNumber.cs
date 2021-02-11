namespace ProjectX.Redis.Abstractions
{
    public abstract class DbNumber
    {
        public readonly int Value;
        public DbNumber(int value) => Value = value;
    }
}
