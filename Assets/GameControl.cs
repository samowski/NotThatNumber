using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NotThatNumber;

public class GameControl : MonoBehaviour
{
  Game game = new Game(new MemoryHighScore());
  Game.State state = Game.State.HaveRoundEnd;

  Button newRoundButton;

  Text highscoreName;
  Text highscoreScore;

  Text score;

  Button[] numberButtons = new Button[4];

  Color disabledColor;

  Button yesButton;
  Button noButton;

  Animator continueAnimator;

  InputField nameInput;

  bool isAnimating = false;

  void Start()
  {
    newRoundButton = GameObject.Find("NewRoundButton").GetComponent<Button>();

    highscoreName = GameObject.Find("Highscore/Name").GetComponent<Text>();
    highscoreScore = GameObject.Find("Highscore/Score").GetComponent<Text>();
		
    setHighscoreLabel();

    score = GameObject.Find("Score/Score").GetComponent<Text>();

    for (int i = 0; i < numberButtons.Length; i++)
    {
      var button = GameObject.Find("Number" + (i + 1).ToString());
      numberButtons[i] = button.GetComponent<Button>();
      numberButtons[i].interactable = false;
    }

    disabledColor = numberButtons[0].colors.disabledColor;

    yesButton = GameObject.Find("YesButton").GetComponent<Button>();
    yesButton.interactable = false;

    noButton = GameObject.Find("NoButton").GetComponent<Button>();
    noButton.interactable = false;

    continueAnimator = GameObject.Find("Continue").GetComponent<Animator>();

    nameInput = GameObject.Find("NameInput").GetComponent<InputField>();
    nameInput.interactable = false;
  }

  void setHighscoreLabel()
  {
    string name = game.Highscore.Name;
    highscoreName.text = string.IsNullOrEmpty(name) ? "---" : name;
    highscoreScore.text = game.Highscore.Score.ToString();
  }

  public void selectNumber(int number)
  {
    if (state == Game.State.NeedNumber)
    {
      game.Number = (uint)number;

      foreach (var button in numberButtons)
      {
        button.interactable = false;
      }

      controlState();
    }
  }

  public void selectContinue(bool selection)
  {
    if (state == Game.State.NeedContinueRound)
    {
      game.ContinueRound = selection;

      yesButton.interactable = false;
      noButton.interactable = false;
			
      continueAnimator.SetBool("visible", false);

      controlState();
    }
  }

  public void selectName()
  {
    if (state == Game.State.NeedName)
    {
      game.Name = nameInput.text;

      nameInput.interactable = false;
      nameInput.text = "";

      controlState();
    }
  }

  public void newRound()
  {
    if (state == Game.State.HaveRoundEnd)
    {
      newRoundButton.interactable = false;

      controlState();
    }
  }

  void handleNeedNumber()
  {
    foreach (var button in numberButtons)
    {
      button.interactable = true;
      setButtonColor(button, disabledColor);
    }
  }

  void handleNeedContinueRound()
  {
    score.text = game.Score.ToString();

    yesButton.interactable = true;
    noButton.interactable = true;

    continueAnimator.SetBool("visible", true);
  }

  void handleNeedName()
  {
    nameInput.interactable = true;
    nameInput.text = "";
    nameInput.Select();
    nameInput.ActivateInputField();
  }

  void handleHaveRoundEnd()
  {
    score.text = "0";

    newRoundButton.interactable = true;
  }

  bool isBlockingState(Game.State state)
  {
    return state == Game.State.NeedNumber ||
    state == Game.State.NeedName ||
    state == Game.State.NeedContinueRound ||
    state == Game.State.HaveRoundEnd;
  }

  void controlState()
  {
    if (isBlockingState(state))
    {
      state = game.Advance();
    } 

    while (true)
    {
      if (isBlockingState(state))
      {
        if (state == Game.State.NeedNumber)
        {
          handleNeedNumber();
        }
        else if (state == Game.State.NeedContinueRound)
        {
          handleNeedContinueRound();
        }
        else if (state == Game.State.NeedName)
        {
          handleNeedName();
        }
        else if (state == Game.State.HaveRoundEnd)
        {
          handleHaveRoundEnd();
        }
        return;
      }
      else
      {
        if (state == Game.State.HaveHighScore)
        {
          setHighscoreLabel();
        }
        else if (state == Game.State.HaveThatNumber)
        {
          if (isAnimating)
          {
            isAnimating = false;
          }
          else
          {
            isAnimating = true;

            StartCoroutine(playThatNumberSelectionAnimation());

            return;
          }
        }

        state = game.Advance();
      }
    }
  }

  IEnumerable<int> randomSequence()
  {
    int previousNumber = -1;

    for (int i = 0; i < 5;)
    {
      int randomNumber = Random.Range(0, 3);

      if (randomNumber == previousNumber)
      {
        previousNumber = randomNumber;
        continue;
      }

      yield return randomNumber;

      previousNumber = randomNumber;

      i++;
    }
  }

  IEnumerator playThatNumberSelectionAnimation()
  {
    uint index = uint.MaxValue;

    foreach (int randomNumber in randomSequence())
    {
      index++;

      yield return new WaitForSeconds((index ^ 2) * 0.1f);
			
      markNumberButton(numberButtons[randomNumber], Color.grey, disabledColor);
    }

    yield return new WaitForSeconds((5 ^ 2) * 0.1f);

    markNumberButton(numberButtons[game.ThatNumber - 1], game.Number == game.ThatNumber ? Color.red : Color.green, disabledColor);

    controlState();
  }

  void markNumberButton(Button markButton, Color markColor, Color unmarkColor)
  {
    foreach (var button in numberButtons)
    {
      if (button == markButton)
      {
        setButtonColor(button, markColor);
      }
      else
      {
        setButtonColor(button, unmarkColor);
      }
    }
  }

  static void setButtonColor(Button button, Color color)
  {
    ColorBlock colors = button.colors;
    colors.disabledColor = color;
    button.colors = colors;
  }

  void Update()
  {
    if (Input.GetButtonUp("NewGame"))
    {
      newRound();	
    }

    if (Input.GetButtonUp("Number1"))
    {
      selectNumber(1);	
    }
    else if (Input.GetButtonUp("Number2"))
    {
      selectNumber(2);	
    }
    else if (Input.GetButtonUp("Number3"))
    {
      selectNumber(3);	
    }
    else if (Input.GetButtonUp("Number4"))
    {
      selectNumber(4);	
    }

    if (Input.GetButtonUp("ContinueYes"))
    {
      selectContinue(true);	
    }
    else if (Input.GetButtonUp("ContinueNo"))
    {
      selectContinue(false);	
    }
  }
}
