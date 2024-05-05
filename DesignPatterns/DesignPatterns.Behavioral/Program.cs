using System;
using DesignPatterns.Behavioral.ChainOfResponsibilityPattern;
using DesignPatterns.Behavioral.InterpreterPattern;
using DesignPatterns.Behavioral.IteratorPattern;
using DesignPatterns.Behavioral.MediatorPattern;
using DesignPatterns.Behavioral.MementoPattern;
using DesignPatterns.Behavioral.ObserverPattern;
using DesignPatterns.Behavioral.StatePattern;
using DesignPatterns.Behavioral.StrategyPattern;
using DesignPatterns.Behavioral.TemplatePattern;
using DesignPatterns.Behavioral.VisitorPattern;

namespace DesignPatterns.Behavioral
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            ChainOfResponsibilityV2.Invoke();

            ChainOfResponsibility.Invoke();

            Interpreter.Invoke();

            Iterator.Invoke();

            Mediator.Invoke();

            MediatorV2.Invoke();

            Memento.Invoke();

            Observer.Invoke();

            State.Invoke();

            Strategy.Invoke();

            Template.Invoke();

            Visitor.Invoke();

            Console.ReadLine();
        }
    }
}
