namespace ProjectX.Core.Payload
{
    public class EventArgs<T> : System.EventArgs
    {
        public T Value { get; }

        public EventArgs(T value)
        {
            Value = value;
        }
    }
}
