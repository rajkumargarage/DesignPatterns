using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DesignPatterns.Behavioral.StrategyPattern
{
    class Strategy
    {
        public static void Invoke()
        {
            var tp = new TextProcessor();
            Console.WriteLine(tp.GetProcessedText("india", "china,USA", "UK"));

            var tp1 = new TextProcessor<MarkDownListStrategy>();

            Console.WriteLine(tp1.GetProcessedText("1", "2", "3"));
            //  tp1 = new TextProcessor<MarkDownListStrategy>(); it will throw error because <T> will match exact type not the type mentioned in Type constraint

            var people = new People();
            people.Add(new Person(3, "1c"));
            people.Add(new Person(1, "1a"));
            people.Add(new Person(2, "2b"));
            people.Persons.Sort();
            Console.WriteLine(people);
            people.Persons.Sort(new Person.NameComparer());
            Console.WriteLine(people);
        }
    }

    internal class People
    {

        public Person this[int index]
        {
            get => Persons[index];
        }

        public List<Person> Persons { get; set; } = new List<Person>();

        public People()
        {
        }

        internal void Add(Person person) => Persons.Add(person);

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var item in Persons)
            {
                sb.AppendLine(item.Name);
            }

            return sb.ToString();
        }
    }

    interface IListStrategy
    {
        void Start(StringBuilder sb);
        void Stop(StringBuilder sb);
        void AddListItem(StringBuilder sb, string item);
    }

    class HtmlListStrategy : IListStrategy
    {
        public void AddListItem(StringBuilder sb, string item)
        {

            sb.AppendLine($" <li>{item}</li>");
        }

        public void Start(StringBuilder sb)
        {
            sb.AppendLine($"<ul>");

        }

        public void Stop(StringBuilder sb)
        {
            sb.AppendLine($"</ul>");
        }
    }

    class MarkDownListStrategy : IListStrategy
    {
        public void AddListItem(StringBuilder sb, string item)
        {
            sb.AppendLine($"*{item}");
        }

        public void Start(StringBuilder sb)
        {
        }

        public void Stop(StringBuilder sb)
        {
        }
    }

    class TextProcessor
    {
        IListStrategy ListStrategy { get; set; }
        public TextProcessor()
        {
            ListStrategy = new HtmlListStrategy();
        }

        /// <summary>
        /// this allows us to vary the strategy at runtime
        /// </summary>
        /// <param name="listStrategy"></param>
        public void SetStrategy(IListStrategy listStrategy)
        {
            ListStrategy = listStrategy;
        }

        public string GetProcessedText(params string[] items)
        {
            var sb = new StringBuilder();
            ListStrategy.Start(sb);

            foreach (var item in items)
                ListStrategy.AddListItem(sb, item);

            ListStrategy.Stop(sb);
            return sb.ToString();
        }
    }

    /// <summary>
    /// allows us to pass list strategy at runtime
    /// </summary>
    /// <typeparam name="LS"></typeparam>
    class TextProcessor<LS> where LS : IListStrategy, new()
    {
        IListStrategy ListStrategy { get; set; } = new LS();
        public TextProcessor()
        {

        }

        /// <summary>
        /// this allows us to vary the strategy at runtime
        /// </summary>
        /// <param name="listStrategy"></param>
        public void SetStrategy(LS listStrategy)
        {
            ListStrategy = listStrategy;
        }

        public string GetProcessedText(params string[] items)
        {
            var sb = new StringBuilder();
            ListStrategy.Start(sb);

            foreach (var item in items)
                ListStrategy.AddListItem(sb, item);

            ListStrategy.Stop(sb);
            return sb.ToString();
        }
    }

    class Person : IComparable, IComparable<Person>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Person(int id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public int CompareTo([AllowNull] Person other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Id.CompareTo(other.Id);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(this, obj)) return 0;
            if (ReferenceEquals(null, obj)) return 1;
            return obj is Person other ? CompareTo(other) : throw new ArgumentException("Can't Compare objects. Type doesn't match");
        }

        internal sealed class NameComparer : IComparer<Person>
        {
            public int Compare([AllowNull] Person x, [AllowNull] Person y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(null, x)) return -1;
                if (ReferenceEquals(null, y)) return 1;
                return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            }
        }

    }
}
