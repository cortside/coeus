//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Acme.ShoppingCart.Data;
//using Acme.ShoppingCart.Domain.Entities;
//using Acme.ShoppingCart.Dto;

//namespace Acme.ShoppingCart.DomainService {
//    public class SubjectService : ISubjectService {
//        private readonly DatabaseContext dbContext;

//        public SubjectService(DatabaseContext dbContext) {
//            this.dbContext = dbContext;
//        }
//        public Task SaveAsync(SubjectDto subject) {
//            var subjectRow = dbContext.Subjects.FirstOrDefault(s => s.SubjectId == subject.SubjectId);
//            if (subjectRow == null) {
//                dbContext.Subjects.Add(new Subject() {
//                    SubjectId = subject.SubjectId,
//                    FamilyName = subject.FamilyName,
//                    GivenName = subject.GivenName,
//                    Name = subject.Name,
//                    UserPrincipalName = subject.UserPrincipalName,
//                    CreatedDate = DateTime.UtcNow
//                });
//            } else {
//                subjectRow.Name = subject.Name;
//                subjectRow.GivenName = subject.GivenName;
//                subjectRow.FamilyName = subject.FamilyName;
//                subjectRow.UserPrincipalName = subject.UserPrincipalName;
//            }
//            return dbContext.SaveChangesAsync();
//        }
//    }
//}
