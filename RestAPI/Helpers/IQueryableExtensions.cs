using RestAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace RestAPI.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy,
            Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (mappingDictionary == null) throw new ArgumentNullException(nameof(mappingDictionary));

            if (string.IsNullOrWhiteSpace(orderBy)) return source;

            var orderByString = string.Empty;

            var orderBySplit = orderBy.Split(",");

            foreach (var orderByClause in orderBySplit)
            {
                var trimmedOrderBy = orderByClause.Trim();
                var orderDescending = trimmedOrderBy.EndsWith(" desc");

                var indexOfFirstSpace = trimmedOrderBy.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedOrderBy : trimmedOrderBy.Remove(indexOfFirstSpace);

                if (!mappingDictionary.ContainsKey(propertyName))
                    throw new ArgumentException($"Key mapping for {propertyName} is missing");

                var propertyMappingValue = mappingDictionary[propertyName];

                if (propertyMappingValue == null) throw new ArgumentNullException(nameof(propertyMappingValue));

                foreach (var destinationProperty in propertyMappingValue.DestinationProperties)
                {
                    if (propertyMappingValue.Revert) orderDescending = !orderDescending;

                    orderByString += (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ")
                                     + destinationProperty
                                     + (orderDescending ? " descending" : " ascending");
                }
            }

            return source.OrderBy(orderByString);
        }
    }
}
