namespace OpenShock.MinimalEvents.Tests;

public class SyncTypedEventTest
{
    [Test]
    public async Task BasicEventFunctionality()
    {
        var eventRaised = false;
        var minimalEvent = new MinimalEvent<object>();
        var subscription = minimalEvent.Subscribe(_ => eventRaised = true);
        minimalEvent.Invoke(new object());

        await Assert.That(eventRaised).IsTrue();
    }

    [Test]
    public async Task Unsubscribe()
    {
        var eventRaised = false;
        var minimalEvent = new MinimalEvent<object>();
        var subscription = minimalEvent.Subscribe(_ => eventRaised = true);
        subscription.Dispose();
        minimalEvent.Invoke(new object());
        
        await Assert.That(eventRaised).IsFalse();
    }
    
    [Test]
    public async Task MultipleSubscriptions()
    {
        var eventRaised = 0;
        var minimalEvent = new MinimalEvent<object>();
        var subscription1 = minimalEvent.Subscribe(_ => eventRaised++);
        var subscription2 = minimalEvent.Subscribe(_ => eventRaised++);
        minimalEvent.Invoke(new object());
        
        await Assert.That(eventRaised).IsEqualTo(2);
    }
    
    [Test]
    public async Task MultipleSubscriptionsWithDifferentTypes()
    {
        var eventRaised = 0;
        var minimalEvent = new MinimalEvent<object>();
        var subscription1 = minimalEvent.Subscribe(_ => eventRaised++);
        var minimalEvent2 = new MinimalEvent<int>();
        var subscription2 = minimalEvent2.Subscribe(_ => eventRaised++);
        minimalEvent.Invoke(new object());
        minimalEvent2.Invoke(0);
        
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
        object? receivedArg = null;
        var minimalEvent = new MinimalEvent<object>();
        var subscription = minimalEvent.Subscribe(arg => receivedArg = arg);
        minimalEvent.Invoke(inputArg!);
        
        await Assert.That(receivedArg).IsEqualTo(inputArg);
    }
}