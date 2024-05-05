using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.Behavioral.ChainOfResponsibilityPattern
{
    public static class ChainOfResponsibility
    {
        public static void Invoke()
        {
            var bison = new Creature("Bison", 2, 2);

            //Console.WriteLine(bison);

            //var dcm = new DoubleAttackModifier(bison);
            //dcm.Handle();
            //Console.WriteLine(bison);

            //var icm = new IncreaseDefenseModifier(bison);
            //icm.Handle();

            var root = new CreatureModifier(bison);
            var dcm = new DoubleAttackModifier(bison);
            root.Add(dcm);
            Console.WriteLine(bison);

            var icm = new AddTwoDefensePointModifier(bison);
            root.Add(icm);

            var tcm = new AddThreeDefensePointModifier(bison);
            root.Add(tcm);

            var tpcm = new TripleAttackModifier(bison);
            root.Add(tpcm);

            root.Handle();

            Console.WriteLine(bison);
        }


        public class Creature
        {
            public string Name;
            public int Attack, Defense;

            public Creature(string name, int attcak, int defense)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Attack = attcak;
                Defense = defense;
            }

            public override string ToString() => $"{nameof(Name)}:{Name},{nameof(Attack)}:{Attack},{nameof(Defense)}:{Defense}";
        }

        public class CreatureModifier
        {
            protected Creature creature;
            protected CreatureModifier next;

            public CreatureModifier(Creature creature)
            {
                this.creature = creature ?? throw new ArgumentNullException(nameof(creature));
            }

            public void Add(CreatureModifier cm)
            {
                if (next == null) next = cm;
                else
                {
                    next.Add(cm);
                }
            }

            public virtual void Handle() => next?.Handle();
        }

        public class DoubleAttackModifier : CreatureModifier
        {
            public DoubleAttackModifier(Creature creature) : base(creature)
            {
            }

            public override void Handle()
            {
                Console.WriteLine("Doubling Attack");
                creature.Attack *= 2;
                base.Handle();
            }

            public override string ToString()
            {
                return base.ToString();
            }
        }

        public class AddTwoDefensePointModifier : CreatureModifier
        {
            public AddTwoDefensePointModifier(Creature creature) : base(creature)
            {
            }

            public override void Handle()
            {
                Console.WriteLine("Increasing Defense");
                creature.Defense += 2;
                base.Handle();
            }
        }

        public class TripleAttackModifier : CreatureModifier
        {
            public TripleAttackModifier(Creature creature) : base(creature)
            {
            }

            public override void Handle()
            {
                Console.WriteLine("Doubling Attack");
                creature.Attack *= 3;
                base.Handle();
            }

            public override string ToString()
            {
                return base.ToString();
            }
        }

        public class AddThreeDefensePointModifier : CreatureModifier
        {
            public AddThreeDefensePointModifier(Creature creature) : base(creature)
            {
            }

            public override void Handle()
            {
                Console.WriteLine("Increasing Defense");
                creature.Defense += 3;
                base.Handle();
            }
        }
    }

}
