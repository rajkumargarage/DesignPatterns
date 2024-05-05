using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.Behavioral.ChainOfResponsibilityPattern
{
    public class ChainOfResponsibilityV2
    {

        public static void Invoke()
        {
            var game = new Game();
            var goblin = new Creature(game, "Goblin", 2, 2);
            Console.WriteLine(goblin);

            using (new DoubleAttackModifier(game, goblin))
            {
                Console.WriteLine(goblin);
                using (new TripleDefenseModifier(game, goblin))
                {
                    Console.WriteLine(goblin);
                }
            }
            Console.WriteLine(goblin);
        }

        public class Game
        {
            public EventHandler<Query> Queries;

            public void PerformQuery(object sender, Query query)
            {
                Queries?.Invoke(sender, query);
            }
        }

        public class Query
        {
            public string CreatureName { get; set; }

            public enum Argument
            {
                Attack, Defense
            }

            public Argument WhatToQuery;

            public int Value;

            public Query(string creatureName, Argument whatToQuery, int value)
            {
                CreatureName = creatureName ?? throw new ArgumentNullException(nameof(creatureName));
                WhatToQuery = whatToQuery;
                Value = value;
            }
        }

        public class Creature
        {
            private Game game;
            public string Name;
            private int attack, defense;

            public Creature(Game game, string name, int attack, int defense)
            {
                this.game = game ?? throw new ArgumentNullException(nameof(game));
                Name = name ?? throw new ArgumentNullException(nameof(name));
                this.attack = attack;
                this.defense = defense;
            }

            public int Attack
            {
                get
                {
                    var q = new Query(Name, Query.Argument.Attack, attack);
                    game.PerformQuery(this, q);
                    return q.Value;

                }
            }
            public int Defense
            {
                get
                {
                    var q = new Query(Name, Query.Argument.Defense, defense);
                    game.PerformQuery(this, q);
                    return q.Value;

                }
            }

            public override string ToString()
            {
                return $"{nameof(Name)}:{Name} ; {nameof(Attack)}:{Attack}; {nameof(Defense)}:{Defense}";
            }
        }

        public abstract class CreatureModifier : IDisposable
        {
            protected Game game;
            protected Creature creature;

            protected CreatureModifier(Game game, Creature creature)
            {
                this.game = game ?? throw new ArgumentNullException(nameof(game));
                this.creature = creature ?? throw new ArgumentNullException(nameof(creature));
                game.Queries += Handle; // Chaining is done here
            }

            protected abstract void Handle(object sender, Query q);

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects).
                        game.Queries -= Handle;
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~CreatureModifier()
            // {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion

        }

        public class DoubleAttackModifier : CreatureModifier
        {
            public DoubleAttackModifier(Game game, Creature creature) : base(game, creature)
            {
            }

            protected override void Handle(object sender, Query q)
            {
                if (q.CreatureName == creature.Name && q.WhatToQuery == Query.Argument.Attack)
                {
                    q.Value *= 2;
                }
            }
        }

        public class TripleDefenseModifier : CreatureModifier
        {
            public TripleDefenseModifier(Game game, Creature creature) : base(game, creature)
            {
            }

            protected override void Handle(object sender, Query q)
            {
                if (q.CreatureName == creature.Name && q.WhatToQuery == Query.Argument.Defense)
                {
                    q.Value *= 3;
                }
            }
        }
    }
}
