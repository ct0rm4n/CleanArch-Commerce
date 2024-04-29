using Dapper;
using FastCommerce.Core.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FastCommerce.Data
{
    public class DbContext : IDbContext
    {
        private readonly string _connectionString;
        private IDbConnection _connection { get; set; }
        private IDbTransaction _transaction { get; set; }

        protected IDbConnection Connection
        {
            get
            {
                if (_connection == null || _connection.State != ConnectionState.Open)
                {
                    _connection = new SqlConnection(_connectionString);
                    _connection.Open();
                }

                return _connection;
            }
        }
        public DbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        //public virtual T QueryFirst<T>(string query, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null)
        //{
        //    if (transaction == null)
        //    {
        //        return Transaction(_transaction => {
        //            var result = Connection.QueryFirstOrDefault<T>(query, param, _transaction, commandType: commandType);
        //            return result;
        //        });
        //    }
        //    else
        //    {
        //        return Connection.QueryFirstOrDefault<T>(query, param, transaction: transaction, commandType: commandType);
        //    }
        //}

        //public virtual IEnumerable<T> Query<T>(string query, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null)
        //{
        //    if (transaction == null)
        //    {
        //        return Transaction(_transaction => {
        //            var result = Connection.Query<T>(query, param, _transaction, commandType: commandType);
        //            return result;
        //        });
        //    }
        //    else
        //    {
        //        return Connection.Query<T>(query, param, transaction: transaction, commandType: commandType);
        //    }
        //}

        //public virtual int InsertSingle(string sql, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null, int? timeout = null)
        //{
        //    var idQueryCheck = "CAST(SCOPE_IDENTITY() AS INT)";

        //    if (!sql.ToUpper().Contains(idQueryCheck))
        //    {
        //        sql = $@"{sql}
        //        SELECT {idQueryCheck};";
        //    }

        //    if (transaction == null)
        //    {
        //        return Transaction(_transaction => {
        //            var result = Connection.QueryFirstOrDefault<int>(sql, param, _transaction, commandType: commandType, commandTimeout: timeout);
        //            return result;
        //        });
        //    }
        //    else
        //    {
        //        return Connection.QueryFirstOrDefault<int>(sql, param, transaction: transaction, commandType: commandType, commandTimeout: timeout);
        //    }
        //}

        //public virtual int Command(string sql, object param = null, CommandType commandType = CommandType.Text, IDbTransaction transaction = null, int? timeout = null)
        //{
        //    if (transaction == null)
        //    {
        //        return Transaction(_transaction => Connection.Execute(sql, param, _transaction, commandType: commandType, commandTimeout: timeout));
        //    }
        //    else
        //    {
        //        return Connection.Execute(sql, param, transaction, commandType: commandType, commandTimeout: timeout);
        //    }
        //}

        public virtual T Transaction<T>(Func<IDbTransaction, T> query)
        {
            using var connection = Connection;
            using var transaction = BeginTransaction();

            try
            {
                var result = query(transaction);
                transaction.Commit();

                return result;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public virtual void Transaction(Action<IDbTransaction> query)
        {
            using var connection = Connection;
            using var transaction = BeginTransaction();

            try
            {
                query(transaction);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        protected IDbTransaction BeginTransaction()
        {
            if (_transaction == null || _transaction.Connection == null)
            {
                _transaction = Connection.BeginTransaction();
            }

            return _transaction;
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection = null;
            }
        }
        ~DbContext()
        {
            Dispose();
        }
    }
}
