using game_x.application.Contract.Infrastructure.Logger;
using ValidationException = game_x.application.Exceptions.ValidationException;

namespace game_x.application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    IAppLogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default)
    {
        // Ignore if there is inheritance (for manual validation)
        if (request is ISkipValidation)
            return await next(ct);

        if (!validators.Any())
            return await next(ct);

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, ct)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();
        if (failures.Count > 0)
        {
            logger.LogError("validate err log:\n {failures}", failures);
            throw new ValidationException(failures);
        }

        return await next(ct);
    }
}
