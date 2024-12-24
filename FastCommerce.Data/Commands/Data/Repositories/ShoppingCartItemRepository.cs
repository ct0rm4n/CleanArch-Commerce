using Core.Entities.Domain.Checkout;
using Core.ViewModel.Generic.Abstracts;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
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
    }
}
