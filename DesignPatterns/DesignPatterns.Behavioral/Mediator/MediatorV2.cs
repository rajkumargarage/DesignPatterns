using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Autofac;

namespace DesignPatterns.Behavioral.MediatorPattern
{
    public class MediatorV2
    {
        public static void Invoke()
        {
            var cb = new ContainerBuilder();

            cb.RegisterType<EventBroker>().SingleInstance();
            cb.Register((c, p) => new FootballCoach(p.Named<string>("name"), c.Resolve<EventBroker>()));
            cb.Register((c, p) => new FootballPlayer(p.Named<string>("name"), c.Resolve<EventBroker>()));

            using (var c = cb.Build())
            {
                var coach = c.Resolve<FootballCoach>(new NamedParameter("name", "Thanos"));
                var player1 = c.Resolve<FootballPlayer>(new NamedParameter("name", "Player1"));
                var player2 = c.Resolve<FootballPlayer>(new NamedParameter("name", "Player2"));

                player1.Score();
                player1.Score();
                player1.Score();

                player2.AssaultReferee();

            }
        }
    }


    class Actor
    {
        protected EventBroker broker;

        public string Name { get; set; }

        public Actor(string name, EventBroker broker)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            this.broker = broker ?? throw new ArgumentNullException(nameof(broker));
        }
    }

    class FootballPlayer : Actor
    {
        public int GoalsScored { get; set; }
        public FootballPlayer(string name, EventBroker broker) : base(name, broker)
        {
            broker.OfType<PlayerScoredEvent>().Where(ps => !ps.Name.Equals(Name)).Subscribe(ps =>
            {

                Console.WriteLine($"Player({Name}): Nicely done {ps.Name}. It's your {ps.GoalsScored} goal");
            });

            broker.OfType<PlayerSentOffEvent>().Where(ps => !ps.Name.Equals(Name)).Subscribe(ps =>
            {
                Console.WriteLine($"Player({Name}): Will meet you {ps.Name} in dressing room");
            });
        }

        internal void AssaultReferee()
        {
            broker.Publish(new PlayerSentOffEvent { Name = this.Name, Reason = PlayerSentOffEvent.PlayerSentOffReason.Violence });
        }

        internal void Score()
        {
            GoalsScored++;
            broker.Publish(new PlayerScoredEvent { Name = this.Name, GoalsScored = this.GoalsScored });
        }
    }

    class FootballCoach : Actor
    {
        public FootballCoach(string name, EventBroker broker) : base(name, broker)
        {
            broker.OfType<PlayerScoredEvent>().Subscribe(pe =>
            {
                if (pe.GoalsScored < 3)
                    Console.WriteLine($"Coach({Name}): well done.{pe.Name}");
            });

            broker.OfType<PlayerSentOffEvent>().Subscribe(pe =>
            {
                if (pe.Reason == PlayerSentOffEvent.PlayerSentOffReason.Violence)
                    Console.WriteLine($"Coach({Name}): how could you, {pe.Name}");
            });
        }
    }

    class PlayerEvent
    {
        public string Name { get; set; }
    }

    class PlayerScoredEvent : PlayerEvent
    {
        public int GoalsScored { get; set; }
    }

    class PlayerSentOffEvent : PlayerEvent
    {
        public enum PlayerSentOffReason
        {
            Violence
        }
        public PlayerSentOffReason Reason { get; set; }
    }

    class EventBroker : IObservable<PlayerEvent>
    {

        private Subject<PlayerEvent> subscriptions = new Subject<PlayerEvent>();

        public IDisposable Subscribe(IObserver<PlayerEvent> observer)
        {
            return subscriptions.Subscribe(observer);
        }

        public void Publish(PlayerEvent pe)
        {
            subscriptions.OnNext(pe);
        }
    }
}
