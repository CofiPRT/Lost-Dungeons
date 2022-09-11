using System;
using Character.Implementation.Ally;
using Game;
using Game.Screens;
using UnityEngine;
using UnityEngine.UI;

namespace CameraScript {
    public class TopdownViewController : MonoBehaviour {
        private const float PopInFunctionParameter = -2; // less or equal to -1
        private const float PopInSpeed = 5f;
        private const float ArrowSpeed = 1f;
        private const float JiggleCount = 5f;
        private const float JiggleSpeed = 5f;
        private const float ArrowFullDuration = 0.6f; // normalized time

        private static readonly Vector3 TopDownOffset = new Vector3(-10, 30, -20);

        private static TopdownViewController Instance { get; set; }

        private Button applyButton;
        private CanvasGroup applyCanvasGroup;
        private Button invalidButton;
        private Button cancelButton;
        private RectTransform cancelRect;
        private CanvasGroup arrowImage;
        private Image pathBlockImage;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }

            Instance = this;

            applyButton = transform.Find("DefendApplyButton").GetComponent<Button>();
            applyCanvasGroup = applyButton.GetComponent<CanvasGroup>();
            invalidButton = transform.Find("DefendInvalidButton").GetComponent<Button>();
            cancelButton = transform.Find("DefendCancelButton").GetComponent<Button>();
            cancelRect = cancelButton.GetComponent<RectTransform>();
            arrowImage = transform.Find("DefendArrowIcon").GetComponent<CanvasGroup>();
            pathBlockImage = transform.Find("DefendPathBlockIcon").GetComponent<Image>();

            // set all to inactive
            applyButton.gameObject.SetActive(false);
            invalidButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            arrowImage.gameObject.SetActive(false);
            pathBlockImage.gameObject.SetActive(false);
        }

        public static bool Activate() {
            var player = GameController.ControlledPlayer;
            if (player.CastBlocksAbilityUsage)
                return false;

            // move camera to topdown view - above the player and relative to the camera's orientation
            var playerPos = player.Pos;

            // offset relative to player look direction
            var cameraPos = playerPos +
                            CameraController.Right * TopDownOffset.x +
                            Vector3.up * TopDownOffset.y +
                            CameraController.Forward * TopDownOffset.z;
            var cameraRot = Quaternion.LookRotation(playerPos - cameraPos, Vector3.up);

            CameraController.SetCustomTarget(cameraPos, cameraRot.eulerAngles, usePlayerForCollision: false);
            ScreenController.AddScreen(new TopDownScreen());
            GameController.ShowCursor();

            return true;
        }

        public static void Deactivate() {
            Instance.DeselectAlly();
            CameraController.SetFollowPlayer();
            ScreenController.RemoveScreen();
            GameController.HideCursor();
        }

        private GenericAlly selectedAlly;

        public static void OnMouseClick() {
            if (Instance.selectedAlly != null)
                return;

            // raycast to see if we clicked on an ally
            var mousePos = Input.mousePosition;
            var ray = CameraController.Cam.ScreenPointToRay(mousePos);

            if (!Physics.Raycast(ray, out var hit, 1000, LayerMask.GetMask("Ally", "Player"))) return;

            var ally = hit.collider.GetComponent<GenericAlly>();

            // cannot select non AI character
            if (ally == GameController.ControlledPlayer)
                return;

            // set ally
            if (ally != null)
                Instance.SelectAlly(ally, mousePos);
        }

        public static void OnRightClick() {
            Instance.DeselectAlly();
        }

        private void SelectAlly(GenericAlly ally, Vector3 mousePosition) {
            selectedAlly = ally;
            cancelButton.transform.position = mousePosition;
            GameController.HideCursor(false);
        }

        private void DeselectAlly() {
            selectedAlly = null;
            GameController.ShowCursor();
        }

        public void OnCancelButtonPressed() {
            selectedAlly.DefendPosition = null;
            DeselectAlly();
        }

        public void OnApplyButtonPressed() {
            selectedAlly.DefendPosition = Instance.defendPosition;
            DeselectAlly();
        }

        private float jiggleCoefficient;

        public void OnInvalidButtonPressed() {
            jiggleCoefficient = 1;
        }

        public static void OnEscapePressed() {
            if (Instance.selectedAlly == null)
                Deactivate();
            else
                Instance.DeselectAlly();
        }

        private Vector3 defendPosition;
        private float targetApplyButtonAlpha;

        private void Update() {
            pathBlockImage.gameObject.SetActive(false);
            arrowImage.gameObject.SetActive(false);

            // set the interactability of the buttons
            var interactable = popInCoefficient >= 1;
            applyButton.interactable = interactable;
            invalidButton.interactable = interactable;
            cancelButton.interactable = interactable;

            if (selectedAlly == null) {
                PopInButtons(true);
                return;
            }

            PopInButtons();

            var mousePos = Input.mousePosition;

            // set button positions
            applyButton.transform.position = mousePos;
            invalidButton.transform.position = mousePos;

            var allyMousePos = CameraController.Cam.WorldToScreenPoint(selectedAlly.transform.position);
            cancelButton.transform.position = allyMousePos;

            // cancel button is active only if the ally is already defending
            cancelButton.gameObject.SetActive(selectedAlly.DefendPosition != null);

            // the apply button should be visible only when cursor is not in the bounds of the cancel button
            targetApplyButtonAlpha = !cancelButton.IsActive() ||
                                     !RectTransformUtility.RectangleContainsScreenPoint(cancelRect, mousePos)
                ? 1
                : 0;

            // lerp the alpha
            applyCanvasGroup.alpha = Mathf.Lerp(
                applyCanvasGroup.alpha,
                targetApplyButtonAlpha,
                Time.deltaTime * PopInSpeed
            );

            // raycast to terrain
            var ray2 = CameraController.Cam.ScreenPointToRay(mousePos);
            var hitSuccess = Physics.Raycast(
                ray2,
                out var hitResult,
                1000,
                LayerMask.GetMask("Terrain")
            );

            if (!hitSuccess) {
                // if no terrain was found, show invalid button
                ShowInvalidButton();
                return;
            }

            if (!selectedAlly.CanSee(hitResult.point, out var hitBlock)) {
                // invalid defend position
                ShowInvalidButton();

                var blockPosMouse = CameraController.Cam.WorldToScreenPoint(hitBlock.point);
                pathBlockImage.gameObject.SetActive(true);
                pathBlockImage.transform.position = blockPosMouse;
                UpdateArrow(blockPosMouse);
                return;
            }

            if (Math.Abs(targetApplyButtonAlpha - 1) < 0.01)
                UpdateArrow(mousePos);

            // valid defend position, show apply button
            ShowApplyButton();

            defendPosition = hitResult.point;
        }

        private void ShowApplyButton() {
            applyButton.gameObject.SetActive(true);
            invalidButton.gameObject.SetActive(false);
            jiggleCoefficient = 0;
        }

        private void ShowInvalidButton() {
            applyButton.gameObject.SetActive(false);
            invalidButton.gameObject.SetActive(true);

            // update jiggle according to sine curve
            var deviation = Mathf.Sin(jiggleCoefficient * JiggleCount * Mathf.PI) * 50f;

            // add deviation horizontally
            invalidButton.transform.position += new Vector3(deviation, 0, 0);

            // update jiggle coefficient
            jiggleCoefficient = Mathf.Clamp01(jiggleCoefficient - Time.deltaTime * JiggleSpeed);
        }

        private float popInCoefficient;

        private void PopInButtons(bool reverse = false) {
            if (popInCoefficient <= 0 && reverse)
                return;

            if (popInCoefficient >= 1 && !reverse)
                return;

            popInCoefficient += PopInSpeed * Time.deltaTime * (reverse ? -1 : 1);
            popInCoefficient = Mathf.Clamp(popInCoefficient, 0, 1);

            var scaleCoefficient = PopInFunction(popInCoefficient);
            var scale = new Vector3(scaleCoefficient, scaleCoefficient, scaleCoefficient);

            applyButton.transform.localScale = scale;
            invalidButton.transform.localScale = scale;
            cancelButton.transform.localScale = scale;
        }

        // y = -ax^2 + (1-a)x, where a = PopInFunctionParameter
        private static float PopInFunction(float x) {
            return PopInFunctionParameter * x * x + (1 - PopInFunctionParameter) * x;
        }

        private float arrowMoveCoefficient;

        private void UpdateArrow(Vector3 mousePos) {
            arrowImage.gameObject.SetActive(true);

            if (arrowMoveCoefficient >= 1)
                arrowMoveCoefficient = 0;

            arrowMoveCoefficient += Time.deltaTime * ArrowSpeed;

            // set arrow opacity
            arrowImage.alpha = ArrowAlphaFunction(arrowMoveCoefficient);

            // set arrow position - move from the ally to the destination
            var arrowTransform = arrowImage.transform;
            var allyMousePos = CameraController.Cam.WorldToScreenPoint(selectedAlly.transform.position);
            arrowTransform.position = Vector3.Lerp(allyMousePos, mousePos, arrowMoveCoefficient);

            // set rotation
            var direction = mousePos - allyMousePos;
            arrowTransform.up = direction;
        }

        // y = (-2 * |x - 0.5| + 1) / (1 - a); where a = ArrowFullDuration
        // e.g.: for a = 0.8, full opacity is achieved between 0.1 and 0.9 normalized time
        private static float ArrowAlphaFunction(float x) {
            return (-2 * Mathf.Abs(x - 0.5f) + 1) / (1 - ArrowFullDuration);
        }
    }
}