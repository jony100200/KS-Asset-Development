using UnityEngine;
using KalponicStudio.Health;
using KalponicStudio.Health.UI;

namespace KalponicStudio.Health.Extensions.UI
{
    /// <summary>
    /// World space health bar that follows a target in 3D space.
    /// Displays health information above enemies or NPCs in world space.
    /// Automatically positions and rotates to face the camera.
    /// </summary>
    public class WorldSpaceHealthBar : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("HealthSystem component to monitor. Auto-finds from parent if not set.")]
        [SerializeField] private HealthSystem healthSystem;

        [Tooltip("HealthBar UI component to update. Auto-finds from children if not set.")]
        [SerializeField] private HealthBar healthBar;

        [Tooltip("Transform to follow. Defaults to health system's transform.")]
        [SerializeField] private Transform target;

        [Header("Positioning")]
        [Tooltip("Offset from target position in world space.")]
        [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.5f, 0f);

        [Tooltip("Whether the health bar should rotate to face the camera.")]
        [SerializeField] private bool faceCamera = true;

        [Tooltip("Camera to face when faceCamera is enabled. Defaults to Camera.main.")]
        [SerializeField] private Camera worldCamera;

        private void Awake()
        {
            if (healthSystem == null)
            {
                healthSystem = GetComponentInParent<HealthSystem>();
            }

            if (healthBar == null)
            {
                healthBar = GetComponentInChildren<HealthBar>();
            }

            if (target == null && healthSystem != null)
            {
                target = healthSystem.transform;
            }

            if (worldCamera == null)
            {
                worldCamera = Camera.main;
            }
        }

        private void OnEnable()
        {
            if (healthSystem != null)
            {
                healthSystem.HealthChanged += OnHealthChanged;
            }

            RefreshUI();
        }

        private void OnDisable()
        {
            if (healthSystem != null)
            {
                healthSystem.HealthChanged -= OnHealthChanged;
            }
        }

        private void LateUpdate()
        {
            if (target != null)
            {
                transform.position = target.position + worldOffset;
            }

            if (faceCamera && worldCamera != null)
            {
                transform.forward = worldCamera.transform.forward;
            }
        }

        private void OnHealthChanged(int current, int max)
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            if (healthBar != null && healthSystem != null)
            {
                healthBar.UpdateHealth(healthSystem.CurrentHealth, healthSystem.MaxHealth);
            }
        }
    }
}
