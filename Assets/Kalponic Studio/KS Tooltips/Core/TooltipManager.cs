using UnityEngine;

namespace KalponicStudio.Tooltips
{
    /// <summary>
    /// Generic tooltip manager that works with any UI framework.
    /// Uses dependency injection for maximum flexibility and testability.
    /// Game-agnostic - can be used for any type of game or application.
    /// </summary>
    public class TooltipManager : MonoBehaviour, ITooltipService
    {
        [Header("Configuration")]
        [Tooltip("Tooltip behavior settings")]
        [SerializeField] private TooltipConfig config = new TooltipConfig();

        [Header("Dependencies")]
        [Tooltip("Tooltip display implementation (optional - will find automatically)")]
        [SerializeField] private MonoBehaviour displayComponent;

        // Dependencies - injected or found automatically
        private ITooltipDisplay display;
        private ITooltipPositioner positioner;

        // State tracking
        private ITooltipData currentTooltipData;
        private Vector2 currentTriggerPosition;
        private float hoverStartTime;
        private float hideStartTime;
        private bool isHovering;
        private bool shouldHide;

        private void Awake()
        {
            InitializeDependencies();
            InitializePositioner();
        }

        private void Update()
        {
            HandleTooltipTiming();
            UpdateTooltipPosition(false);
        }

        /// <summary>
        /// Initializes display and positioner dependencies.
        /// Can be overridden for custom implementations.
        /// </summary>
        protected virtual void InitializeDependencies()
        {
            if (displayComponent != null && displayComponent is ITooltipDisplay typedDisplay)
            {
                display = typedDisplay;
                return;
            }
            else if (displayComponent != null)
            {
                TooltipLog.Warning("Assigned display component does not implement ITooltipDisplay.", this);
            }

            display = FindDisplayInScene();
            if (display == null)
            {
                TooltipLog.Warning("No tooltip display found in scene. Tooltips will not be displayed.", this);
                enabled = false;
            }
        }

        /// <summary>
        /// Initializes the positioner with current settings.
        /// </summary>
        private void InitializePositioner()
        {
            positioner = new MouseFollowPositioner();
            ApplyConfigToPositioner();
        }

        /// <summary>
        /// Handles tooltip show/hide timing based on hover state.
        /// </summary>
        private void HandleTooltipTiming()
        {
            if (isHovering && display != null && !display.IsVisible && Time.time - hoverStartTime >= config.ShowDelay)
            {
                ShowTooltipInternal();
            }

            if (shouldHide && Time.time - hideStartTime >= config.HideDelay)
            {
                HideTooltipInternal();
            }
        }

        /// <summary>
        /// Updates tooltip position if following mouse.
        /// </summary>
        private void UpdateTooltipPosition(bool force)
        {
            if ((!config.FollowMouse && !force) || display == null || !display.IsVisible || positioner == null) return;

            Vector2 basePosition = config.FollowMouse ? Input.mousePosition : currentTriggerPosition;
            Vector2 tooltipSize = display.GetTooltipSize();
            Vector2 newPosition = positioner.CalculatePosition(basePosition, tooltipSize, new Vector2(Screen.width, Screen.height));

            display.UpdatePosition(newPosition);
        }

        /// <summary>
        /// Shows tooltip for the specified data.
        /// </summary>
        public void ShowTooltip(ITooltipData data)
        {
            BeginShow(data, Input.mousePosition);
        }

        /// <summary>
        /// Shows tooltip for the specified data using an explicit trigger position.
        /// </summary>
        public void ShowTooltip(ITooltipData data, Vector2 triggerScreenPosition)
        {
            BeginShow(data, triggerScreenPosition);
        }

        private void BeginShow(ITooltipData data, Vector2 triggerScreenPosition)
        {
            if (data == null || !data.HasContent)
            {
                TooltipLog.Warning("Attempted to show tooltip with null or empty data.", this);
                return;
            }

            currentTooltipData = data;
            currentTriggerPosition = triggerScreenPosition;
            hoverStartTime = Time.time;
            isHovering = true;
            shouldHide = false;
        }

        /// <summary>
        /// Starts the hide process for the current tooltip.
        /// </summary>
        public void HideTooltip()
        {
            isHovering = false;
            shouldHide = true;
            hideStartTime = Time.time;
        }

        /// <summary>
        /// Immediately hides the tooltip without delay.
        /// </summary>
        public void HideTooltipImmediate()
        {
            isHovering = false;
            shouldHide = false;
            currentTooltipData = null;

            if (display != null)
            {
                display.HideTooltip();
            }
        }

        /// <summary>
        /// Actually displays the tooltip with current data.
        /// </summary>
        private void ShowTooltipInternal()
        {
            if (display == null || currentTooltipData == null) return;

            Vector2 position = config.FollowMouse ? Input.mousePosition : currentTriggerPosition;
            Vector2 tooltipSize = display.GetTooltipSize();
            Vector2 finalPosition = positioner?.CalculatePosition(position, tooltipSize, new Vector2(Screen.width, Screen.height)) ?? position;

            display.ShowTooltip(currentTooltipData, finalPosition);
        }

        /// <summary>
        /// Actually hides the tooltip.
        /// </summary>
        private void HideTooltipInternal()
        {
            currentTooltipData = null;
            if (display != null)
            {
                display.HideTooltip();
            }
        }

        /// <summary>
        /// Updates the tooltip configuration at runtime.
        /// </summary>
        public void UpdateConfiguration(TooltipConfig newConfig)
        {
            config = newConfig;
            ApplyConfigToPositioner();
        }

        /// <summary>
        /// Updates the tooltip position manually.
        /// </summary>
        public void UpdatePosition()
        {
            UpdateTooltipPosition(true);
        }

        /// <summary>
        /// Whether the tooltip is currently visible.
        /// </summary>
        public bool IsVisible => display != null && display.IsVisible;

        /// <summary>
        /// Injects a display implementation at runtime.
        /// </summary>
        public void SetDisplay(ITooltipDisplay newDisplay)
        {
            display = newDisplay;
        }

        /// <summary>
        /// Injects a positioner implementation at runtime.
        /// </summary>
        public void SetPositioner(ITooltipPositioner newPositioner)
        {
            positioner = newPositioner;
            ApplyConfigToPositioner();
        }

        private void ApplyConfigToPositioner()
        {
            if (positioner == null)
            {
                return;
            }

            positioner.SetOffset(config.Offset);
            positioner.SetBounds(new Vector2(Screen.width, Screen.height));
            positioner.SetClampToBounds(config.ClampToScreen);
        }

        private static ITooltipDisplay FindDisplayInScene()
        {
            var behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is ITooltipDisplay displayBehaviour)
                {
                    return displayBehaviour;
                }
            }

            return null;
        }
    }

    public static class TooltipLog
    {
        private const string Category = "Tooltips";

        public static void Warning(string message, Object context = null)
        {
            if (context != null)
            {
                Debug.LogWarning($"[KS][{Category}] {message}", context);
            }
            else
            {
                Debug.LogWarning($"[KS][{Category}] {message}");
            }
        }

        public static void Error(string message, Object context = null)
        {
            if (context != null)
            {
                Debug.LogError($"[KS][{Category}] {message}", context);
            }
            else
            {
                Debug.LogError($"[KS][{Category}] {message}");
            }
        }
    }
}
