using System;
using Character.Abilities;
using Character.Implementation.Player;
using UnityEngine;
using UnityEngine.UI;

namespace CameraScript.HUD {
    public partial class HUDController {
        private CanvasGroup rootCanvasGroup; // control alpha of entire Cooldown HUD

        private AbilityHUD[] cooldowns;

        private void AwakeCooldown() {
            var ownTransform = transform;

            rootCanvasGroup = ownTransform.Find("Cooldowns").GetComponent<CanvasGroup>();

            var abilityShield = new AbilityHUD(ownTransform.Find("Cooldowns/Shield"));
            abilityShield.SetIconUpdater(player => player.iconShield);
            abilityShield.SetInfoUpdater(
                () => Player.ShieldRechargeTime,
                () => Player.ShieldCooldown,
                () => Player.IsBlocking
            );

            var abilityDodge = new AbilityHUD(
                ownTransform.Find("Cooldowns/Dodge"),
                player => player.AbilityDodge
            );
            var abilityTagTeam = new AbilityHUD(
                ownTransform.Find("Cooldowns/TagTeam"),
                player => player.AbilityTagTeam
            );
            var ability1 = new AbilityHUD(
                ownTransform.Find("Cooldowns/Ability1"),
                player => player.Ability1
            );
            var ability2 = new AbilityHUD(
                ownTransform.Find("Cooldowns/Ability2"),
                player => player.Ability2
            );
            var ultimate = new AbilityHUD(
                ownTransform.Find("Cooldowns/Ultimate"),
                player => player.Ultimate
            );

            cooldowns = new[] {
                abilityShield,
                abilityDodge,
                abilityTagTeam,
                ability1,
                ability2,
                ultimate
            };
        }

        private void UpdateCooldown() {
            foreach (var cooldown in cooldowns)
                cooldown.Tick();
        }

        public static void LerpCooldownTransparency(float coefficient) {
            Instance.rootCanvasGroup.alpha = coefficient;
        }

        public static void RefreshIcons() {
            foreach (var cooldown in Instance.cooldowns)
                cooldown.UpdateIcon();
        }

        private class AbilityHUD {
            private const float ActiveOverlayLerpSpeed = 5f;

            private readonly Image icon;
            private readonly Image radialCooldownOverlay;
            private readonly CanvasGroup activeOverlay;
            private readonly Text timer;

            private Action<AbilityHUD> iconUpdater;
            private Action<AbilityHUD> infoUpdater;

            internal delegate float FloatGetter();

            internal delegate bool BoolGetter();

            public AbilityHUD(Transform transform, Func<GenericPlayer, IAbility> abilityGetter = null) {
                icon = transform.Find("IconMask/IconImage").GetComponent<Image>();
                radialCooldownOverlay = transform.Find("CooldownMask").GetComponent<Image>();
                activeOverlay = transform.Find("ActiveOverlayMask").GetComponent<CanvasGroup>();
                timer = transform.Find("Timer").GetComponent<Text>();

                if (abilityGetter != null)
                    SetUpdatersFromAbility(abilityGetter);
            }

            private void SetUpdatersFromAbility(Func<GenericPlayer, IAbility> abilityGetter) {
                SetIconUpdater(player => abilityGetter(player).IconGetter());
                SetInfoUpdater(
                    () => abilityGetter(Player).CooldownTime,
                    () => abilityGetter(Player).CurrentCooldown,
                    () => abilityGetter(Player).Active
                );
            }

            public void SetIconUpdater(Func<GenericPlayer, Sprite> spriteGetter) {
                iconUpdater = hud => hud.icon.sprite = spriteGetter(Player);
            }

            public void SetInfoUpdater(
                FloatGetter cooldownTimeGetter,
                FloatGetter currentCooldownGetter,
                BoolGetter activeGetter
            ) {
                infoUpdater = hud => {
                    // update radial cooldown overlay
                    hud.radialCooldownOverlay.fillAmount = currentCooldownGetter() / cooldownTimeGetter();

                    // update timer
                    var cooldown = currentCooldownGetter();

                    if (cooldown == 0)
                        hud.timer.text = "";
                    else if (cooldown < 1)
                        hud.timer.text = cooldown.ToString("0.0");
                    else
                        hud.timer.text = cooldown.ToString("0");

                    // update active state
                    hud.activeOverlay.alpha = Mathf.Lerp(
                        hud.activeOverlay.alpha,
                        activeGetter() ? 0.1f : 0f,
                        Time.deltaTime * ActiveOverlayLerpSpeed
                    );
                };
            }

            public void UpdateIcon() {
                iconUpdater(this);
            }

            public void Tick() {
                infoUpdater(this);
            }
        }
    }
}