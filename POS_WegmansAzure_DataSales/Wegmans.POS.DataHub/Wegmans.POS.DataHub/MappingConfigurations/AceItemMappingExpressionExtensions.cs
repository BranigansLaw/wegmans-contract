using AutoMapper;
using Wegmans.POS.DataHub.JSONOutputModel.Transaction.v1;
using System;

namespace Wegmans.POS.DataHub.MappingConfigurations
{
    internal static class AceItemMappingExpressionExtensions
    {
        internal static IMappingExpression<T, AceItem> EnsureOnlyOnce<T>(this IMappingExpression<T, AceItem> config)
            => config.BeforeMap((_, item) =>
            {
                var value = typeof(T).Name;
                if (item.ProcessedStrings.Contains(value))
                {
                    throw new InvalidOperationException($"The {value} has already been processed on the item '{item.UniversalProductCode}'.");
                }
            })
            .AfterMap((_, item) => item.ProcessedStrings.Add(typeof(T).Name));
    }
}