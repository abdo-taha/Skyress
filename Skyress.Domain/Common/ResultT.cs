namespace Skyress.Domain.Common;

public class Result<Tvalue> : Result
{
    private readonly Tvalue? _value;
    protected internal Result(Tvalue? value, bool isSuccess, Error error) : base(isSuccess, error)
    => _value = value;

    public Tvalue Value => IsSuccess
           ? _value!
           : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    public static implicit operator Result<Tvalue>(Tvalue? value) => Create(value);
}
