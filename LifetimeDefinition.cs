using System;
using System.Collections.Generic;
using System.Linq;

namespace Lifetime
{
    public interface ILifeTime
    {
        void Add(Action OnTerminate);
    }

    public class LifeTime : ILifeTime
    {
        private readonly List<Action> _onTerminate = new List<Action>();

        public void Add(Action OnTerminate) { _onTerminate.Add(OnTerminate); }

        public void Terminate()
        {
            foreach (Action terminateAction in Enumerable.Reverse(_onTerminate))
                terminateAction();
        }
    }

    public static class LifeTimes
    {
        private static readonly LifeTime _eternal = new LifeTime();

        public static LifeTime Eternal
        {
            get { return _eternal; }
        }

        public static ILifetimeDefinition Define(LifeTime Parent)
        {
            var def = new LifetimeDefinition(new LifeTime());
            Parent.Add(def.Dispose);
            return def;
        }
    }

    public interface ILifetimeDefinition : IDisposable
    {
        LifeTime LifeTime { get; }
    }

    public class LifetimeDefinition : ILifetimeDefinition
    {
        private readonly LifeTime _lifeTime;

        public LifetimeDefinition(LifeTime LifeTime) { _lifeTime = LifeTime; }

        public LifeTime LifeTime
        {
            get { return _lifeTime; }
        }

        public void Dispose() { LifeTime.Terminate(); }
    }
}
