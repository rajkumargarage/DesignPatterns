using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignPatterns.Behavioral.MediatorPattern
{
    public class Mediator
    {
        public static void Invoke()
        {
            var hulk = new Person("Hulk");
            var ironMan = new Person("Iron Man");
            var thanos = new Person("Thanos");
            var blackwidow = new Person("Black Widow");

            var lunchGroup = new ChatRoom("Lunch Group");

            lunchGroup.Join(hulk);
            lunchGroup.Join(blackwidow);

            var teamA = new ChatRoom("Team A");

            teamA.Join(ironMan);
            teamA.Join(hulk);

            teamA.Send(new Message("HI iron.. Are you alive..", hulk, ironMan));

        }
    }

    interface IChatableComponent
    {
        public string Name { get; set; }
    }

    class Message
    {
        public string Text { get; set; }

        public Person From { get; set; }
        public List<Person> To { get; set; } = new List<Person>();

        public Message(string text, Person from, params Person[] to)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            From = from ?? throw new ArgumentNullException(nameof(from));
            To.AddRange(to ?? throw new ArgumentNullException(nameof(to)));
        }
    }


    class Person : IChatableComponent
    {
        public string Name { get; set; }

        public List<ChatRoom> chatRoom = new List<ChatRoom>();

        public Person(string name)
        {
            Name = name;
        }
    }

    class ChatRoom : IChatableComponent
    {
        List<Person> persons = new List<Person>();
        public string Name { get; set; }

        public ChatRoom(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void Join(Person person)
        {
            Console.WriteLine($"{person.Name} joined the chat {Name}");

            persons.Add(person);

            person.chatRoom.Add(this);
        }

        public void BroadCast(string message, Person sender)
        {
            foreach (var person in persons.Where(x => x != sender))
            {
                Send(new Message(message, sender, person));
            }
        }

        public void Send(Message message)
        {
            foreach (var receiver in message.To)
            {
                if (!persons.Any(x => x == receiver))
                {
                    Console.WriteLine($"{receiver.Name} is not part of {Name}");
                }

                Console.WriteLine($"{Name}'s Chat Session");
                Console.WriteLine($"{message.From.Name}:{message.Text}");
            }
        }
    }



}
