using System.Linq.Expressions;
using System.Reflection;

namespace game_x.application.Common.Filters;

public sealed class CriteriaBuilder<TModel> : ICriteriaBuilder<TModel>
{
    // Map of allowed operators for each primitive type
    private static readonly Dictionary<Type, string[]> OperatorMap = new()
    {
        { typeof(string), ["eq", "ne", "c", "sw", "ew"] },
        { typeof(int), ["eq", "ne", "gt", "gte", "lt", "lte"] },
        { typeof(decimal), ["eq", "ne", "gt", "gte", "lt", "lte"] },
        { typeof(double), ["eq", "ne", "gt", "gte", "lt", "lte"] },
        { typeof(long), ["eq", "ne", "gt", "gte", "lt", "lte"] },
        { typeof(float), ["eq", "ne", "gt", "gte", "lt", "lte"] },
        { typeof(DateTime), ["eq", "ne", "gt", "gte", "lt", "lte"] },
        { typeof(bool), ["eq", "ne"] },
        { typeof(Guid), ["eq", "ne"] },
        { typeof(Enum), ["eq", "ne"] }
    };

    private static readonly string[] AllowedSortDirections = new[] { "asc", "desc" };

    /// <summary>
    ///     Applies filtering and sorting to the given IQueryable based on filters and sorts.
    /// </summary>
    public IQueryable<TModel> Apply(
        IQueryable<TModel> query,
        IEnumerable<QueryFilter>? filters = null,
        IEnumerable<QuerySort>? sorts = null,
        Func<string, Expression<Func<TModel, bool>>>? searchByKeyCondition = null,
        Dictionary<string, Func<object, Expression<Func<TModel, bool>>>>? options = null)
    {
        if (filters != null && filters.Any())
            foreach (var filter in filters)
            {
                // Handle search by key word
                if (filter.Field == "search" && searchByKeyCondition != null)
                {
                    query = query.Where(searchByKeyCondition(filter.Value));
                    continue;
                }

                // Handle search by custom query
                if (options is not null
                    && options.TryGetValue(filter.Field, out var callback))
                    try
                    {
                        query = query.Where(callback(filter.Value));
                        continue;
                    }
                    catch
                    {
                        throw new ArgumentException($"{filter.Field} invalid filter.");
                    }

                // Apply filter
                query = ApplyFilter(query, filter);
            }

        if (sorts != null && sorts.Any())
        {
            var isFirstSort = true;
            foreach (var sort in sorts)
            {
                query = ApplySort(query, sort, isFirstSort);
                isFirstSort = false;
            }
        }

        return query;
    }

    /// <summary>
    ///     Applies a single filter to the query.
    /// </summary>
    private IQueryable<TModel> ApplyFilter(IQueryable<TModel> query, QueryFilter filter)
    {
        if (string.IsNullOrWhiteSpace(filter.Value))
            return query;

        var propertyInfo = typeof(TModel).GetProperty(
            filter.Field,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo == null)
            return query;

        var parameter = Expression.Parameter(typeof(TModel), "x");
        var property = Expression.Property(parameter, propertyInfo);
        var propertyType = property.Type;

        if (!IsValidOperator(filter.Operator, propertyType))
            throw new NotSupportedException($"Operator '{filter.Operator}' is not valid for type {propertyType.Name}");

        if (!TryParseValue(filter.Value, propertyType, out var parsedValue))
            throw new NotSupportedException($"Cannot parse filter value '{filter.Value}' to type {propertyType.Name}");

        var comparisonExpr = BuildComparisonExpression(property, parsedValue!, filter.Operator);
        var lambda = Expression.Lambda<Func<TModel, bool>>(comparisonExpr, parameter);

        return query.Where(lambda);
    }

    /// <summary>
    ///     Applies sorting to the query.
    /// </summary>
    private IQueryable<TModel> ApplySort(IQueryable<TModel> query, QuerySort sort, bool isFirstSort)
    {
        if (!AllowedSortDirections.Contains(sort.Direction.ToLower()))
            throw new NotSupportedException($"Sort direction '{sort.Direction}' is not supported.");

        var propertyInfo = typeof(TModel).GetProperty(
            sort.Field,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo == null)
            return query;

        var parameter = Expression.Parameter(typeof(TModel), "x");
        var property = Expression.Property(parameter, propertyInfo);
        var lambda = Expression.Lambda(property, parameter);

        string methodName;
        var descending = sort.Direction.Equals("desc", StringComparison.OrdinalIgnoreCase);

        if (isFirstSort)
            methodName = descending ? "OrderByDescending" : "OrderBy";
        else
            methodName = descending ? "ThenByDescending" : "ThenBy";

        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(TModel), property.Type],
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<TModel>(resultExpression);
    }

    /// <summary>
    ///     Checks if the operator is valid for the target type.
    /// </summary>
    private bool IsValidOperator(string op, Type targetType)
    {
        // Unwrap Nullable<T>
        var type = Nullable.GetUnderlyingType(targetType) ?? targetType;

        // If this is a Value Object (has "Value" property), unwrap its Value type
        var valueProp = type.GetProperty("Value");
        if (valueProp != null)
        {
            var innerType = Nullable.GetUnderlyingType(valueProp.PropertyType) ?? valueProp.PropertyType;
            return OperatorMap.TryGetValue(innerType, out var ops) && ops.Contains(op.ToLower());
        }

        if (type.BaseType == typeof(Enum))
            return OperatorMap.TryGetValue(typeof(Enum), out var validEnumOps) && validEnumOps.Contains(op.ToLower());
        
        // Otherwise, normal primitive type check
        return OperatorMap.TryGetValue(type, out var validOps) && validOps.Contains(op.ToLower());
    }

    /// <summary>
    ///     Builds the comparison expression based on operator.
    /// </summary>
    private Expression BuildComparisonExpression(Expression property, object value, string op)
    {
        var constant = Expression.Constant(value, property.Type);

        return op.ToLower() switch
        {
            "eq" => Expression.Equal(property, constant),
            "ne" => Expression.NotEqual(property, constant),
            "gt" => Expression.GreaterThan(property, constant),
            "gte" => Expression.GreaterThanOrEqual(property, constant),
            "lt" => Expression.LessThan(property, constant),
            "lte" => Expression.LessThanOrEqual(property, constant),
            "c" => Expression.Call(property, nameof(string.Contains), Type.EmptyTypes, constant),
            "sw" => Expression.Call(property, nameof(string.StartsWith), Type.EmptyTypes, constant),
            "ew" => Expression.Call(property, nameof(string.EndsWith), Type.EmptyTypes, constant),
            _ => throw new NotSupportedException($"Unsupported operator: '{op}'")
        };
    }

    /// <summary>
    ///     Attempts to parse the string value into the target type.
    ///     Supports primitive types and Value Objects with static Of method and Value property.
    /// </summary>
    private bool TryParseValue(string value, Type targetType, out object? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        // Detect if targetType is a Value Object with a Value property
        var valueProp = targetType.GetProperty("Value");
        if (valueProp != null)
        {
            var underlyingValueType = Nullable.GetUnderlyingType(valueProp.PropertyType) ?? valueProp.PropertyType;

            if (!TryParsePrimitiveValue(value, underlyingValueType, out var primitiveValue))
                return false;

            // Look for static Of method that accepts underlyingValueType parameter
            var ofMethod = targetType.GetMethod("Of", BindingFlags.Public | BindingFlags.Static, null,
                new[] { underlyingValueType }, null);
            if (ofMethod == null)
                throw new NotSupportedException(
                    $"Type {targetType.Name} must have a static Of({underlyingValueType.Name}) method.");

            try
            {
                result = ofMethod.Invoke(null, new[] { primitiveValue! });
                return result != null;
            }
            catch
            {
                return false;
            }
        }

        // Not a Value Object: parse primitive normally
        return TryParsePrimitiveValue(value, targetType, out result);
    }

    /// <summary>
    ///     Attempts to parse primitive CLR types from string.
    /// </summary>
    private bool TryParsePrimitiveValue(string value, Type targetType, out object? result)
    {
        result = null;

        try
        {
            if (targetType == typeof(string))
            {
                result = value;
                return true;
            }

            if (targetType == typeof(int))
                return int.TryParse(value, out var intVal) ? (result = intVal) != null : false;
            if (targetType == typeof(decimal))
                return decimal.TryParse(value, out var decVal) ? (result = decVal) != null : false;
            if (targetType == typeof(double))
                return double.TryParse(value, out var dblVal) ? (result = dblVal) != null : false;
            if (targetType == typeof(long))
                return long.TryParse(value, out var longVal) ? (result = longVal) != null : false;
            if (targetType == typeof(float))
                return float.TryParse(value, out var fltVal) ? (result = fltVal) != null : false;
            if (targetType == typeof(DateTime))
                return DateTime.TryParse(value, out var dtVal)
                    ? (result = DateTime.SpecifyKind(dtVal, DateTimeKind.Utc)) != null
                    : false;
            if (targetType == typeof(bool))
                return bool.TryParse(value, out var boolVal) ? (result = boolVal) != null : false;
            if (targetType == typeof(Guid))
                return Guid.TryParse(value, out var guidVal) ? (result = guidVal) != null : false;

            if (targetType.IsEnum)
                return Enum.TryParse(targetType, value, true, out var enumVal) && (result = enumVal) != null;

            // Fallback for convertible types
            result = Convert.ChangeType(value, targetType);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }
}