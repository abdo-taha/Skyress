namespace Skyress.Domain.Common;

public class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && Error != Error.None)
        {
            throw new InvalidOperationException();
        }
        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException();
        }
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public Error Error { get; }

    public bool IsFailure => !IsSuccess;

    public static Result Success() => new Result(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue? value) => new Result<TValue>(value, true, Error.None);

    public static Result<TValue> Create<TValue>(TValue value, Error error)
    where TValue : class
    => value is null ? Failure<TValue>(error) : Success(value);

    public static Result Failure(Error error) => new Result(false, error);
    public static Result<TValue> Failure<TValue>(Error error) => new Result<TValue>(default!, false, error);

    public static Result<TValue> Create<TValue>(TValue? value) => Success(value);

}
