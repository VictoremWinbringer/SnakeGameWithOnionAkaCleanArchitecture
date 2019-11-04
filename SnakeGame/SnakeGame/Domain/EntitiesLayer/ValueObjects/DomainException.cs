using System;
using System.Runtime.Serialization;

[Serializable]
sealed class DomainException : Exception
{
    public DomainException(DomainExceptionCode code) : base() { Code = code; }
    public DomainException(DomainExceptionCode code, string message) : base(message) { Code = code; }
    public DomainException(DomainExceptionCode code, string message, Exception inner) : base(message, inner) { Code = code; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Code), Code);
        base.GetObjectData(info, context);
    }

    public DomainExceptionCode Code { get; }
}
