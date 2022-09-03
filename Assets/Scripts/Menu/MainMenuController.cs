using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu {
    public class MainMenuController : MonoBehaviour {
        private static readonly Dictionary<string, string[]> LevelRequirements = new Dictionary<string, string[]>() {
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
        private Transform levelSelectionButtons;
        private Transform levelResetConfirmationButtons;

        private Transform activeButtons;

        private void Awake() {
            mainButtons = transform.Find("MainButtons");
            levelSelectionButtons = transform.Find("LevelSelectionButtons");
            levelResetConfirmationButtons = transform.Find("LevelResetConfirmationButtons");

            activeButtons = mainButtons;
            mainButtons.gameObject.SetActive(true);
            levelSelectionButtons.gameObject.SetActive(false);
            levelResetConfirmationButtons.gameObject.SetActive(false);

            AwakeLevelSelectionButtons();
            SetDefaults();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void ChangeActiveButtons(Transform newActiveButtons) {
            activeButtons.gameObject.SetActive(false);
            activeButtons = newActiveButtons;
            activeButtons.gameObject.SetActive(true);
        }

        /* Main Buttons */

        public void OpenLevelSelection() {
            ChangeActiveButtons(levelSelectionButtons);
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
        }

        public void OpenLevelResetConfirmation() {
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
    }
}