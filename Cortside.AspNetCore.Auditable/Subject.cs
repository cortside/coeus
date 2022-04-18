using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acme.ShoppingCart.Domain.Entities
{
    [Table("Subject")]
    public class Subject
    {
        public Subject(Guid subjectId, string givenName, string familyName, string name, string userPrincipalName)
        {
            SubjectId = subjectId;
            GivenName = givenName;
            FamilyName = familyName;
            Name = name;
            UserPrincipalName = userPrincipalName;
            CreatedDate = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid SubjectId { get; private set; }

        [StringLength(100)]
        public string Name { get; private set; }

        [StringLength(100)]
        public string GivenName { get; private set; }

        [StringLength(100)]
        public string FamilyName { get; private set; }

        [StringLength(100)]
        public string UserPrincipalName { get; private set; }

        public DateTime CreatedDate { get; private set; }
    }
}