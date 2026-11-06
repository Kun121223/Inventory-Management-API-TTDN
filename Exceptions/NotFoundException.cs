namespace InventoryManagementApi.Exceptions;

/// <summary>
/// Thrown when a requested resource (item, supplier, etc.) does not exist.
/// Mapped to HTTP 404 by ExceptionHandlingMiddleware.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
