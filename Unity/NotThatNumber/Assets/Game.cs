﻿using System;
using System.Collections.Generic;

namespace NotThatNumber
{
  class Game
  {
    public enum State
    {
      NeedName,
      NeedNumber,
      NeedContinueRound,
      HaveRoundStart,
      HaveRoundEnd,
      HaveHighScore,
      HaveThatNumber,
    }
      
    // output:

    public HighScore Highscore { get; private set; }

    public uint ThatNumber { get; private set; }

    public uint Score { get; private set; }

    // input:

    public bool ContinueRound = true;
    public string Name = "";
    public uint Number;

    IEnumerator<State> gameIterator;
    bool isThatNumber = false;
    Random random = new Random();

    public Game(HighScore HighScore)
    {
      gameIterator = playGame();

      this.Highscore = HighScore;
    }

    public State Advance()
    {
      gameIterator.MoveNext();
      return gameIterator.Current;
    }

    IEnumerator<State> playGame()
    {
      while (true)
      {
        yield return State.HaveRoundStart;

        Score = 0u;

        foreach (State state in playRound())
        {
          yield return state;
        }

        if (Score >= Highscore.Score && !isThatNumber)
        {
          yield return State.NeedName;

          Highscore.Set(Score, Name);

          yield return State.HaveHighScore;
        }

        yield return State.HaveRoundEnd;
      }
    }

    IEnumerable<State> playRound()
    {
      while (true)
      {
        yield return State.NeedNumber;

        ThatNumber = (uint)random.Next(1, 5);

        yield return State.HaveThatNumber;

        isThatNumber = Number == ThatNumber;

        if (isThatNumber)
        {
          break;
        }
        else
        {
          Score++;
        }

        yield return State.NeedContinueRound;

        if (!ContinueRound)
        {
          break;
        }
      }
    }
  }
}