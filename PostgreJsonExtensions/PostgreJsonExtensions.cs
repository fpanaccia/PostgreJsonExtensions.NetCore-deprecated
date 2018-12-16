using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace PostgreJsonExtensions
{
    public static class PostgreJsonExtensions
    {
        private static readonly Dictionary<ExpressionType, string> ops = new Dictionary<ExpressionType, string>
        {
            { ExpressionType.Equal, "=" },
            { ExpressionType.NotEqual, "<>" },
            { ExpressionType.GreaterThanOrEqual, ">=" },
            { ExpressionType.GreaterThan, ">" },
            { ExpressionType.LessThan, "<" },
            { ExpressionType.LessThanOrEqual, "<=" },
            { ExpressionType.Or, "or" },
            { ExpressionType.OrElse, "or" },
            { ExpressionType.And, "and" },
            { ExpressionType.AndAlso, "and" }
        };

        private static readonly List<ExpressionType> conditions = new List<ExpressionType>()
        {
            ExpressionType.Or,
            ExpressionType.OrElse,
            ExpressionType.And,
            ExpressionType.AndAlso
        };

        public static IQueryable<TEntity> JsonWhere<TEntity, TJsonObj>(this IQueryable<TEntity> source, string jsonColumnName, Expression<Func<TJsonObj, bool>> predicate) where TEntity : class
        {
            var sql = source.ToSql();
            var expression = (LambdaExpression)predicate;
            var part = (BinaryExpression)expression.Body;
            var parameter = expression.Parameters[0].Name;

            if (part != null)
            {
                sql = sql + " where ";
            }

            var sqlBuild = new List<string>();

            ParseTree(part, parameter, jsonColumnName, sqlBuild);

            var cond = conditions.Select(x => ops[x]).Distinct();
            var last = sqlBuild.Last().Split(" ");

            if (last.Any(y => cond.Contains(y)))
            {
                last = last.Where(x => !cond.Contains(x)).ToArray();
                sqlBuild.RemoveAt(sqlBuild.Count - 1);
                sqlBuild.Add(string.Join(" ", last));
            }

#pragma warning disable EF1000 // Possible SQL injection vulnerability.
            return source.FromSql(sql + string.Join(" ", sqlBuild));
#pragma warning restore EF1000 // Possible SQL injection vulnerability.
        }

        public static void ParseTree(dynamic part, string parameter, string jsonColumnName, List<string> sql, ExpressionType? cond = null)
        {
            if (conditions.Contains(part.NodeType))
            {
                ParseTree(part.Left, parameter, jsonColumnName, sql, part.NodeType);
                ParseTree(part.Right, parameter, jsonColumnName, sql, part.NodeType);
            }
            else
            {
                var str = string.Empty;
                string[] columnArray = null;
                dynamic value = null;
                var operation = string.Empty;
                if (part is BinaryExpression)
                {
                    operation = ops[part.NodeType];
                    columnArray = part.Left.ToString().Split('.');
                    value = Expression.Lambda(part.Right).Compile().DynamicInvoke();
                }
                else
                {
                    if (part.Type.Name == "Boolean")
                    {
                        bool IsTrue = true;
                        var regex = new Regex("^Not\\(.*\\)", RegexOptions.IgnoreCase);
                        string prop = part.ToString();
                        if (regex.Match(prop).Success)
                        {
                            prop = prop.Remove(prop.Length - 1).Replace("Not(", "");
                            IsTrue = false;
                        }
                        operation = ops[ExpressionType.Equal];
                        value = IsTrue;
                        columnArray = prop.Split('.');
                    }
                }

                var column = columnArray[0].Replace(parameter, jsonColumnName);
                if (columnArray.Count() > 2)
                {
                    for (var i = 1; i < columnArray.Count(); i++)
                    {
                        column += $"{(columnArray.Count() - 1 == i ? "->>" : "->")} '{columnArray[i]}'";
                    }
                }
                else
                {
                    column += $" ->> '{columnArray[1]}'";

                }

                str = $"{column} {operation} '{JsonConvert.SerializeObject(value).Replace("\"", "")}'";

                if (cond.HasValue)
                {
                    str += $" {ops[cond.Value]} ";
                }

                sql.Add(str);
            }
        }
    }
}
