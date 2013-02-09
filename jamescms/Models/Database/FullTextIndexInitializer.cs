using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace jamescms
{
    public class FullTextIndexInitializer<T> : IDatabaseInitializer<T> where T : DbContext
    {
        private const string FullTextQuery = "SELECT name FROM sys.fulltext_catalogs";
        private const string CreateFullTextCatalogQueryTemplate = "CREATE FULLTEXT CATALOG {catalogName}";
        private const string CreateFullTextIndexQueryTemplate = "CREATE FULLTEXT INDEX ON {tableName}({columnName}) KEY INDEX {primaryKey} ON {catalogName}";

        public void InitializeDatabase(T context)
        {
            const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;

            foreach (var dataSetProperty in typeof(T).GetProperties(PublicInstance).Where(
                p => p.PropertyType.Name == typeof(IDbSet<>).Name))
            {
                var entityType = dataSetProperty.PropertyType.GetGenericArguments().Single();

                TableAttribute[] tableAttributes = (TableAttribute[])entityType.GetCustomAttributes(typeof(TableAttribute), false);
                FullTextIndexAttribute[] indexAttributes = (FullTextIndexAttribute[])entityType.GetCustomAttributes(typeof(FullTextIndexAttribute), false);

                NotMappedAttribute[] notMappedAttributes = (NotMappedAttribute[])entityType.GetCustomAttributes(typeof(NotMappedAttribute), false);
                if (indexAttributes.Length > 0 && notMappedAttributes.Length == 0)
                {
                    ColumnAttribute[] columnAttributes = (ColumnAttribute[])dataSetProperty.GetCustomAttributes(typeof(ColumnAttribute), false);

                    foreach (var indexAttribute in indexAttributes)
                    {
                        string catalogName = indexAttribute.Name;
                        string tableName = tableAttributes.Length != 0 ? tableAttributes[0].Name : dataSetProperty.Name;
                        string columnName = columnAttributes.Length != 0 ? columnAttributes[0].Name : dataSetProperty.Name;
                        string catalogQuery = CreateFullTextCatalogQueryTemplate.Replace("{catalogName}", catalogName);
                        string indexQuery = CreateFullTextIndexQueryTemplate.Replace("{catalogName}", catalogName)
                            .Replace("{tableName}", tableName)
                            .Replace("{columnName}", columnName)
                            .Replace("{primaryKey}", "PK_DBO." + tableName);

                        context.Database.CreateIfNotExists();
                        bool indexExists = false;
                        var results = context.Database.SqlQuery(typeof(string), FullTextQuery);
                        foreach (string result in results)
                        {
                            if (result == catalogName)
                                indexExists = true;
                        }
                        if (!indexExists)
                        {
                            context.Database.ExecuteSqlCommand(catalogQuery);
                            context.Database.ExecuteSqlCommand(indexQuery);
                        }
                    }
                    
                }
            }
        }
    }
}