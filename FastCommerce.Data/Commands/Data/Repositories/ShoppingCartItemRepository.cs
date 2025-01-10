using Core.Entities.Domain;
using Core.Entities.Domain.Checkout;
using Core.ViewModel.Generic.Abstracts;
using Core.Wrappers;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;
using static Dapper.SqlMapper;
namespace Data.Commands.Data.Repositories
{
    public class ShoppingCartItemRepository : GenericRepository<ShoppingCartItem, IBaseVM>
    {
        private readonly string schema = "FastCommerce.dbo";
        public ShoppingCartItemRepository(IConfiguration configuration) : base(configuration)
        {
        }
        public (ShoppingCartItem, bool) AddOrUpdate(ShoppingCartItem entity)
        {
            var connection = this.GetConnection();
            IEnumerable<ShoppingCartItem> result = null;
            try
            {
                string tableName = GetTableName();
                string columns = GetColumns(excludeKey: true);
                string properties = GetPropertyNames(excludeKey: true);
                string propertiesValues = GetPropertyValues(entity, excludeKey: true);

                // Obter valores das propriedades UserId e ProductId
                var userId = GetPropertyValue(entity, "UserId");
                var productId = GetPropertyValue(entity, "ProductId");
                var quantity = GetPropertyValue(entity, "Quantity");

                if (userId == null || productId == null || quantity == null)
                {
                    throw new Exception("UserId, ProductId ou Quantity não podem ser nulos.");
                }

                string query = $@"
                                IF EXISTS (SELECT 1 FROM {schema}.{tableName} WHERE UserId = @UserId AND ProductId = @ProductId)
                                BEGIN
                                    UPDATE {schema}.{tableName}
                                    SET Quantity = Quantity + @Quantity,
                                        LastModifiedDate = GETDATE()
                                    WHERE UserId = @UserId AND ProductId = @ProductId;
                                    SELECT * FROM {schema}.{tableName} WHERE UserId = @UserId AND ProductId = @ProductId;
                                END
                                ELSE
                                BEGIN
                                    INSERT INTO {schema}.{tableName} ({columns}) OUTPUT inserted.* VALUES ({propertiesValues});
                                END";
                
                connection.Open();
                result = connection.Query<ShoppingCartItem>(query, new { UserId = userId, ProductId = productId, Quantity = quantity });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                return (null, false);
            }
            finally
            {
                connection.Close();
            }
            return (result.FirstOrDefault(), result.Any());
        }


        public List<Tuple<ShoppingCartItem, Product>>? GetAllByCustomerId(int customerId)
        {
            var connection = this.GetConnection();
            string tableName = GetTableName();
            string query = $@"
                SELECT scitem.*, p.* 
                FROM {schema}.{tableName} as scitem
                LEFT JOIN {schema}.Product p ON scitem.ProductId = p.Id
                WHERE scitem.UserId = @UserId";

            connection.Open();
            var result = connection.Query<ShoppingCartItem, Product, Tuple<ShoppingCartItem, Product>>(
                query,
                (scitem, p) => new Tuple<ShoppingCartItem, Product>(scitem, p),
                param: new { UserId = customerId },
                splitOn: "InsertedDate");

            connection.Close();
            return result.ToList();
        }
    }
}
