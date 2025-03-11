namespace OpenShock.MinimalEvents.Tests;

public class AsyncEventTest
{
    [Test]
    public async Task BasicEventFunctionality()
    {
        var eventRaised = false;
        var minimalEvent = new AsyncMinimalEvent();
        var subscription = await minimalEvent.SubscribeAsync(() =>
        {
            eventRaised = true;
            return Task.CompletedTask;
        });
        
        await minimalEvent.InvokeAsyncParallel();

        await Assert.That(eventRaised).IsTrue();
    }
    
    [Test]
    public async Task Unsubscribe()
    {
        var eventRaised = false;
        var minimalEvent = new AsyncMinimalEvent();
        var subscription = await minimalEvent.SubscribeAsync(() =>
        {
            eventRaised = true;
            return Task.CompletedTask;
        });
        await subscription.DisposeAsync();
        
        await minimalEvent.InvokeAsyncParallel();
        
        await Assert.That(eventRaised).IsFalse();
    }
    
    [Test]
    public async Task MultipleSubscriptions()
    {
        var eventRaised = 0;
        var minimalEvent = new AsyncMinimalEvent();
        var subscription1 = await minimalEvent.SubscribeAsync(() =>
        {
            Interlocked.Increment(ref eventRaised);
            return Task.CompletedTask;
        });
        var subscription2 = await minimalEvent.SubscribeAsync(() =>
        {
            Interlocked.Increment(ref eventRaised);
            return Task.CompletedTask;
        });
        
        await minimalEvent.InvokeAsyncParallel();
        
        await Assert.That(eventRaised).IsEqualTo(2);
    }
    
}