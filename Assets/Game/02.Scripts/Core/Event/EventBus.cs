using System;
using System.Collections.Generic;

namespace Game.Core
{
    public class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlerDict = new();

        public IDisposable Subscribe<T>(Action<T> handler) where T : struct
        {
            var eventType = typeof(T);

            if (!_handlerDict.ContainsKey(eventType))
            {
                _handlerDict[eventType] = new List<Delegate>();
            }

            _handlerDict[eventType].Add(handler);

            return new Subscription<T>(this, handler);
        }

        public void Publish<T>(T gameEvent) where T : struct
        {
            var eventType = typeof(T);

            if (!_handlerDict.TryGetValue(eventType, out var handlers) || handlers.Count == 0)
            {
                return;
            }

            for (var i = handlers.Count - 1; i >= 0; i--)
            {
                if (handlers[i] is Action<T> handler)
                {
                    handler.Invoke(gameEvent);
                }
            }
        }

        private void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            var eventType = typeof(T);

            if (_handlerDict.TryGetValue(eventType, out var handlers))
            {
                handlers.Remove(handler);
            }
        }

        private class Subscription<T> : IDisposable where T : struct
        {
            private readonly EventBus _eventBus;
            private readonly Action<T> _handler;
            private bool _disposed;

            public Subscription(EventBus eventBus, Action<T> handler)
            {
                _eventBus = eventBus;
                _handler = handler;
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _eventBus.Unsubscribe(_handler);
                _disposed = true;
            }
        }
    }
}
