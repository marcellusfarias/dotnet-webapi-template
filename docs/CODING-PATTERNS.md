### CODE

Always wrap if/else/while/for in curly braces. Don't do this:

```csharp
if (expression)
    return true;
```

Do this instead:
```csharp
if (expression)
{
    return true;
}
```

On creating new Configuration, never use Cascade delete style. Always use Restrict.

#### CancelationToken on integration tests

Ensure we have a CancelationToken on integration tests, following the following pattern and that we pass then around.

```csharp
private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
```

### ARCHITECTURE

When implementing a new endpoint, you need to add a new .http file under the Http folder, following the folder structure, where each folder represents a controller.

### TESTING

When implementing a change or a feature, always run the tests and make sure they are passing.

We need to have Integration Tests and Unit Tests.

### BACKLOG.md

When your feature is ready, remove it from the backlog.md file.