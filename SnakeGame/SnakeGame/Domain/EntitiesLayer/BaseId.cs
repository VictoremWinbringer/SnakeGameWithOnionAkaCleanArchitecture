using System;

abstract class BaseId<T>
{
    protected BaseId(T value)
    {
        if (value == default)
            throw new ArgumentOutOfRangeException(nameof(value));
        Value = value;
    }

    public T Value { get; }
}