
namespace Core.Data
{
    public interface ICommand : IQuery<Tuple<int, string>> { }
}
