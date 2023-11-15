using System;
using System.Collections.Generic;
using System.Linq;
using Cortside.AspNetCore.Auditable;
using Cortside.Common.Security;
using Acme.IdentityServer.WebApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace Acme.IdentityServer.WebApi.Tests {
    public abstract class BaseTestFixture : IDisposable {
        static Random r = new Random();
        private List<Mock> mocks;

        protected BaseTestFixture() {
            mocks = new List<Mock>();
        }

        public void Arrange(Action arrange) {
            try {
                arrange();
            } catch (Exception) {
                throw;
                //Assert.Inconclusive($"{ex.Message} {Environment.NewLine} {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Loops through the lists and compare each element side by side against the provided function.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="listA"></param>
        /// <param name="listB"></param>
        /// <param name="compareAction"></param>
        /// <returns></returns>
        protected bool CompareAll<TTypeA, TTypeB>(List<TTypeA> listA, List<TTypeB> listB, Func<TTypeA, TTypeB, bool> compareAction) {
            if (compareAction == null) {
                throw new ArgumentNullException(nameof(compareAction));
            }
            if (listA == null) {
                throw new ArgumentNullException(nameof(listA));
            }
            if (listB == null) {
                throw new ArgumentNullException(nameof(listB));
            }
            if (listA.Count != listB.Count) {
                throw new Exception("List must have the same size");
            }
            for (var i = 0; i < listA.Count; i++) {
                if (!compareAction(listA[i], listB[i])) {
                    return false;
                }
            }
            return true;
        }

        public T CreateTestData<T>() //Would normally use Autofixture, but not available for .NET Core
        {
            var instance = Activator.CreateInstance<T>();
            var props = typeof(T).GetProperties().ToList();

            foreach (var x in props) {
                if (x.PropertyType == typeof(Guid)) {
                    x.SetValue(instance, Guid.NewGuid());
                } else if (x.PropertyType == typeof(string)) {
                    x.SetValue(instance, Guid.NewGuid().ToString());
                } else if (x.PropertyType == typeof(bool)) {
                    x.SetValue(instance, Convert.ToBoolean(r.Next(0, 1)));
                } else if (x.PropertyType == typeof(short)) {
                    x.SetValue(instance, Convert.ToInt16(r.Next(short.MinValue, short.MaxValue)));
                } else if (x.PropertyType == typeof(int)) {
                    x.SetValue(instance, r.Next());
                } else if (x.PropertyType == typeof(long)) {
                    x.SetValue(instance, r.NextLong());
                } else if (x.PropertyType == typeof(double)) {
                    x.SetValue(instance, r.Next() + r.NextDouble());
                } else if (x.PropertyType == typeof(decimal)) {
                    x.SetValue(instance, Convert.ToDecimal(r.Next() + r.NextDouble()));
                } else if (x.PropertyType == typeof(float)) {
                    x.SetValue(instance, Convert.ToSingle(r.Next() + r.NextDouble()));
                } else if (x.PropertyType == typeof(byte[])) {
                    byte[] bytes = new byte[10];
                    r.NextBytes(bytes);
                    x.SetValue(instance, bytes);
                } else if (x.PropertyType == typeof(DateTime)) {
                    x.SetValue(instance, DateTime.Now);
                }
            }

            return instance;
        }

        public virtual void Dispose() {
            mocks.ForEach(m => m.VerifyAll());
        }

        protected Mock<IQueryable<T>> MockAsyncQueryable<T>(params T[] input) {
            var data = input.AsQueryable();
            Mock<IQueryable<T>> usersMock = new Mock<IQueryable<T>>();
            usersMock.Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<T>(data.Provider));
            usersMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            usersMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            usersMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return usersMock;
        }

        protected Mock<T> InstantiateMock<T>() where T : class {
            var mock = new Mock<T>();
            mocks.Add(mock);
            return mock;
        }

        public IdentityServerDbContext GetDatabaseContext() {
            var subjectMock = new Mock<ISubjectPrincipal>();
            subjectMock.Setup(x => x.SubjectId).Returns(Guid.NewGuid().ToString());
            var httpMock = new Mock<IHttpContextAccessor>();

            var databaseContextOptions = new DbContextOptionsBuilder<IdentityServerDbContext>()
                .UseInMemoryDatabase($"db-{Guid.NewGuid():d}")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var dbName = $"DBNAME_{Guid.NewGuid()}";
            var dbOptions = new DbContextOptionsBuilder<IdentityServerDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new IdentityServerDbContext(dbOptions, subjectMock.Object, httpMock.Object, new DefaultSubjectFactory());
        }
    }
}
