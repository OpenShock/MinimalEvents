using OpenShock.MinimalEvents;

namespace MinimalEventsTests;

public class SyncEventTest
{
    [Test]
    public async Task BasicEventFunctionality()
    {
        var eventRaised = false;
        var minimalEvent = new MinimalEvent();
        var subscription = minimalEvent.Subscribe(() => eventRaised = true);
        minimalEvent.Invoke();

        await Assert.That(eventRaised).IsTrue();
    }

    [Test]
    public async Task Unsubscribe()
    {
        var eventRaised = false;
        var minimalEvent = new MinimalEvent();
        var subscription = minimalEvent.Subscribe(() => eventRaised = true);
        subscription.Dispose();
        minimalEvent.Invoke();
        
        await Assert.That(eventRaised).IsFalse();
    }
    
    [Test]
    public async Task TestMultipleSubscriptions()
    {
        var eventRaised = 0;
        var minimalEvent = new MinimalEvent();
        var subscription1 = minimalEvent.Subscribe(() => eventRaised++);
        var subscription2 = minimalEvent.Subscribe(() => eventRaised++);
        minimalEvent.Invoke();
        
        await Assert.That(eventRaised).IsEqualTo(2);
    }
}