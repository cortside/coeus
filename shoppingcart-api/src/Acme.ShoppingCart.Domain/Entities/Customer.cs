using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Cortside.AspNetCore.Auditable.Entities;
using Cortside.Common.Messages;
using Cortside.Common.Messages.MessageExceptions;

namespace Acme.ShoppingCart.Domain.Entities {
    [Table("Customer")]
    public class Customer : AuditableEntity {
        public Customer(string firstName, string lastName, string email) {
            Update(firstName, lastName, email);
            CustomerResourceId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; private set; }

        public Guid CustomerResourceId { get; private set; }

        [StringLength(50)]
        public string FirstName { get; private set; }
        [StringLength(50)]
        public string LastName { get; private set; }
        [StringLength(250)]
        public string Email { get; private set; }

        public void Update(string firstName, string lastName, string email) {
            var messages = new MessageList();
            messages.Aggregate(() => string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2, () => new InvalidValueError(nameof(firstName), firstName));
            messages.Aggregate(() => string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2, () => new InvalidValueError(nameof(lastName), lastName));
            string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";
            messages.Aggregate(() => string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, regex, RegexOptions.IgnoreCase), () => new InvalidValueError(nameof(email), email));
            messages.ThrowIfAny<ValidationListException>();

            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }
}
