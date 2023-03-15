using System;

namespace Acme.ShoppingCart.Data.Attributes {
    public class CustomColDesc : Attribute {
        public CustomColDesc(string description) {
            Description = description;
        }

        public CustomColDesc(string description, string tableName) {
            Description = description;
            LinkedTable = tableName;
        }

        public CustomColDesc(string description, string tableName, string linkedTablePropertyName) {
            Description = description;
            LinkedTable = tableName;
            LinkedTablePropertyName = linkedTablePropertyName;
        }

        public string Description { get; }

        public string LinkedTable { get; } = "";

        public string LinkedTablePropertyName { get; } = "";
    }
}
