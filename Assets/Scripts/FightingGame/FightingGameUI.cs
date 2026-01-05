using Animancer;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class FightingGameUI : MonoBehaviour
{
	[Title("References")]
	[SerializeField]
	private MatchupManager _matchupManager;

	[SerializeField]
	private AnimancerComponent _animancer;

	[SerializeField]
	private Image _popUpImage;

	[SerializeField]
	private Image _screenImage;

	[Title("Animations")]
	[SerializeField]
	private AnimationClip _introAnimation;

	[SerializeField]
	private AnimationClip _popUpAnimation;

	[SerializeField]
	private AnimationClip _screenAnimation;

	[SerializeField]
	private AnimationClip _screenOutAnimation;

	[SerializeField]
	private AnimationClip _countDownAnimation;

	[Title("Sprites")]
	[SerializeField]
	private Sprite _losePopUpSprite;

	[SerializeField]
	private Sprite _winPopUpSprite;

	[SerializeField]
	private Sprite _drawPopUpSprite;

	[SerializeField]
	private Sprite _loseScreenSprite;

	[SerializeField]
	private Sprite _winScreenSprite;

	[HideInInspector]
	public UnityEvent OnIntroStart;

	[HideInInspector]
	public UnityEvent OnIntroEnd;

	private AnimancerState _activeState;

	private void OnEnable()
	{
		if (_matchupManager != null)
		{
			_matchupManager.OnRoundWin.AddListener(OnRoundWin);
			_matchupManager.OnRoundLose.AddListener(OnRoundLose);
			_matchupManager.OnRoundDraw.AddListener(OnRoundDraw);

			_matchupManager.OnGameWin.AddListener(OnGameWin);
			_matchupManager.OnGameLose.AddListener(OnGameLose);

			_matchupManager.OnNewFight.AddListener(PlayIntroAnimation);
		}
	}

	private void OnDisable()
	{
		if (_matchupManager != null)
		{
			_matchupManager.OnRoundWin.RemoveListener(OnRoundWin);
			_matchupManager.OnRoundLose.RemoveListener(OnRoundLose);
			_matchupManager.OnRoundDraw.RemoveListener(OnRoundDraw);

			_matchupManager.OnGameWin.RemoveListener(OnGameWin);
			_matchupManager.OnGameLose.RemoveListener(OnGameLose);

			_matchupManager.OnNewFight.RemoveListener(PlayIntroAnimation);
		}
	}

	private void Start()
	{
		PlayIntroAnimation();
	}

	private void OnRoundWin()
	{
		if (_winPopUpSprite)
		{
			_popUpImage.sprite = _winPopUpSprite;
		}

		PlayPopUpAnimation();
	}

	private void OnRoundLose()
	{
		if (_losePopUpSprite)
		{
			_popUpImage.sprite = _losePopUpSprite;
		}

		PlayPopUpAnimation();
	}

	private void OnRoundDraw()
	{
		if (_drawPopUpSprite)
		{
			_popUpImage.sprite = _drawPopUpSprite;
		}

		PlayPopUpAnimation();
	}

	private void OnGameWin()
	{
		GameManager.Instance.PreventCursorHide = true;
		GameManager.Instance.ShowCursor();

		if (_winScreenSprite)
		{
			_screenImage.sprite = _winScreenSprite;
		}

		PlayScreenAnimation();
	}

	private void OnGameLose()
	{
		GameManager.Instance.PreventCursorHide = true;
		GameManager.Instance.ShowCursor();

		if (_loseScreenSprite)
		{
			_screenImage.sprite = _loseScreenSprite;
		}

		PlayScreenAnimation();
	}

	private void PlayIntroAnimation()
	{
		if (_introAnimation != null)
		{
			_activeState = _animancer.Play(_introAnimation);
			_activeState.Events(this).OnEnd = () =>
			{
				OnIntroEnd?.Invoke();
				PlayCountDownAnimation();
			};

			OnIntroStart?.Invoke();
		}
	}

	private void PlayPopUpAnimation()
	{
		if (_popUpAnimation != null)
		{
			_activeState = _animancer.Play(_popUpAnimation);
			_activeState.Events(this).OnEnd = () =>
			{
				_activeState.Stop();
				_activeState = null;
			};
		}
	}

	private void PlayScreenAnimation()
	{
		if (_screenAnimation != null)
		{
			_activeState = _animancer.Play(_screenAnimation);
			_activeState.Events(this).OnEnd = () =>
			{
				_activeState.Stop();
				_activeState = null;
			};
		}
	}

	private void PlayScreenOutAnimation()
	{
		if (_screenOutAnimation != null)
		{
			_activeState = _animancer.Play(_screenOutAnimation);
			_activeState.Events(this).OnEnd = () =>
			{
				_activeState.Stop();
				_activeState = null;
				_matchupManager.Reset();
				PlayIntroAnimation();
			};
		}
	}

	private void PlayCountDownAnimation()
	{
		if (_countDownAnimation != null)
		{
			_activeState = _animancer.Play(_countDownAnimation);
			_activeState.Events(this).OnEnd = () =>
			{
				_activeState.Stop();
				_activeState = null;
			};

			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Countdown_Sfx);
		}
	}

	public void RetryButtonPress()
	{
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonClick_Sfx);
		AudioManager.Instance.SwitchMusicTrack(FMODEvents.Instance.FightingGame_Bgm);
		GameManager.Instance.PreventCursorHide = false;
		PlayScreenOutAnimation();
		GameManager.Instance.HideCursor();
	}

	public void MenuButtonPress()
	{
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonClick_Sfx);
		GameManager.Instance.PreventCursorHide = false;
		GameManager.Instance.SwitchScenes(GameManager.SceneNames.MainMenu);
	}

	public void PlayFightingGameMusic()
	{
		AudioManager.Instance.SwitchMusicTrack(FMODEvents.Instance.FightingGame_Bgm, true);
	}
}
