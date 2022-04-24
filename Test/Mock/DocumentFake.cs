using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Domain;

namespace Test.Mock
{
    public static class DocumentFake
    {
        public static Guid Document1Id = Guid.NewGuid();
        public static Guid Document2Id = Guid.NewGuid();
        public static Guid Document3Id = Guid.NewGuid();
        public static Guid Document4Id = Guid.NewGuid();

        public static Document Document1() => new()
        {
            Id = Document1Id,
            Name = "Document 1"
        };
        public static Document Document2() => new()
        {
            Id = Document2Id,
            Name = "Document 2"
        };
        public static Document Document3() => new()
        {
            Id = Document3Id,
            Name = "Document 3"
        };
        public static Document Document4() => new()
        {
            Id = Document4Id,
            Name = "Document 4"
        };

    }
}
