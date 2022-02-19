using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using EFCore.Seeder.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Acme.ShoppingCart.WebApi.IntegrationTests.Helpers {
    public static class DbSetExtensions {
        /// <summary>
        /// Seeds a DBSet from a CSV file that will be read from the specified stream
        /// </summary>
        /// <typeparam name="T">The type of entity to load</typeparam>
        /// <param name="dbSet">The DbSet to populate</param>
        /// <param name="stream">The stream containing the CSV file contents</param>
        /// <param name="additionalMapping">Any additonal complex mappings required</param>
        public static async Task<List<T>> SeedFromFileAsync<T>(this DbSet<T> dbSet, string filename, params CsvColumnMapping<T>[] additionalMapping) where T : class {
            var data = DataFromFile(filename, additionalMapping);
            await dbSet.AddRangeAsync(data).ConfigureAwait(false);
            return data;
        }

        public static List<T> DataFromFile<T>(string filename, params CsvColumnMapping<T>[] additionalMapping) where T : class {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            filename = Path.Combine(path, filename);

            if (!File.Exists(filename)) {
                throw new FileNotFoundException($"{filename} not found", filename);
            }

            try {
                using var reader = new StreamReader(filename);
                var results = new List<T>();

                var config = new CsvConfiguration(CultureInfo.InvariantCulture) {
                    IgnoreReferences = true,
                    IgnoreBlankLines = true,
                    HeaderValidated = null,
                    MissingFieldFound = null
                };
                var csvReader = new CsvReader(reader, config);
                var map = csvReader.Configuration.AutoMap<T>();
                map.ReferenceMaps.Clear();
                csvReader.Configuration.RegisterClassMap(map);

                while (csvReader.Read()) {
                    var csvEntity = csvReader.GetRecord<T>();
                    foreach (var csvColumnMapping in additionalMapping) {
                        csvColumnMapping.Execute(csvEntity, csvReader.GetField(csvColumnMapping.CsvColumnName));
                    }

                    results.Add(csvEntity);
                }

                return results;
            } catch (Exception exception) {
                var message = $"Error Seeding DbSet<{typeof(T).Name}>: {exception}";
                throw new Exception(message, exception);
            }
        }
    }
}
