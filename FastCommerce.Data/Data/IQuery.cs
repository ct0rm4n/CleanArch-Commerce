using System.Data;

namespace Core.Data
{
    public interface IQuery<T>
    {
        string Sql { get; set; }

        T Execute(IDbContext context, IDbTransaction transaction = null);
    }
}
