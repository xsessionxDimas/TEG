using System;
using System.Runtime.Serialization;

namespace CustomTypes.Exceptions
{
    [Serializable]
    public class CustomException : Exception
    {
        public CustomException()
        { }

        public CustomException(string message)
            : base(message) { }

        public CustomException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public CustomException(string message, Exception innerException)
            : base(message, innerException) { }

        public CustomException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected CustomException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ReportException : Exception
    {
        public ReportException()
        { }

        public ReportException(string message)
            : base(message) { }

        public ReportException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public ReportException(string message, Exception innerException)
            : base(message, innerException) { }

        public ReportException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected ReportException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ZeroQuantityException : Exception
    {
        public ZeroQuantityException()
        { }

        public ZeroQuantityException(string message)
            : base(message) { }

        public ZeroQuantityException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public ZeroQuantityException(string message, Exception innerException)
            : base(message, innerException) { }

        public ZeroQuantityException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected ZeroQuantityException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
