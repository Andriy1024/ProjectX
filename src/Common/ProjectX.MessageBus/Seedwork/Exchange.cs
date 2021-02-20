using ProjectX.Core;

namespace ProjectX.RabbitMq
{
    public class Exchange 
    {
        public sealed class Type : StringEnumeration
        {
            public readonly static Type Direct = new Type("direct");
            public readonly static Type Fanout = new Type("fanout");

            protected Type(string value)
                : base(value) { }

            public static implicit operator Type(string value)
            {
                return FindValue<Type>(value);
            }
        }

        public sealed class Name : StringEnumeration
        {
            public readonly static Name Identity = new Name("ProjectX.Identity");
            public readonly static Name Realtime = new Name("ProjectX.Realtime");
            public readonly static Name Blog = new Name("ProjectX.Blog");

            protected Name(string value)
                : base(value) { }

            public static implicit operator Name(string value)
            {
                return FindValue<Name>(value);
            }
        }
    }
}
