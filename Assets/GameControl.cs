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
		state = Game.State.HaveRoundEnd;//game.Advance();

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
		Debug.Log(number.ToString() + " " + state);
		//Debug.Log(state);
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
		Debug.Log(selection);
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
		Debug.Log(nameInput.text);
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
			Debug.Log(state.ToString());

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
					setHighscoreLabel(); // design flaw with highscore update
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
		for (int i = 0; i < 5; i++)
		{
			int rand = Random.Range(0, 3);
			setButtonColor(numberButtons[rand], Color.red);
			yield return new WaitForSeconds((i^2) * 0.1f);
			setButtonColor(numberButtons[rand], disabledColor);	
		}

		yield return new WaitForSeconds((5^2) * 0.1f);

		setButtonColor(numberButtons[game.ThatNumber - 1], game.Number == game.ThatNumber ? Color.red : Color.green);

		controlState();
	}

	static void setButtonColor(Button button, Color color)
	{
		ColorBlock colors = button.colors;
		colors.disabledColor = color;
		button.colors = colors;
	}

	// Update is called once per frame
	void Update()
	{
	
	}
}
