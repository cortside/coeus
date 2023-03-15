using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Newtonsoft.Json;

namespace Acme.ShoppingCart.Data.Generators {


    /// <summary>Entity configuration across an Assignable Type</summary>
    public interface IGlobalEntityConfiguration {
        void OnModelCreating(ModelBuilder builder);
    }

    public interface IAuditableExecutedByUserId {
        DateTime ExecutedDateTime { get; set; }
    }

    /// <summary>A common entity class</summary>
    public interface IAuditableStandardUserName {
        string CreatedByUserName { get; set; }

        DateTime CreatedDateTime { get; set; }

        string UpdatedByUserName { get; set; }

        DateTime? UpdatedDateTime { get; set; }
    }

    /// <summary>Entity configuration across an Assignable Type</summary>
    public class AuditableExecutedByUserIdGlobalConfiguration : IGlobalEntityConfiguration {
        public virtual void OnModelCreating(ModelBuilder builder) {
            foreach (var entityType in builder.Model.GetEntityTypes()) {
                if (typeof(IAuditableExecutedByUserId).IsAssignableFrom(entityType.ClrType)) {
                    // PROPERTIES
                    var executedDateTime = builder.Entity(entityType.ClrType)
                                                  .Metadata
                                                  .FindProperty("ExecutedDateTime");

                    // Defaults
                    if (executedDateTime != null) {
                        executedDateTime.SetDefaultValueSql("GETUTCDATE()");
                    }
                }
            }
        }
    }

    /// <summary>Entity configuration across an Assignable Type</summary>
    public class AuditableStandardUserNameGlobalConfiguration : IGlobalEntityConfiguration {
        public virtual void OnModelCreating(ModelBuilder builder) {
            foreach (var entityType in builder.Model.GetEntityTypes()) {
                if (typeof(IAuditableStandardUserName).IsAssignableFrom(entityType.ClrType)) {
                    // PROPERTIES
                    var createdDateTime = builder.Entity(entityType.ClrType)
                                                 .Metadata
                                                 .FindProperty("CreatedDateTime");

                    var createdByUserName = builder.Entity(entityType.ClrType)
                                                 .Metadata
                                                 .FindProperty("CreatedByUserName");

                    var updatedDateTime = builder.Entity(entityType.ClrType)
                                                 .Metadata
                                                 .FindProperty("UpdatedDateTime");

                    var updatedByUserName = builder.Entity(entityType.ClrType)
                                                 .Metadata
                                                 .FindProperty("UpdatedByUserName");

                    // Defaults
                    if (createdDateTime != null)
                        createdDateTime.SetDefaultValueSql("GETUTCDATE()");

                    if (createdByUserName != null)
                        createdByUserName.SetMaxLength(50);

                    if (updatedDateTime != null)
                        updatedDateTime.SetDefaultValueSql("GETUTCDATE()");

                    if (updatedByUserName != null)
                        updatedByUserName.SetMaxLength(50);
                }
            }
        }
    }

    //// This is in your Concrete DbContext class
    //protected override void OnModelCreating(ModelBuilder builder) {
    //    base.OnModelCreating(builder);

    //    // GLOBAL Configurations
    //    builder.ApplyGlobalEntityConfiguration(new AuditableStandardUserNameGlobalConfiguration());

    //    // ENTITY Configurations
    //    builder.ApplyConfiguration(new ContextTypeConfiguration());
    //    builder.ApplyConfiguration(new ObjectStateConfiguration());
    //    builder.ApplyConfiguration(new ObjectStateEventConfiguration());
    //    builder.ApplyConfiguration(new WorkflowConfiguration());
    //    builder.ApplyConfiguration(new WorkflowEventConfiguration());
    //    builder.ApplyConfiguration(new WorkflowTransitionConfiguration());
    //}

    public class DescriptionMigrationSqlGenerator : SqlServerMigrationsSqlGenerator {
        public DescriptionMigrationSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, IRelationalAnnotationProvider migrationsAnnotations) : base(dependencies, migrationsAnnotations) {
        }

        protected override void Generate(MigrationOperation operation, IModel model, MigrationCommandListBuilder builder) {
            Console.WriteLine($"\r\n---\r\nDescriptionMigrationSqlGenerator with operation {operation.GetType()} and {operation.GetAnnotations().Count()} operations was used\r\n---");

            if (operation is CreateTableOperation createUserOperation) {
                GenerateCreateTable(createUserOperation, model, builder);
            } else {
                base.Generate(operation, model, builder);
            }
        }

        private void GenerateCreateTable(CreateTableOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate = true) {
            Console.WriteLine($"\r\n---\r\n{operation.Name}\r\n---");
            Console.WriteLine(operation.GetAnnotations().Count());
            foreach (var a in operation.GetAnnotations()) {
                Console.WriteLine(a.Name);
            }

            base.Generate(operation, model, builder, terminate);

            var descriptionData = operation.FindAnnotation("Descriptions");
            if (descriptionData == null) {
                Console.WriteLine("descriptions not found");
                return;
            }

            try {
                var descriptionValues = JsonConvert.DeserializeObject<KeyValuePair<string, string>[]>(descriptionData.Value?.ToString());

                foreach (var descriptionValue in descriptionValues) {
                    Console.WriteLine(descriptionValue.Key);
                    Console.WriteLine(descriptionValue.Value);

                    var column = operation.Columns.SingleOrDefault(x => string.Equals(x.Name, descriptionValue.Key, StringComparison.CurrentCultureIgnoreCase));
                    if (column == null) {
                        Console.WriteLine("column not found");
                        continue;
                    }

                    var tableNameParts = operation.Name.Split('.');
                    var schemaName = tableNameParts.First();
                    var tableName = tableNameParts.Last();

                    builder.Append("EXEC sp_addextendedproperty @name=N'MS_Description', @value = ");
                    builder.Append($"N'{descriptionValue.Value}', @level0type=N'Schema', @level0name= {schemaName}, ");
                    builder.AppendLine($"@level1type=N'Table', @level1name={tableName}, @level2type=N'Column', @level2name={column.Name}");
                    builder.EndCommand();
                }
            } catch {
                // TODO: Handle situation where invalid description annotation provided. (I.e. not provided collection of key value pairs...
            }
        }
    }

    //internal sealed class Configuration : DbMigrationsConfiguration<DatabaseContext> {
    //    public Configuration() {
    //        AutomaticMigrationsEnabled = true;
    //        SetSqlGenerator("System.Data.SqlClient", new DescriptionMigrationSqlGenerator());
    //    }
    //}

    //private void SetTableDescriptions(Type tableType) {
    //    string tableName = tableType.Name;

    //    // -- Set table description

    //    CustomTblDesc tblDesc = null;

    //    var custDescs = tableType.GetCustomAttributes(typeof(CustomTblDesc), false);
    //    if (custDescs != null && custDescs.Length > 0) {
    //        tblDesc = custDescs[0] as CustomTblDesc;
    //        SetTableDescription(tableName, tblDesc.Description);
    //    }

    //    // -- Set column description

    //    foreach (var prop in tableType.GetProperties(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)) {
    //        if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
    //            continue;
    //        else {
    //            CustomColDesc colDesc = null;

    //            custDescs = prop.GetCustomAttributes(typeof(CustomColDesc), false);
    //            if (custDescs != null && custDescs.Length > 0) {
    //                colDesc = custDescs[0] as CustomColDesc;

    //                if (string.IsNullOrEmpty(colDesc.LinkedTable))
    //                    SetColumnDescription(tableName, prop.Name, colDesc.Description);
    //                else
    //                    SetColumnDescription(colDesc.LinkedTable, colDesc.LinkedTablePropertyName, colDesc.Description);
    //            }
    //        }
    //    }
    //}
}
