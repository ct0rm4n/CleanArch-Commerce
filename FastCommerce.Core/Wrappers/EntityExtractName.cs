using System.ComponentModel.DataAnnotations.Schema;

namespace FastCommerce.Core.Wrappers
{
    public class EntityUtils()
    {
        public string GetNameofEntity(Type classe)
        {
            if (classe.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
            {
                TableAttribute atributoTabela = (TableAttribute)classe.GetCustomAttributes(typeof(TableAttribute), true)[0];
                return atributoTabela.Name;
            }
            return null;

        }
    }
}