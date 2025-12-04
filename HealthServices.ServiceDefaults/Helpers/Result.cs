using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace HealthServices.ServiceDefaults.Helpers;

public class Result
{
    public HttpStatusCode? StatusCode { get; }
    public string? StatusMessage { get; }

    protected Result()
    {
        IsSuccess = true;
    }

    protected Result(HttpStatusCode statusCode, string statusMessage)
    {
        IsSuccess = false;
        StatusCode = statusCode;
        StatusMessage = statusMessage ?? string.Empty;
    }

    public bool IsSuccess { get; }

    [MemberNotNullWhen(true, nameof(StatusCode))]
    [MemberNotNullWhen(true, nameof(StatusMessage))]
    public bool IsFailure => !IsSuccess;

    public static Result Success()
    {
        return new Result();
    }

    public static Result Failure(HttpStatusCode statusCode, string statusMessage)
    {
        return new Result(statusCode, statusMessage);
    }
}

public class Result<T> : Result
{
    public T? Value { get; }
    protected Result(T? value)
    {
        Value = value;
    }
    protected Result(HttpStatusCode statusCode, string statusMessage)
        : base(statusCode, statusMessage) { }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public new static Result<T> Failure(HttpStatusCode statusCode, string statusMessage)
    {
        return new Result<T>(statusCode, statusMessage);
    }
}
