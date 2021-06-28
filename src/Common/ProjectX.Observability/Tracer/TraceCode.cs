namespace ProjectX.Observability.Tracer
{
    public struct TraceCode
    {
        private TraceCode(string code) => Code = code;

        public string Code { get; }

        public static TraceCode Success = new TraceCode("OK");

        public static TraceCode Error = new TraceCode("ERROR");

        public static TraceCode Unknown = new TraceCode("UNSET");
    }
}
