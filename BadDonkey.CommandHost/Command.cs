
namespace BadDonkey.CommandHost
{
    public abstract class Command : ICommand
    {
        public string Name { get; set; }
    }
}
