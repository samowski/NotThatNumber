using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NotThatNumber;


public class GameControl : MonoBehaviour
{
	Game game = new Game(new MemoryHighScore());
	Game.State state;

	Button newRoundButton;

	Text highscoreName;
	Text highscoreScore;

	Text score;

	Button[] numberButtons = new Button[4];

	Color disabledColor;

	Button yesButton;
	Button noButton;
	Animator continueButtonAnimator;

	InputField nameInput;

	bool isAnimating = false;

	void Start()
	{
		state = Game.State.HaveRoundEnd;

		newRoundButton = GameObject.Find("NewRoundButton").GetComponent<Button>();

		highscoreName = GameObject.Find("Highscore/Name").GetComponent<Text>();
		highscoreScore = GameObject.Find("Highscore/Score").GetComponent<Text>();
		setHighscoreLabel();

		score = GameObject.Find("Score/Score").GetComponent<Text>();

		for (int i = 0; i < numberButtons.Length; i++)
		{
			GameObject button = GameObject.Find("Number" + (i + 1).ToString());
			numberButtons[i] = button.GetComponent<Button>();
			numberButtons[i].interactable = false;
		}

		disabledColor = numberButtons[0].colors.disabledColor;

		yesButton = GameObject.Find("YesButton").GetComponent<Button>();
		noButton = GameObject.Find("NoButton").GetComponent<Button>();
		yesButton.interactable = false;
		noButton.interactable = false;
		continueButtonAnimator = GameObject.Find("ContinueButtons").GetComponent<Animator>();

		nameInput = GameObject.Find("NameInput").GetComponent<InputField>();
		nameInput.interactable = false;

		//controlState();
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
			

			continueButtonAnimator.SetBool("visible", false);

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

	void controlState()
	{
		if (state == Game.State.NeedNumber || state == Game.State.NeedName || state == Game.State.NeedContinueRound || state == Game.State.HaveRoundEnd)
		{
			state = game.Advance();
		} 

		while (true)
		{
			if (state == Game.State.NeedNumber || state == Game.State.NeedName || state == Game.State.NeedContinueRound || state == Game.State.HaveRoundEnd)
			{
				if (state == Game.State.NeedNumber)
				{
					foreach (var button in numberButtons)
					{
						button.interactable = true;
						setButtonColor(button, disabledColor);

					}
				}

				if (state == Game.State.NeedContinueRound)
				{
					score.text = game.Points.ToString();

					yesButton.interactable = true;
					noButton.interactable = true;

					continueButtonAnimator.SetBool("visible", true);
				}

				if (state == Game.State.NeedName)
				{
					nameInput.interactable = true;
					nameInput.text = "";
					nameInput.Select();
					nameInput.ActivateInputField();
				}

				if (state == Game.State.HaveRoundEnd)
				{
					score.text = "0";

					newRoundButton.interactable = true;
				}

				break;
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
						Debug.Log(game.ThatNumber);


						isAnimating = true;

						StartCoroutine(wait());

						break;
					}

				}

				state = game.Advance();
			}

		}
	}

	IEnumerator wait()
	{
		int prevRand = -1;

		for (int i = 0; i < 5;)
		{
			int rand = Random.Range(0, 3);
			if (rand == prevRand)
			{
				prevRand = rand;
				continue;
			}
			yield return new WaitForSeconds((i^2) * 0.1f);
			markNumberButton(numberButtons[rand], Color.grey, disabledColor);
			prevRand = rand;
			i++;
		}

		yield return new WaitForSeconds((5^2) * 0.1f);

		markNumberButton(numberButtons[game.ThatNumber - 1], game.Number == game.ThatNumber ? Color.red : Color.green, disabledColor);

		controlState();
	}

	void markNumberButton(Button markButton, Color markColor, Color unmarkColor)
	{
		foreach (var button in numberButtons)
		{
			if (button == markButton)
			{
				setButtonColor (button, markColor);
			} 
			else
			{
				setButtonColor (button, unmarkColor);
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

		if (Input.GetButtonUp("Number2"))
		{
			selectNumber(2);	
		}

		if (Input.GetButtonUp("Number3"))
		{
			selectNumber(3);	
		}

		if (Input.GetButtonUp("Number4"))
		{
			selectNumber(4);	
		}

		if (Input.GetButtonUp("ContinueYes"))
		{
			selectContinue(true);	
		}

		if (Input.GetButtonUp("ContinueNo"))
		{
			selectContinue(false);	
		}
	}
}
