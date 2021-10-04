using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp.CommandHandlers
{
    internal static class CommandHandlerExtensions
    {
        internal static Dictionary<string, string> ExtractKeyValuePair(string source, string[] separator)
        {
            const int KeyIndex = 0;
            const int ValuesIndex = 1;
            var result = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            var parameterPairs = source.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var parameterValuePair = parameterPairs.Select(x => x.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            foreach (var pair in parameterValuePair)
            {
                result.Add(pair[KeyIndex], pair[ValuesIndex].Trim('\u0027'));
            }

            return result;
        }

        internal static Predicate<FileCabinetRecord> GeneratePredicate(Dictionary<string, string> keyValuePair)
        {
            if (keyValuePair is null)
            {
                return _ => true;
            }

            Predicate<FileCabinetRecord> intermediateResult = null;

            foreach (var pair in keyValuePair)
            {
                var property =
                    typeof(FileCabinetRecord).GetProperties().FirstOrDefault(property => property.Name.Contains(pair.Key, StringComparison.CurrentCultureIgnoreCase));
                intermediateResult += record => property!.GetValue(record)?.ToString() == pair.Value;
            }

            return intermediateResult!.GetInvocationList().Length > 0 ? CombinePredicateIntoOneMethod(intermediateResult) : intermediateResult;

            static Predicate<FileCabinetRecord> CombinePredicateIntoOneMethod(Predicate<FileCabinetRecord> predicate)
            {
                return record =>
                {
                    foreach (var method in predicate.GetInvocationList())
                    {
                        if (((Predicate<FileCabinetRecord>)method).Invoke(record) is false)
                        {
                            return false;
                        }
                    }

                    return true;
                };
            }
        }
    }
}
