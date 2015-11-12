using System;
using System.IO;

namespace NotThatNumber
{
  abstract class HighScore
  {
    public uint Score { get; protected set; }

    public string Name { get; protected set; }

    abstract public void Set(uint Score, string Name);
  }

  // Saves the highscore in Memory, lost after restart:

  class MemoryHighScore : HighScore
  {
    public MemoryHighScore()
    {      
    }

    public override void Set(uint Score, string Name)
    {
      this.Score = Score;
      this.Name = Name;
    }
  }

  // Uses a file to load and save the highscore:

  class FileHighScore : HighScore
  {
    string path;

    public FileHighScore(string Path)
    {
      path = Path;

      if (File.Exists(path))
      {
        //read file:

        var file = new StreamReader(path);
        Score = Convert.ToUInt32(file.ReadLine());
        Name = file.ReadLine();
        file.Close();
      }
      else
      {
        File.WriteAllText(path, "0" + Environment.NewLine + Environment.NewLine);  // create file
        Score = 0;
        Name = "";
      }
    }

    public override void Set(uint Score, string Name)
    {
      this.Score = Score;
      this.Name = Name;

      File.WriteAllText(path, Score.ToString() + Environment.NewLine + Name);
    }
  }
}

