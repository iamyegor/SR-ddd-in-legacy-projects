using ACL.Extensions;

namespace ACL.Utils;

public class PostgreSqlGenerator
{
    public string InsertOrUpdate<T>(string tableName, string tablePrimaryKey)
    {
        string[] propertyNames = typeof(T).GetProperties().Select(p => p.Name).ToArray();

        string columns = string.Join(',', propertyNames.Select(p => p.ToSnakeCase()));
        string values = string.Join(',', propertyNames.Select(p => "@" + p));
        string onConflictUpdate = string.Join(',', columns.Select(c => $"{c} = excluded.{c}"));

        string query =
            @$"
            insert into {tableName} ({columns}) values ({values})
            on conflict ({tablePrimaryKey}) do update set {onConflictUpdate}";

        return query;
    }
}
