using System.Collections.Concurrent;
using System.Text.Json;

var eventBus = new MyEventBus();
//静态注册
eventBus.Subscribe("A", new AEventHandler());

while (true)
{
    var inStr = Console.ReadLine();
    if (inStr.StartsWith("Add", StringComparison.OrdinalIgnoreCase))
    {
        //动态注册
        eventBus.Subscribe("B", new BEventHandler());
        Console.WriteLine("增加B事件的处理器");
        continue;
    }

    //语法格式 name,arg
    var spliter = inStr.Split(",");
    if (spliter.Length < 2)
    {
        Console.WriteLine("语法格式为<name>,<arg>");
        continue;
    }
    var name = spliter[0];
    var val = spliter[1];
    await eventBus.Publish(name, new { val });
}

public class AEventHandler : IEventHandler
{
    public Task Handler(dynamic eArg)
    {
        Console.WriteLine("AEventHandler Exec");
        Console.WriteLine(JsonSerializer.Serialize(eArg));
        return Task.CompletedTask;
    }
}

public class BEventHandler : IEventHandler
{
    public Task Handler(dynamic eArg)
    {
        Console.WriteLine("BEventHandler Exec");
        Console.WriteLine(JsonSerializer.Serialize(eArg));
        return Task.CompletedTask;
    }
}

internal interface IEventHandler
{
    Task Handler(dynamic eArg);
}

internal class MyEventBus
{
    private static readonly ConcurrentDictionary<string, List<IEventHandler>> _handlers = new ConcurrentDictionary<string, List<IEventHandler>>();

    public void Subscribe(string name, IEventHandler handler)
    {
        if (_handlers.TryGetValue(name, out var handlers))
        {
            handlers.Add(handler);
        }
        else
        {
            _handlers[name] = new List<IEventHandler>() { handler };
        }
    }

    public async Task Publish(string name, dynamic eArg)
    {
        if (_handlers.TryGetValue(name, out var handlers))
        {
            foreach (var handler in handlers)
            {
                await handler.Handler(eArg).ConfigureAwait(false);
            }
        }
        else
        {
            Console.WriteLine($"没有事件{name}的处理器");
            return;
        }
    }
}