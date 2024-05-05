using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.Behavioral.TemplatePattern
{
    /// <summary>
    /// Allows subclasses to redefine the steps(abstract or virtual methods) of algorithm without changing the algo structure
    /// </summary>
    class Template
    {
        public static void Invoke()
        {
            var chess = new ChessGame();
            chess.Run(); //Template Mehtod
        }
    }


    abstract class Game
    {
        protected readonly int numberofPlayers;

        public Game(int numberofPlayers)
        {
            this.numberofPlayers = numberofPlayers;
        }

        public void Run()
        {
            Start();
            while (!HasWinner)
                TakeTurn();
            Console.WriteLine($"The winning player is {WinningPlayer}");
            End();
        }

        protected abstract void Start();

        protected virtual void End()
        {
            Console.WriteLine("Game Ended");
        }
        protected abstract void TakeTurn();
        protected abstract bool HasWinner { get; }
        protected abstract int WinningPlayer { get; }
        protected abstract int CurrentPlayer { get; }
    }

    class ChessGame : Game
    {
        public ChessGame() : base(2)
        {
        }

        protected override bool HasWinner => CurrentTurn == MaxTurn;
        protected override int WinningPlayer => CurrentPlayer;
        protected override int CurrentPlayer => (CurrentTurn % numberofPlayers) == 0 ? numberofPlayers : (CurrentTurn % numberofPlayers);

        protected override void Start()
        {
            Console.WriteLine($"Welcome to Chess Game.Number of players {numberofPlayers}");
        }

        protected override void End()
        {
            Console.Write("Chess ");
            base.End();
        }

        protected override void TakeTurn()
        {
            CurrentTurn++;
            Console.WriteLine($"Player {CurrentPlayer}'s Turn: Turn number {CurrentTurn}");
        }


        private int CurrentTurn = 0;
        private int MaxTurn => 10;
    }
}
