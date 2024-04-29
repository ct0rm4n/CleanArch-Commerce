using System.Data;

namespace FastCommerce.Core.Data
{
    public interface IQuery<T>
    {
        string Sql { get; set; }

        T Execute(IDbContext context, IDbTransaction transaction = null);
    }
}
