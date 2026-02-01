using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace KalponicStudio.Health.Tests
{
    /// <summary>
    /// PlayMode tests for the KS Health System.
    /// Tests core functionality including shield absorption, visual effects,
    /// and integration between health, shield, and visual systems.
    /// </summary>
    public class KSHealthSystemPlayModeTests
    {
        [Test]
        public void ShieldAbsorbsBeforeHealth()
        {
            var go = new GameObject("ShieldHealthTest");
            var shield = go.AddComponent<ShieldSystem>();
            var health = go.GetComponent<HealthSystem>();

            health.SetMaxHealth(100);
            health.SetHealth(100);
            shield.SetMaxShield(50);
            shield.SetShield(50);

            health.TakeDamage(30);
            Assert.AreEqual(100, health.CurrentHealth);
            Assert.AreEqual(20, shield.CurrentShield);

            health.TakeDamage(40);
            Assert.AreEqual(80, health.CurrentHealth);
            Assert.AreEqual(0, shield.CurrentShield);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void LowHealthOverlayTogglesOnHealthChanged()
        {
            var go = new GameObject("LowHealthTest");
            var health = go.AddComponent<HealthSystem>();
            var visuals = go.AddComponent<HealthVisualSystem>();

            var canvasGo = new GameObject("Canvas", typeof(Canvas));
            var overlayGo = new GameObject("LowHealthOverlay", typeof(Image));
            overlayGo.transform.SetParent(canvasGo.transform, false);
            var overlay = overlayGo.GetComponent<Image>();

            SetPrivateField(visuals, "lowHealthOverlay", overlay);
            SetPrivateField(visuals, "lowHealthThreshold", 0.25f);

            health.SetMaxHealth(100);
            health.SetHealth(100);
            InvokePrivateMethod(visuals, "UpdateLowHealthEffect");

            health.SetHealth(20);
            InvokePrivateMethod(visuals, "UpdateLowHealthEffect");
            Assert.IsTrue(overlay.gameObject.activeSelf);

            health.SetHealth(80);
            InvokePrivateMethod(visuals, "UpdateLowHealthEffect");
            Assert.IsFalse(overlay.gameObject.activeSelf);

            Object.DestroyImmediate(go);
            Object.DestroyImmediate(canvasGo);
        }

        [Test]
        public void HealthSystemEmitsUnityAndChannelEvents()
        {
            var go = new GameObject("EventTest");
            var health = go.AddComponent<HealthSystem>();

            var channel = ScriptableObject.CreateInstance<HealthEventChannelSO>();
            SetPrivateField(health, "healthEvents", channel);

            int unityCount = 0;
            int channelCount = 0;
            health.DamageTaken += _ => unityCount++;
            channel.DamageTaken += _ => channelCount++;

            health.SetMaxHealth(100);
            health.SetHealth(100);
            health.TakeDamage(10);

            Assert.AreEqual(1, unityCount);
            Assert.AreEqual(1, channelCount);

            Object.DestroyImmediate(go);
            Object.DestroyImmediate(channel);
        }

        [Test]
        public void HealthSystemValidatesSetupCorrectly()
        {
            var go = new GameObject("ValidationTest");
            var health = go.AddComponent<HealthSystem>();

            // Test negative max health
            SetPrivateField(health, "maxHealth", -10);
            InvokePrivateMethod(health, "ValidateSetup");
            Assert.AreEqual(100, health.MaxHealth);

            // Test negative current health
            SetPrivateField(health, "currentHealth", -5);
            InvokePrivateMethod(health, "ValidateSetup");
            Assert.AreEqual(0, health.CurrentHealth);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void DamageTypesApplyResistances()
        {
            var go = new GameObject("ResistanceTest");
            var health = go.AddComponent<HealthSystem>();

            health.SetMaxHealth(100);
            health.SetHealth(100);

            // Add fire resistance
            var resistances = new KalponicStudio.Health.DamageResistance[]
            {
                new KalponicStudio.Health.DamageResistance
                {
                    type = KalponicStudio.Health.DamageType.Fire,
                    multiplier = 0.5f // 50% resistance (multiplier of 0.5 = 50% damage taken)
                }
            };
            SetPrivateField(health, "damageResistances", resistances);

            // Take fire damage - should be reduced by 50%
            var fireDamage = new KalponicStudio.Health.DamageInfo
            {
                Amount = 40,
                Type = KalponicStudio.Health.DamageType.Fire
            };

            health.TakeDamage(fireDamage);
            Assert.AreEqual(80, health.CurrentHealth); // 40 * 0.5 = 20 damage taken

            Object.DestroyImmediate(go);
        }

        [Test]
        public void HealingCapsAtMaxHealth()
        {
            var go = new GameObject("HealCapTest");
            var health = go.AddComponent<HealthSystem>();

            health.SetMaxHealth(100);
            health.SetHealth(90);

            health.Heal(20); // Should cap at 100
            Assert.AreEqual(100, health.CurrentHealth);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void SetMaxHealthAdjustsCurrentHealth()
        {
            var go = new GameObject("MaxHealthTest");
            var health = go.AddComponent<HealthSystem>();

            health.SetMaxHealth(100);
            health.SetHealth(80);

            health.SetMaxHealth(50); // Current health should be clamped to 50
            Assert.AreEqual(50, health.CurrentHealth);
            Assert.AreEqual(50, health.MaxHealth);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void InvulnerabilityPreventsDamage()
        {
            var go = new GameObject("InvulnerabilityTest");
            var health = go.AddComponent<HealthSystem>();

            health.SetMaxHealth(100);
            health.SetHealth(100);

            // Manually set invulnerability
            SetPrivateField(health, "invulnerabilityTimer", 2f);

            health.TakeDamage(30);
            Assert.AreEqual(100, health.CurrentHealth); // No damage taken

            Object.DestroyImmediate(go);
        }

        [Test]
        public void DownedStateTriggersOnLethalDamage()
        {
            var go = new GameObject("DownedTest");
            var health = go.AddComponent<HealthSystem>();

            health.SetMaxHealth(100);
            health.SetHealth(50);

            // Enable downed state
            SetPrivateField(health, "enableDownedState", true);
            SetPrivateField(health, "allowRevive", true);

            bool downedTriggered = false;
            health.Downed += () => downedTriggered = true;

            health.TakeDamage(60); // Lethal damage

            Assert.IsTrue(downedTriggered);
            Assert.IsTrue(health.IsDowned);

            Object.DestroyImmediate(go);
        }

        private static void SetPrivateField<TTarget, TValue>(TTarget target, string fieldName, TValue value)
        {
            var field = typeof(TTarget).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field, $"Field '{fieldName}' not found on {typeof(TTarget).Name}");
            field.SetValue(target, value);
        }

        private static void InvokePrivateMethod<TTarget>(TTarget target, string methodName)
        {
            var method = typeof(TTarget).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(method, $"Method '{methodName}' not found on {typeof(TTarget).Name}");
            method.Invoke(target, null);
        }
    }
}
