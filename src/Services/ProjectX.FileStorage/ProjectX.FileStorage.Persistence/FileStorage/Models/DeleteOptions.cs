namespace ProjectX.FileStorage.Persistence.FileStorage.Models
{
    public class DeleteOptions
    {
        public string Location { get; }

        public DeleteOptions(string location)
        {
            Location = location;
        }

    }
}
