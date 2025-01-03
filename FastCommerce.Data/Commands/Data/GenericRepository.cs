﻿using Dapper;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Text;
using System.Buffers;
using static Dapper.SqlMapper;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Data.Commands.Data
{
    public class GenericRepository<T, IBaseVM> : IGenericRepository<T, IBaseVM> where T : class
    {
        #region ===[ Private Members ]=============================================================

        private readonly IConfiguration configuration;
        private IDbConnection _connection;
        private readonly string schema = "FastCommerce.dbo";

        #endregion
        
        public GenericRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("SqlConnection"));
            this.configuration = configuration;
        }
        public IDbConnection GetConnection()
        {
            return new SqlConnection(configuration.GetConnectionString("SqlConnection"));
        }
        public bool Add(T entity)
        {
            int rowsEffected = 0;
            try
            {
                string tableName = GetTableName();
                string columns = GetColumns(excludeKey: true);
                string properties = GetPropertyNames(excludeKey: true);

                string query = $"INSERT INTO {schema}.{tableName} ({columns}) VALUES ({properties})";
                _connection.Open();

                var response = _connection.Query<string>(query, entity);

                rowsEffected = _connection.Execute(query, entity);
            }
            catch (Exception ex) { }

            return rowsEffected > 0 ? true : false;
        }
        public (T, bool) AddReturn(T entity)
        {
            IEnumerable<T> result = null;
            try
            {
                string tableName = GetTableName();
                string columns = GetColumns(excludeKey: true);
                string properties = GetPropertyNames(excludeKey: true);
                string propertiesValues = GetPropertyValues(entity, excludeKey: true);

                string query = $"INSERT INTO  {schema}.{tableName} ({columns}) OUTPUT inserted.* VALUES ({propertiesValues})";
                _connection.Open();
                result = _connection.Query<T>(query);
            }
            catch (Exception ex)
            {
                return (null, result.Count() > 0 ? true : false);
            }
            return (result.FirstOrDefault(), result.Count() > 0 ? true : false);
        }


        public IEnumerable<T> BulkInsertReturn(IEnumerable<T> entitys)
        {
            IEnumerable<T> result = null;
            try
            {
                string tableName = GetTableName();
                string columns = GetColumns(excludeKey: true);
                string properties = GetPropertyNames(excludeKey: true);

                var query = new StringBuilder();
                query.Append($"INSERT INTO  {schema}.{tableName} ({columns}) OUTPUT inserted.* VALUES");
                int count = 0;
                for (int i = 0; i < entitys.Count(); i++)
                {
                    count++;
                    var entity = entitys.ElementAt(i);
                    string propertyValues = GetPropertyValues(entity, excludeKey: true);
                    query.Append(count == 1 ? $" ({propertyValues})" : $", ({propertyValues})");
                }

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                _connection.Open();
                result = _connection.Query<T>(query.ToString());
                stopwatch.Stop();
                long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"Query execution time: {elapsedMilliseconds} milliseconds");

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        public bool Delete(T entity)
        {
            int rowsEffected = 0;
            try
            {
                string tableName = GetTableName();
                string keyColumn = GetKeyColumnName();
                string keyProperty = GetKeyPropertyName();
                string query = $"DELETE FROM  {schema}.{tableName} WHERE {keyColumn} = @{keyProperty}";
                _connection.Open();
                rowsEffected = _connection.Execute(query, entity);
            }
            catch (Exception ex) { }

            return rowsEffected > 0 ? true : false;
        }

        public IEnumerable<T> GetAll()
        {
            IEnumerable<T> result = null;
            try
            {
                string tableName = GetTableName();
                string query = $"SELECT * FROM  {schema}.{tableName}";
                _connection.Open();
                result = _connection.Query<T>(query);
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public (IEnumerable<T>, int) GetPagged(string text, List<string> enity, int page_size = 20, int page = 1)
        {
            IEnumerable<dynamic> result_abstract = null;
            int total = 0;
            try
            {
                string tableName = GetTableName();
                string whereClause = string.Empty;
                if (!string.IsNullOrEmpty(text) && enity?.Any() == true)
                {
                    var conditions = enity.Select(e => $"{e} ILIKE @Text");
                    whereClause = $"WHERE {string.Join(" OR ", conditions)}";
                }
                string query = $@"SELECT *, COUNT(*) OVER() AS total  
                FROM {schema}.{tableName} ORDER BY Id {whereClause} 
                OFFSET {(page - 1) * page_size} ROWS FETCH NEXT {page_size} ROWS ONLY";

                _connection.Open();
                result_abstract = _connection.Query<dynamic>(query, new { Text = $"%{text}%" });
                if (result_abstract.Any())
                {
                    total = result_abstract.First().total ?? 0;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                _connection.Close();
            }
            var result = result_abstract.Select(x => (IDictionary<string, object>)x).ToList();
            var result_entity = ConvertToList<T>(result);
            return (result_entity, total);
        }


        public T GetById(int Id)
        {
            IEnumerable<T> result = null;
            try
            {
                string tableName = GetTableName();
                string keyColumn = GetKeyColumnName();
                string query = $"SELECT * FROM  {schema}.{tableName} WHERE {keyColumn} = '{Id}'";
                _connection.Open();
                result = _connection.Query<T>(query);
            }
            catch (Exception ex) { }

            return result.FirstOrDefault();
        }

        public bool Update(T entity)
        {
            int rowsEffected = 0;
            try
            {
                string tableName = GetTableName();
                string keyColumn = GetKeyColumnName();
                string keyProperty = GetPropertyValue(entity, keyColumn)?.ToString() ?? string.Empty;

                StringBuilder query = new StringBuilder();
                query.Append($"UPDATE  {schema}.{tableName} SET ");

                foreach (var property in GetProperties(true))
                {
                    var columnAttr = property.GetCustomAttribute<ColumnAttribute>();

                    string propertyName = property.Name;
                    string propertyValue = GetPropertyValue(entity, propertyName)?.ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(propertyValue))
                        query.Append($"{propertyName} = '{propertyValue}',");
                }

                query.Remove(query.Length - 1, 1);

                query.Append($" WHERE {keyColumn} = '{keyProperty}'");

                rowsEffected = _connection.Execute(query.ToString(), entity);
            }
            catch (Exception ex) { }

            return rowsEffected > 0 ? true : false;
        }



        public string GetTableName()
        {
            string tableName = "";
            var type = typeof(T);
            var tableAttr = type.GetCustomAttribute<TableAttribute>();
            if (tableAttr != null)
            {
                tableName = tableAttr.Name;
                return tableName;
            }

            return type.Name;
        }

        public static string GetKeyColumnName()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object[] keyAttributes = property.GetCustomAttributes(typeof(KeyAttribute), true);

                if (keyAttributes != null && keyAttributes.Length > 0)
                {
                    object[] columnAttributes = property.GetCustomAttributes(typeof(ColumnAttribute), true);

                    if (columnAttributes != null && columnAttributes.Length > 0)
                    {
                        ColumnAttribute columnAttribute = (ColumnAttribute)columnAttributes[0];
                        return columnAttribute.Name;
                    }
                    else
                    {
                        return property.Name;
                    }
                }
            }

            return null;
        }


        public string GetColumns(bool excludeKey = false)
        {
            var type = typeof(T);

            var columns = string.Join(", ", type.GetProperties()
                .Where(p => excludeKey && !p.Name.Equals("Id"))
                .Select(p =>
                {
                    var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
                    return columnAttr != null ? columnAttr.Name : p.Name;
                }));

            return columns;
        }

        protected string GetPropertyNames(bool excludeKey = false)
        {
            var properties = typeof(T).GetProperties()
                .Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);
            if (excludeKey)
                properties = properties.Where(x => !x.Name.Equals("Id"));

            var values = string.Join(", ", properties.Select(p =>
            {
                return $"@{p.Name}";
            }));

            return values;
        }

        protected string GetPropertyValues(T entity, bool excludeKey = false)
        {
            var properties = typeof(T).GetProperties()
                .Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);
            if (excludeKey)
                properties = properties.Where(x => !x.Name.Equals("Id"));

            var values = string.Join(", ", properties.Select(p =>
            {
                var value = p.GetValue(entity);
                return value != null ? $"'{value.ToString()}'" : "NULL";
            }));

            return values;
        }
        public static object GetPropertyValue<T>(T entity, string propertyName)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            Type type = typeof(T);
            PropertyInfo propertyInfo = type.GetProperty(propertyName);

            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type '{type.Name}'.");
            }

            return propertyInfo.GetValue(entity);
        }

        protected IEnumerable<PropertyInfo> GetProperties(bool excludeKey = false)
        {
            var properties = typeof(T).GetProperties()
                .Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);

            return properties;
        }

        protected string GetKeyPropertyName()
        {
            var properties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<KeyAttribute>() != null);

            if (properties.Any())
            {
                return properties.FirstOrDefault().Name;
            }

            return null;
        }
        public IList<string> GetValidationMessage(IBaseVM viewModel)
        {
            try
            {
                var validationContext = new ValidationContext(viewModel, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(viewModel, validationContext, validationResults, true);
                var messages = new List<string>();
                // If there any exception return them in the return result
                if (!isValid)
                {
                    OperationStatus opStatus = new OperationStatus();

                    foreach (ValidationResult message in validationResults)
                    {
                        messages.Add(message.ErrorMessage);
                    }

                    return messages;
                }
                return messages;
            }
            catch (Exception ex)
            {
                return new List<string>() { $"Error: {ex.Message}" };
            }

        }

        public static List<T> ConvertToList<T>(List<IDictionary<string, object>> dictionaries)
        {
            var resultList = new List<T>();

            foreach (var dictionary in dictionaries)
            {
                var obj = Activator.CreateInstance<T>();

                foreach (var property in typeof(T).GetProperties())
                {
                    if (dictionary.ContainsKey(property.Name))
                    {
                        var value = dictionary[property.Name];

                        if (value != null && property.CanWrite)
                        {
                            if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                            {
                                property.SetValue(obj, value == null ? null : Convert.ChangeType(value, Nullable.GetUnderlyingType(property.PropertyType)));
                            }
                            else
                            {
                                if (property.PropertyType.IsEnum)
                                {
                                    var enumValue = Enum.TryParse(property.PropertyType, value.ToString(), out var parsedEnumValue);
                                    if (enumValue)
                                    {
                                        property.SetValue(obj, parsedEnumValue);
                                    }
                                    else
                                    {
                                        property.SetValue(obj, null);
                                    }
                                }
                                else
                                {
                                    property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                                }
                            }
                        }
                    }
                }

                resultList.Add(obj);
            }

            return resultList;
        }
    }
}
