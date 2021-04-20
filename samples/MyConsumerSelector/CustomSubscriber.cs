using System;
using DotNetCore.CAP;
using Newtonsoft.Json;

namespace MyConsumerSelector
{
    public class CustomSubscriber : IMessageSubscriber, ICapSubscribe
    {
        [MessageSubscription("string")]
        public void String(string message)
        {
            Console.WriteLine($"String: {message}");
        }
        
        [MessageSubscription("message.string")]
        public void String(Message<string> message)
        {
            Console.WriteLine($"String: {JsonConvert.SerializeObject(message)}");
        }
        
        [MessageSubscription("message.datetime")]
        public void Date(Message<DateTime> message, [FromCap] CapHeader header)
        {
            Console.WriteLine($"Date: {JsonConvert.SerializeObject(message)}");
            Console.WriteLine(JsonConvert.SerializeObject(header));
        }
        
        [MessageSubscription("message.bytes")]
        public void Bytes(Message<byte[]> message, [FromCap] CapHeader header)
        {
            Console.WriteLine($"Bytes: {JsonConvert.SerializeObject(message)}");
            Console.WriteLine(JsonConvert.SerializeObject(header));
        }

        [CapSubscribe("cap")]
        public void Cap(string message, [FromCap] CapHeader header)
        {
            Console.WriteLine($"Cap {message}");
            Console.WriteLine(JsonConvert.SerializeObject(header));
        }
    }

    public class Message<T> 
    {
        public string Name { get; set; }
        public T Body { get; set; }
    }
}