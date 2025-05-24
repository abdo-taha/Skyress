namespace Skyress.Domain.Common;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
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

    public static Result Failure(Error error) => new Result(false, error);

    public static Result<TValue> Create<TValue>(TValue? value) => Success(value);

}
