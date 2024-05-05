using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.Behavioral.MementoPattern
{
    public class Memento
    {
        public static void Invoke()
        {
            var bankAccount = new BankAccount(100);

            Console.WriteLine(bankAccount);

            bankAccount.Deposit(330);
            Console.WriteLine(bankAccount);

            bankAccount.Deposit(499);
            Console.WriteLine(bankAccount);

            bankAccount.Withdraw(230);
            Console.WriteLine(bankAccount);

            bankAccount.Undo();
            Console.WriteLine(bankAccount);

            bankAccount.Undo();
            Console.WriteLine(bankAccount);

            bankAccount.Undo();
            Console.WriteLine(bankAccount);
        }
    }

    interface IBankAccount
    {
        public decimal Amount { get; }

        public void Deposit(decimal amount);

        public void Withdraw(decimal amount);
    }

    class BankAccount : IBankAccount
    {
        private int currentIndex = -1;

        BankAccountMemento memento = new BankAccountMemento();


        public BankAccount(decimal amount)
        {
            Deposit(amount);
        }

        public decimal Amount { get; private set; }

        public void Deposit(decimal amount)
        {
            Amount += amount;
            memento.Add(Amount);
            currentIndex++;
        }

        public void Withdraw(decimal amount)
        {
            Amount -= amount;
            memento.Add(Amount);
            currentIndex++;
        }

        public void Undo()
        {
            if (currentIndex <= 0)
                return;

            Amount = memento[--currentIndex];
        }

        public void Redo()
        {
            if (currentIndex + 1 >= memento.Count)
                return;

            Amount = memento[++currentIndex];
        }

        public override string ToString()
        {
            return $"Balance:{Amount}";
        }
    }

    internal class BankAccountMemento
    {
        public decimal this[int index]
        {
            get { return Amounts[index]; }
        }

        public List<decimal> Amounts { get; } = new List<decimal>();

        public int Count
        {
            get { return Amounts.Count; }
        }

        public void Add(decimal amount)
        {
            Amounts.Add(amount);
        }
    }
}
