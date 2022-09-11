using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu {
    public class MainMenuController : MonoBehaviour {
        private static readonly Dictionary<string, string[]> LevelRequirements = new Dictionary<string, string[]> {
            { "T-01", new string[] { } },
            { "T-02", new[] { "T-01" } },
            { "T-03", new[] { "T-02" } },
            { "R-01", new string[] { } },
            { "R-02", new[] { "R-01" } },
            { "R-03", new[] { "R-02" } },
            { "D-01", new[] { "T-01", "R-01" } },
            { "D-02", new[] { "D-01", "T-02", "R-02" } },
            { "D-03", new[] { "D-02", "T-03", "R-03" } }
        };

        private Transform mainButtons;
        private Button playButton;
        private RectTransform playButtonRect;
        private Transform levelSelectionButtons;
        private Transform levelResetConfirmationButtons;
        private Transform background;
        private Vector3 originalBackgroundPosition;

        private Transform activeButtons;

        private readonly Dictionary<Button, Vector3> buttonToOriginalPosition = new Dictionary<Button, Vector3>();

        private void Awake() {
            mainButtons = transform.Find("MainButtons");
            playButton = mainButtons.Find("PlayButton").GetComponent<Button>();
            playButtonRect = playButton.GetComponent<RectTransform>();
            levelSelectionButtons = transform.Find("LevelSelectionButtons");
            levelResetConfirmationButtons = transform.Find("LevelResetConfirmationButtons");
            background = transform.Find("Background");
            originalBackgroundPosition = background.localPosition;

            activeButtons = mainButtons;
            mainButtons.gameObject.SetActive(true);
            levelSelectionButtons.gameObject.SetActive(false);
            levelResetConfirmationButtons.gameObject.SetActive(false);

            AwakeLevelSelectionButtons();
            AwakeLogos();
            SetDefaults();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // fill dictionary
            foreach (var button in levelSelectionButtons.GetComponentsInChildren<Button>())
                buttonToOriginalPosition.Add(button, button.transform.localPosition);
        }

        private void ChangeActiveButtons(Transform newActiveButtons) {
            activeButtons.gameObject.SetActive(false);
            activeButtons = newActiveButtons;
            activeButtons.gameObject.SetActive(true);
        }

        /* Main Buttons */

        public void OpenLevelSelection() {
            ChangeActiveButtons(levelSelectionButtons);
            gameLogoTargetPosition = GameLogoLevelSelectionPosition;
            gameLogoTargetSize = GameLogoLevelSelectionSize;
        }

        /* Level Selection Buttons */

        private void AwakeLevelSelectionButtons() {
            foreach (var entry in LevelRequirements) {
                var levelName = entry.Key;
                var requirements = entry.Value;

                var button = levelSelectionButtons.Find(levelName).GetComponent<Button>();
                button.interactable = requirements.All(
                    requirement => PlayerPrefs.GetInt("level." + requirement + ".finished") == 1
                );
            }
        }

        public void StartT01() {
            SceneManager.LoadScene(1);
        }

        public void StartR01() {
            SceneManager.LoadScene(2);
        }

        public void StartD01() {
            SceneManager.LoadScene(3);
        }

        public void StartT02() {
            SceneManager.LoadScene(4);
        }

        public void StartR02() {
            SceneManager.LoadScene(5);
        }

        public void StartD02() {
            SceneManager.LoadScene(6);
        }

        public void StartT03() {
            SceneManager.LoadScene(7);
        }

        public void StartR03() {
            SceneManager.LoadScene(8);
        }

        public void StartD03() {
            SceneManager.LoadScene(9);
        }

        public void ToMainMenu() {
            ChangeActiveButtons(mainButtons);
            gameLogoTargetPosition = GameLogoMainPosition;
            gameLogoTargetSize = GameLogoMainSize;
        }

        public void OpenLevelResetConfirmation() {
            gameLogoTargetPosition = GameLogoResetConfirmationPosition;
            ChangeActiveButtons(levelResetConfirmationButtons);
        }

        public void ConfirmLevelReset() {
            // delete completed levels
            foreach (var entry in LevelRequirements) {
                var levelName = entry.Key;
                PlayerPrefs.SetInt("level." + levelName + ".finished", 0);
            }

            AwakeLevelSelectionButtons();
            OpenLevelSelection();
        }

        private void SetDefaults() {
            foreach (var entry in LevelRequirements) {
                var levelName = entry.Key;

                var pref = "level." + levelName + ".finished";

                if (!PlayerPrefs.HasKey(pref))
                    PlayerPrefs.SetInt(pref, 0);
            }
        }

        /* Intro Sequence */

        private CanvasGroup companyLogo;

        private CanvasGroup gameLogo;
        private CanvasGroup gameLogoGhost;

        private Image gameLogoImage;
        private Image gameLogoGhostImage;

        private CanvasGroup darkOverlay;
        private AudioSource audioSource;

        public float beatsPerMinute = 134;
        private float SecondsPerBeat => 60f / beatsPerMinute;
        private float SecondsPerBar => SecondsPerBeat * 4;

        public int companyLogoBars = 2;
        public int gameLogoBars = 2;
        public float fadeInBars = 1f;
        public float fadeOutBars = 0.5f;

        public float offsetSeconds;

        private void OnValidate() {
            // offset must be positive
            offsetSeconds = Mathf.Max(offsetSeconds, 0f);
        }

        private float companyLogoStart;
        private float companyLogoEnd;
        private float companyLogoFadeInStart;
        private float companyLogoFadeInEnd;
        private float companyLogoFadeOutStart;
        private float companyLogoFadeOutEnd;

        private float gameLogoStart;
        private float gameLogoEnd;
        private float gameLogoFadeInStart;
        private float gameLogoFadeInEnd;
        private float gameLogoFadeOutStart;
        private float gameLogoFadeOutEnd;

        private Vector3 gameLogoIntroFinalFadeScale;

        private static readonly Vector3 CompanyLogoIntroInitialScale = new Vector3(1.5f, 1.5f, 1.5f);
        private static readonly Vector3 CompanyLogoIntroFinalScale = new Vector3(1.75f, 1.75f, 1.75f);
        private static readonly Vector3 GameLogoIntroInitialScale = new Vector3(1.5f, 1.5f, 1.5f);
        private static readonly Vector3 GameLogoIntroFinalScale = new Vector3(1.75f, 1.75f, 1.75f);

        private static readonly Vector3 GameLogoMainPosition = new Vector3(0f, 0f, 0f);
        private static readonly Vector3 GameLogoMainSize = new Vector3(1f, 1f, 1f) * 1.5f;

        private static readonly Vector3 GameLogoLevelSelectionPosition = new Vector3(0f, 500f, 0f);
        private static readonly Vector3 GameLogoLevelSelectionSize = new Vector3(1f, 1f, 1f) * 0.5f;

        private static readonly Vector3 GameLogoResetConfirmationPosition = new Vector3(0f, 1000f, 0f);

        private static readonly Color ActiveColor = new Color(1f, 0.8f, 0.6f, 1f);
        private static readonly Color InactiveColor = new Color(0.5f, 0.4f, 0.3f, 1f);

        public float gameLogoBeatSizeMultiplier = 1.2f;

        private const float GameLogoPositionLerpSpeed = 5f;
        private const float GameLogoColorLerpSpeed = 5f;

        private Vector3 gameLogoTargetPosition;
        private Vector3 gameLogoTargetSize;
        private Color gameLogoTargetColor;

        private Vector3 gameLogoCurrentPosition;
        private Vector3 gameLogoCurrentSize;

        private bool hasFinishedIntro;

        private void AwakeLogos() {
            companyLogo = transform.Find("CompanyLogo").GetComponent<CanvasGroup>();

            gameLogo = transform.Find("GameLogo").GetComponent<CanvasGroup>();
            gameLogoGhost = transform.Find("GameLogo/ImageGhost").GetComponent<CanvasGroup>();

            gameLogoImage = gameLogo.transform.Find("Image").GetComponent<Image>();
            gameLogoGhostImage = gameLogoGhost.GetComponent<Image>();

            darkOverlay = transform.Find("DarkOverlay").GetComponent<CanvasGroup>();
            audioSource = GetComponent<AudioSource>();

            // initially, the overlay is fully opaque
            darkOverlay.alpha = 1f;

            /* precomputed values */

            // for the first 2 bars, show the company logo
            companyLogoStart = 0f;
            companyLogoEnd = SecondsPerBar * companyLogoBars;

            // fade in company logo
            companyLogoFadeInStart = companyLogoStart;
            companyLogoFadeInEnd = companyLogoFadeInStart + SecondsPerBar * fadeInBars;

            // fade out company logo
            companyLogoFadeOutStart = companyLogoEnd - SecondsPerBar * fadeOutBars;
            companyLogoFadeOutEnd = companyLogoEnd;

            // for the next 2 bars, show the game logo
            gameLogoStart = companyLogoEnd;
            gameLogoEnd = gameLogoStart + SecondsPerBar * gameLogoBars;

            // fade in game logo
            gameLogoFadeInStart = gameLogoStart;
            gameLogoFadeInEnd = gameLogoFadeInStart + SecondsPerBar * fadeInBars;

            // fade out game logo
            gameLogoFadeOutStart = gameLogoStart + SecondsPerBar * gameLogoBars - SecondsPerBar * fadeOutBars;
            gameLogoFadeOutEnd = gameLogoEnd;

            // intermediary scale for the game logo at the start of the fade out
            gameLogoIntroFinalFadeScale = Vector3.Lerp(
                GameLogoIntroInitialScale,
                GameLogoIntroFinalScale,
                Mathf.InverseLerp(gameLogoStart, gameLogoEnd, gameLogoFadeOutStart)
            );

            gameLogoTargetPosition = GameLogoMainPosition;
            gameLogoCurrentSize = GameLogoMainSize;
            gameLogoTargetSize = GameLogoMainSize;
            gameLogoGhost.alpha = 0f;
        }

        private void Update() {
            var time = audioSource.time - offsetSeconds;

            if (!hasFinishedIntro && time < gameLogoEnd) {
                UpdateIntro(time);
                return;
            }

            if (!hasFinishedIntro) // make the play button interactable
                playButton.interactable = true;

            hasFinishedIntro = true;

            // lerp game logo color
            var targetColor = !mainButtons.gameObject.activeSelf || playButtonRect.rect.Contains(
                playButtonRect.InverseTransformPoint(Input.mousePosition)
            )
                ? ActiveColor
                : InactiveColor;
            gameLogoImage.color = Color.Lerp(
                gameLogoImage.color,
                targetColor,
                Time.deltaTime * GameLogoColorLerpSpeed
            );
            gameLogoGhostImage.color = Color.Lerp(
                gameLogoGhostImage.color,
                targetColor,
                Time.deltaTime * GameLogoColorLerpSpeed
            );

            // lerp the game logo to the target position
            gameLogoCurrentPosition = Vector3.Lerp(
                gameLogoCurrentPosition,
                gameLogoTargetPosition,
                Time.deltaTime * GameLogoPositionLerpSpeed
            );
            // the play button follows this position
            playButton.transform.localPosition =
                gameLogo.transform.localPosition = PositionTowardsCursor(gameLogoCurrentPosition);

            // the level selection buttons also follow mouse
            foreach (var entry in buttonToOriginalPosition) {
                var button = entry.Key;
                var originalPosition = entry.Value;

                button.transform.localPosition = PositionTowardsCursor(originalPosition);
            }

            // lerp the background to follow mouse
            background.transform.localPosition = PositionTowardsCursor(originalBackgroundPosition, true);

            // lerp the game logo to the target size
            gameLogoCurrentSize = Vector3.Lerp(
                gameLogoCurrentSize,
                gameLogoTargetSize,
                Time.deltaTime * GameLogoPositionLerpSpeed
            );

            // the game logo size depends on the beat
            var beat = (int)(time / SecondsPerBeat) % 4;

            // find out where inside the current beat we are, but treat beat 1 as non-existent
            var beatProgress = Mathf.InverseLerp(
                SecondsPerBeat * Mathf.Floor(time / SecondsPerBeat),
                SecondsPerBeat * Mathf.Ceil(time / SecondsPerBeat),
                time
            );

            if (beat == 0) // if we're in beat 0, lerp from 0 to 0.5
                beatProgress = Mathf.Lerp(0f, 0.5f, beatProgress);
            else if (beat == 1) // if we're in beat 1, lerp from 0.5 to 1
                beatProgress = Mathf.Lerp(0.5f, 1f, beatProgress);
            // otherwise, keep progress

            // big size at the start of the beat, small size at the end
            var beatSize = Mathf.Lerp(gameLogoBeatSizeMultiplier, 1f, beatProgress);
            gameLogo.transform.localScale = gameLogoCurrentSize * beatSize;
            gameLogoGhost.transform.localScale = new Vector3(1f, 1f, 1f) * (gameLogoBeatSizeMultiplier + 1 - beatSize);

            // the ghost is visible at the start of the beat, and fades out until the middle of the beat
            gameLogoGhost.alpha = 1 - Mathf.InverseLerp(0f, 0.5f, beatProgress);
            // gameLogoGhost.alpha = Mathf.Lerp(1f, 0f, beatProgress);
        }

        private void UpdateIntro(float time) {
            // for the first 2 bars, show the company logo
            if (time < companyLogoFadeInStart)
                companyLogo.alpha = 0f;
            else if (time < companyLogoFadeInEnd)
                companyLogo.alpha = Mathf.InverseLerp(companyLogoFadeInStart, companyLogoFadeInEnd, time);
            else if (time < companyLogoFadeOutStart)
                companyLogo.alpha = 1f;
            else if (time < companyLogoFadeOutEnd)
                companyLogo.alpha = Mathf.InverseLerp(companyLogoFadeOutEnd, companyLogoFadeOutStart, time);
            else
                companyLogo.alpha = 0f;

            // lerp the company logo scale
            if (time > companyLogoStart && time < companyLogoEnd) {
                var t = Mathf.InverseLerp(companyLogoStart, companyLogoEnd, time);
                companyLogo.transform.localScale = Vector3.Lerp(
                    CompanyLogoIntroInitialScale,
                    CompanyLogoIntroFinalScale,
                    t
                );
            }

            // for the next 2 bars, show the game logo - we will not fade out the game logo
            if (time < gameLogoFadeInStart)
                gameLogo.alpha = 0f;
            else if (time < gameLogoFadeInEnd)
                gameLogo.alpha = Mathf.InverseLerp(gameLogoFadeInStart, gameLogoFadeInEnd, time);
            else
                gameLogo.alpha = 1f;

            // lerp the game logo scale until the start of the fade out
            if (time > gameLogoStart && time < gameLogoFadeOutStart) {
                var t = Mathf.InverseLerp(gameLogoStart, gameLogoEnd, time);
                gameLogo.transform.localScale = Vector3.Lerp(GameLogoIntroInitialScale, GameLogoIntroFinalScale, t);
            }

            // while fading out, lerp the logo to the main position and remove the dark overlay
            if (time < gameLogoFadeOutStart) {
                gameLogo.transform.localPosition = Vector3.zero;
            } else if (time < gameLogoFadeOutEnd) {
                gameLogo.transform.localPosition = Vector3.Lerp(
                    Vector3.zero,
                    GameLogoMainPosition,
                    Mathf.InverseLerp(gameLogoFadeOutStart, gameLogoFadeOutEnd, time)
                );
                darkOverlay.alpha = 1 - Mathf.InverseLerp(gameLogoFadeOutStart, gameLogoFadeOutEnd, time);
                gameLogo.transform.localScale = Vector3.Lerp(
                    gameLogoIntroFinalFadeScale,
                    GameLogoMainSize,
                    Mathf.InverseLerp(gameLogoFadeOutStart, gameLogoFadeOutEnd, time)
                );
            } else {
                gameLogo.transform.localPosition = GameLogoMainPosition;
                darkOverlay.alpha = 0f;
            }
        }

        private const float MaxDisplacement = 50f;

        private Vector3 PositionTowardsCursor(Vector3 origin, bool opposite = false) {
            var cursorPosition = Input.mousePosition; // cursor position on screen
            var screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f); // center of the screen
            var displacement = cursorPosition - screenCenter; // displacement from the center of the screen

            var distanceX = Mathf.Abs(displacement.x - origin.x);
            var distanceY = Mathf.Abs(displacement.y - origin.y);

            // lerp based on distance, max distance is achieved at screen half the size
            var xT = Mathf.InverseLerp(0f, Screen.width, distanceX);
            var yT = Mathf.InverseLerp(0f, Screen.height, distanceY);

            // find position
            var offset = Vector3.Scale(new Vector3(xT, yT, 0), (displacement - origin).normalized * MaxDisplacement);

            return origin + offset * (opposite ? -1 : 1);
        }
    }
}