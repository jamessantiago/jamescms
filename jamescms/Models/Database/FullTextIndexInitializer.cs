using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace jamescms
{
    public class FullTextIndexInitializer<T> : IDatabaseInitializer<T> where T : DbContext
    {
        private const string CreateFullTextCatalogQueryTemplate = "CREATE FULLTEXT CATALOG {indexName}";
        private const string CreateFullTextIndexQueryTemplate = "CREATE FULLTEXT INDEX ON {tableName}({columnName}) KEY INDEX {primaryKey} ON {indexName}";

        public void InitializeDatabase(T context)
        {
            const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;

            foreach (var dataSetProperty in typeof(T).GetProperties(PublicInstance).Where(
                p => p.PropertyType.Name == typeof(DbSet<>).Name))
            {
                var entityType = dataSetProperty.PropertyType.GetGenericArguments().Single();

                TableAttribute[] tableAttributes = (TableAttribute[])entityType.GetCustomAttributes(typeof(TableAttribute), false);

                foreach (var property in entityType.GetProperties(PublicInstance))
                {
                    IndexAttribute[] indexAttributes = (IndexAttribute[])property.GetCustomAttributes(typeof(IndexAttribute), false);
                    NotMappedAttribute[] notMappedAttributes = (NotMappedAttribute[])property.GetCustomAttributes(typeof(NotMappedAttribute), false);
                    if (indexAttributes.Length > 0 && notMappedAttributes.Length == 0)
                    {
                        ColumnAttribute[] columnAttributes = (ColumnAttribute[])property.GetCustomAttributes(typeof(ColumnAttribute), false);

                        foreach (var indexAttribute in indexAttributes)
                        {
                            string indexName = indexAttribute.Name;
                            string tableName = tableAttributes.Length != 0 ? tableAttributes[0].Name : dataSetProperty.Name;
                            string columnName = columnAttributes.Length != 0 ? columnAttributes[0].Name : property.Name;
                            string primaryKey = columnAttributes.Length != 0 ? columnAttributes[0].Name : property.Name;
                            string indexQuery = CreateFullTextIndexQueryTemplate.Replace("{indexName}", indexName)
                                .Replace("{tableName}", tableName)
                                .Replace("{columnName}", columnName)
                                .Replace("{primaryKey}", primaryKey);

                            context.Database.CreateIfNotExists();

                            context.Database.ExecuteSqlCommand(query);
                        }
                    }
                }
            }
        }
    }
}