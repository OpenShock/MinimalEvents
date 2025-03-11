using OpenShock.MinimalEvents;

namespace MinimalEventsTests;

public class AsyncTypedEventTest
{
    [Test]
    public async Task BasicEventFunctionality()
    {
        var eventRaised = false;
        var minimalEvent = new AsyncMinimalEvent<object>();
        var subscription = await minimalEvent.SubscribeAsync(_ =>
        {
            eventRaised = true;
            return Task.CompletedTask;
        });
        
        await minimalEvent.InvokeAsyncParallel(new object());

        await Assert.That(eventRaised).IsTrue();
    }
    
    [Test]
    public async Task EventUnsubscribe()
    {
        var eventRaised = false;
        var minimalEvent = new AsyncMinimalEvent<object>();
        var subscription = await minimalEvent.SubscribeAsync(_ =>
        {
            eventRaised = true;
            return Task.CompletedTask;
        });
        
        await subscription.DisposeAsync();
        await minimalEvent.InvokeAsyncParallel(new object());

        await Assert.That(eventRaised).IsFalse();
    }
    
    [Test]
    public async Task MultipleSubscriptions()
    {
        var eventRaised = 0;
        var minimalEvent = new AsyncMinimalEvent<object>();
        var subscription1 = await minimalEvent.SubscribeAsync(_ =>
        {
            Interlocked.Increment(ref eventRaised);
            return Task.CompletedTask;
        });
        var subscription2 = await minimalEvent.SubscribeAsync(_ =>
        {
            Interlocked.Increment(ref eventRaised);
            return Task.CompletedTask;
        });
        
        await minimalEvent.InvokeAsyncParallel(new object());

        await Assert.That(eventRaised).IsEqualTo(2);
    }
    
    [Test]
    public async Task MultipleSubscriptionsWithDifferentTypes()
    {
        var eventRaised = 0;
        var minimalEvent = new AsyncMinimalEvent<object>();
        var minimalEvent2 = new AsyncMinimalEvent<int>();
        var subscription1 = await minimalEvent.SubscribeAsync(_ =>
        {
            Interlocked.Increment(ref eventRaised);
            return Task.CompletedTask;
        });
        var subscription2 = await minimalEvent2.SubscribeAsync(_ =>
        {
            Interlocked.Increment(ref eventRaised);
            return Task.CompletedTask;
        });
        
        await minimalEvent.InvokeAsyncParallel(new object());
        await minimalEvent2.InvokeAsyncParallel(1);

        await Assert.That(eventRaised).IsEqualTo(2);
    }
    
    [Test]
    [Arguments("Hello, World!")]
    [Arguments(0)]
    [Arguments(null)]
    [Arguments(typeof(string))]
    // ReSharper disable once UseCollectionExpression
    [Arguments(new object[] {new object[] {1, 2, 3}})]
    public async Task ArgumentValue(object? inputArg)
    {
        object? eventArg = null;
        var minimalEvent = new AsyncMinimalEvent<object>();
        var subscription = await minimalEvent.SubscribeAsync(arg =>
        {
            eventArg = arg;
            return Task.CompletedTask;
        });
        
        await minimalEvent.InvokeAsyncParallel(inputArg!);

        await Assert.That(eventArg).IsEqualTo(inputArg);
    }

}