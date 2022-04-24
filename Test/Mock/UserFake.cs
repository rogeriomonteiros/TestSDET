using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Domain;

namespace Test.Mock
{
    public class UserFake
    {
        public static Guid Manager1Id = Guid.NewGuid();
        public static Guid Manager2Id = Guid.NewGuid();
        public static Guid Employee1Id = Guid.NewGuid();
        public static Guid Employee2Id = Guid.NewGuid();

        public static User Manager1() => new()
        {
            Id = Manager1Id,
            Email = "manager1@test.com.br",
            Password = "manager1",
            IsManager = true
        };
        public static User Manager2() => new()
        {
            Id = Manager2Id,
            Email = "manager2@test.com.br",
            Password = "manager2",
            IsManager = true
        };
        public static User Employee1() => new()
        {
            Id = Employee1Id,
            Email = "employee1@test.com.br",
            Password = "employee1",
            Documents = new List<Document> { DocumentFake.Document1(), DocumentFake.Document2() },
            IsManager = false
        };
        public static User Employee2() => new()
        {
            Id = Employee2Id,
            Email = "employee1@test.com.br",
            Password = "employee1",
            Documents = new List<Document> { DocumentFake.Document3(), DocumentFake.Document4() },
            IsManager = false
        };

        public static List<User> AllUsers() => new()
        {
            Manager1(), Manager2(), Employee1(), Employee2()
        };

    }
}
