using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.InputSystem;  // Remove this line
using KalponicStudio.Health;
using KalponicStudio.Health.UI;

namespace KalponicStudio.Health.Examples
{
    /// <summary>
    /// Demo scene setup script. Creates a complete health/combat demonstration.
    /// Attach to an empty GameObject in your demo scene.
    /// </summary>
    public class DemoSetup : MonoBehaviour
    {
        [Header("Demo Objects")]
        [SerializeField] private GameObject playerObject;
        [SerializeField] private GameObject enemyObject;
        [SerializeField] private Canvas uiCanvas;

        [Header("UI References")]
        [SerializeField] private Text demoText;

        [Header("UI Prefabs (Optional)")]
        [SerializeField] private GameObject healthBarPrefab;
        [SerializeField] private GameObject damageTextPrefab;

        private HealthSystem playerHealth;
        private HealthSystem enemyHealth;
        private ShieldSystem enemyShield;

        protected void Start()
        {
            SetupDemo();
        }

        private void SetupDemo()
        {
            // Setup Player
            if (playerObject == null)
            {
                playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                playerObject.name = "Player";
                playerObject.transform.position = new Vector3(-2, 1, 0);
            }

            playerHealth = playerObject.AddComponent<HealthSystem>();
            playerHealth.SetMaxHealth(100);
            playerHealth.SetHealth(100);

            // Add visual effects to player
            playerObject.AddComponent<HealthVisualSystem>();

            // Setup Enemy
            if (enemyObject == null)
            {
                enemyObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                enemyObject.name = "Enemy";
                enemyObject.transform.position = new Vector3(2, 1, 0);
            }

            enemyHealth = enemyObject.AddComponent<HealthSystem>();
            enemyHealth.SetMaxHealth(75);
            enemyHealth.SetHealth(75);

            // Add shield to enemy
            enemyShield = enemyObject.AddComponent<ShieldSystem>();
            enemyShield.SetMaxShield(25);
            enemyShield.SetShield(25);

            // Add status effects to enemy
            enemyObject.AddComponent<StatusEffectSystem>();

            // Setup UI if canvas exists
            if (uiCanvas != null)
            {
                SetupUI();
            }

            // Subscribe to events for demo feedback
            playerHealth.HealthChanged += OnPlayerHealthChanged;
            enemyHealth.HealthChanged += OnEnemyHealthChanged;

            Debug.Log("Demo setup complete! Use number keys 1-5 for different damage types.");
        }

        private void SetupUI()
        {
            // Create health bars for both characters
            if (healthBarPrefab != null)
            {
                // Player health bar
                GameObject playerBar = Instantiate(healthBarPrefab, uiCanvas.transform);
                playerBar.name = "PlayerHealthBar";
                RectTransform playerRect = playerBar.GetComponent<RectTransform>();
                playerRect.anchoredPosition = new Vector2(-200, -200);

                HealthBar playerHealthBar = playerBar.GetComponent<HealthBar>();
                // HealthBar connects automatically via events - no manual connection needed
                // The HealthUIController would handle this in a real setup

                // Enemy health bar
                GameObject enemyBar = Instantiate(healthBarPrefab, uiCanvas.transform);
                enemyBar.name = "EnemyHealthBar";
                RectTransform enemyRect = enemyBar.GetComponent<RectTransform>();
                enemyRect.anchoredPosition = new Vector2(200, -200);

                HealthBar enemyHealthBar = enemyBar.GetComponent<HealthBar>();
                // HealthBar connects automatically via events - no manual connection needed
            }
        }

        protected void Update()
        {
            // Demo controls
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerHealth.TakeDamage(10);
                Debug.Log("Player took 10 damage");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                enemyHealth.TakeDamage(15);
                Debug.Log("Enemy took 15 damage (shield absorbs first)");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                playerHealth.Heal(20);
                Debug.Log("Player healed 20 health");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                // Apply poison to enemy
                if (enemyObject.TryGetComponent(out StatusEffectSystem enemyStatus))
                {
                    enemyStatus.ApplyPoison(5f, 3);
                    Debug.Log("Applied poison to enemy (5 seconds, 3 damage/sec)");
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                // Save/Load demo - requires Extensions assembly
                // HealthSnapshotComponent snapshot = playerObject.AddComponent<HealthSnapshotComponent>();
                // snapshot.CaptureSnapshot();
                Debug.Log("Save/Load demo - add HealthSnapshotComponent in Extensions for full functionality");
            }

            // Update demo text if assigned
            if (demoText != null)
            {
                demoText.text = "KS Health & Combat System Demo\n\n" +
                    "Controls:\n" +
                    "1: Damage Player (-10)\n" +
                    "2: Damage Enemy (-15, shield first)\n" +
                    "3: Heal Player (+20)\n" +
                    "4: Poison Enemy (5s, 3/sec)\n" +
                    "5: Save Player State\n\n" +
                    $"Player: {(playerHealth != null ? playerHealth.CurrentHealth : 0)}/{(playerHealth != null ? playerHealth.MaxHealth : 0)}\n" +
                    $"Enemy: {(enemyHealth != null ? enemyHealth.CurrentHealth : 0)}/{(enemyHealth != null ? enemyHealth.MaxHealth : 0)} " +
                    $"(Shield: {(enemyShield != null ? enemyShield.CurrentShield : 0)})";
            }
        }

        private void OnPlayerHealthChanged(int current, int max)
        {
            Debug.Log($"Player Health: {current}/{max}");
        }

        private void OnEnemyHealthChanged(int current, int max)
        {
            Debug.Log($"Enemy Health: {current}/{max} (Shield: {(enemyShield != null ? enemyShield.CurrentShield : 0)})");
        }

        // private void OnGUI()
        // {
        //     // Simple demo instructions
        //     GUI.Label(new Rect(10, 10, 400, 200),
        //         "KS Health & Combat System Demo\n\n" +
        //         "Controls:\n" +
        //         "1: Damage Player (-10)\n" +
        //         "2: Damage Enemy (-15, shield first)\n" +
        //         "3: Heal Player (+20)\n" +
        //         "4: Poison Enemy (5s, 3/sec)\n" +
        //         "5: Save Player State\n\n" +
        //         $"Player: {playerHealth?.CurrentHealth ?? 0}/{(playerHealth?.MaxHealth ?? 0)}\n" +
        //         $"Enemy: {enemyHealth?.CurrentHealth ?? 0}/{(enemyHealth?.MaxHealth ?? 0)} " +
        //         $"(Shield: {enemyShield?.CurrentShield ?? 0})");
        // }
    }
}