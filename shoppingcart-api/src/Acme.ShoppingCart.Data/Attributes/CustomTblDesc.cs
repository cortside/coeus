using System;

namespace Acme.ShoppingCart.Data.Attributes {
    public class CustomTblDesc : Attribute {
        public CustomTblDesc(string description) {
            Description = description;
        }

        public string Description { get; }
    }
}
