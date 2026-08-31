# Minimal Events

Minimal Events is a very opinionated, minimal and modern event library designed to be used in OpenShock projects.

Inspired by [System.Reactive (Rx.NET)](https://github.com/dotnet/reactive), it provides a tiny observable-style API for subscribing to and invoking events, both synchronously and asynchronously.

## Features

- Synchronous and asynchronous events
- Optional strongly-typed event arguments
- Subscriptions are managed via `IDisposable` / `IAsyncDisposable` — dispose to unsubscribe
- Thread-safe subscribe/unsubscribe backed by immutable handler lists
- Targets `net10.0` and `netstandard2.1`

## Installation

```sh
dotnet add package OpenShock.MinimalEvents
```

## Usage

### Synchronous

```csharp
using OpenShock.MinimalEvents;

var onTick = new MinimalEvent();

using (onTick.Subscribe(() => Console.WriteLine("tick")))
{
    onTick.Invoke(); // prints "tick"
}

onTick.Invoke(); // nothing — the subscription was disposed
```

### Synchronous with an argument

```csharp
var onMessage = new MinimalEvent<string>();

using var subscription = onMessage.Subscribe(msg => Console.WriteLine($"got: {msg}"));

onMessage.Invoke("hello"); // prints "got: hello"
```

### Asynchronous

```csharp
var onStarted = new AsyncMinimalEvent();

await using var subscription = await onStarted.SubscribeAsync(async () =>
{
    await Task.Delay(10);
    Console.WriteLine("started");
});

await onStarted.InvokeAsyncParallel(); // invokes all handlers in parallel
```

### Asynchronous with an argument

```csharp
var onValue = new AsyncMinimalEvent<int>();

await using var subscription = await onValue.SubscribeAsync(async value =>
{
    await Task.Delay(10);
    Console.WriteLine($"value: {value}");
});

await onValue.InvokeAsyncParallel(42);
```

Async handlers are invoked in parallel; if any handler throws, the aggregated exception is rethrown once all handlers have completed.

## Exposing events

To expose an event without letting consumers invoke it, return the matching observable interface:

- `IMinimalEventObservable` / `IMinimalEventObservable<T>`
- `IAsyncMinimalEventObservable` / `IAsyncMinimalEventObservable<T>`

```csharp
public sealed class Connection
{
    private readonly AsyncMinimalEvent<string> _onMessage = new();

    // Consumers can subscribe, but only Connection can invoke.
    public IAsyncMinimalEventObservable<string> OnMessage => _onMessage;

    internal Task RaiseAsync(string message) => _onMessage.InvokeAsyncParallel(message);
}
```

## License

[MIT](LICENSE)
