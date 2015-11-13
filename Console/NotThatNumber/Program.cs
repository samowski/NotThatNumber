using System;
using NotThatNumber;

namespace NotThatNumberConsoleVersion
{
  class Program
  {
    static string getHighScoreName()
    {
      string input;

      Console.Write("Neuer Highscore! ");

      do
      {
        Console.Write("Name: ");
        input = Console.ReadLine();
      }
      while(input == "");

      return input;
    }

    static uint getNumber()
    {
      bool validInput = false;
      uint number;
      string input;

      do
      {
        Console.Write("Zahl von 1 bis 4 eingeben: ");
        input = Console.ReadKey().KeyChar.ToString();//Console.ReadLine();
        Console.WriteLine();

        if (!UInt32.TryParse(input, out number))
        {
          continue;
        }

        validInput = number > 0 && number < 5;
      }
      while (!validInput);

      return number;
    }

    static bool getContinue(string message)
    {
      Console.Write(message + "[J/N]");

      while (true)
      {
        var key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.J)
        {
          Console.WriteLine();
          return true;
        }
        else if (key == ConsoleKey.N)
        {
          Console.WriteLine();
          return false;
        }
      }
    }

    static void Main()
    {
      Game game = new Game(new FileHighScore("highscore.txt"));

      bool isGameRunning = true;

      while (isGameRunning)
      {
        switch (game.Advance())
        {
          case Game.State.NeedName:
            game.Name = getHighScoreName();
            break;
          case Game.State.NeedNumber:
            game.Number = getNumber();
            break;
          case Game.State.NeedContinueRound:
            game.ContinueRound = getContinue(String.Format("[Score {0}] weiter Spielen? ", game.Score));
            break;
          case Game.State.HaveRoundEnd:
            isGameRunning = getContinue("Neue Runde? ");
            break;
          case Game.State.HaveRoundStart:
            string name = game.Highscore.Name;
            Console.WriteLine("Highscore [{0} : {1}]", string.IsNullOrEmpty(name) ? "Niemand" : name, game.Highscore.Score);
            break;
          case Game.State.HaveThatNumber:
            Console.WriteLine("Die Zahl {0} wurde gezogen.{1}", game.ThatNumber, game.Number == game.ThatNumber ? " Runde verloren" : "");
            break;
          case Game.State.HaveHighScore:
            break;
          default:
            Console.WriteLine("unkown error");
            isGameRunning = false;
            break;
        }
      }
    }
  }
}
