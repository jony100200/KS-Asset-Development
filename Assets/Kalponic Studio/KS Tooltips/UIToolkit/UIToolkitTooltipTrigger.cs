using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace KalponicStudio.Tooltips
{
    /// <summary>
    /// UI Toolkit trigger that shows tooltips when hovering over a VisualElement.
    /// </summary>
    public class UIToolkitTooltipTrigger : MonoBehaviour
    {
        private static MethodInfo panelToScreenMethod;
        private static bool panelToScreenChecked;

        [Header("UI References")]
        [Tooltip("UIDocument containing the target VisualElement")]
        [SerializeField] private UIDocument uiDocument;

        [Tooltip("Name of the VisualElement to attach hover events to")]
        [SerializeField] private string targetElementName = "";

        [Header("Tooltip Configuration")]
        [Tooltip("Tooltip data to display when hovering")]
        [SerializeField] private TooltipData tooltipData;

        [Tooltip("Tooltip service to use (optional - will find automatically)")]
        [SerializeField] private MonoBehaviour tooltipServiceComponent;

        private ITooltipService tooltipService;
        private VisualElement targetElement;

        private void Awake()
        {
            if (tooltipServiceComponent != null)
            {
                tooltipService = tooltipServiceComponent as ITooltipService;
            }

            if (tooltipService == null)
            {
                tooltipService = FindFirstObjectByType<TooltipManager>();
            }
        }

        private void OnEnable()
        {
            Bind();
        }

        private void OnDisable()
        {
            Unbind();
        }

        private void Bind()
        {
            if (uiDocument == null)
            {
                uiDocument = FindFirstObjectByType<UIDocument>();
            }

            if (uiDocument == null || uiDocument.rootVisualElement == null)
            {
                return;
            }

            var root = uiDocument.rootVisualElement;
            targetElement = string.IsNullOrWhiteSpace(targetElementName) ? root : root.Q<VisualElement>(targetElementName);

            if (targetElement == null)
            {
                TooltipLog.Warning("Target VisualElement not found for tooltip trigger.", this);
                return;
            }

            targetElement.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            targetElement.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
        }

        private void Unbind()
        {
            if (targetElement == null)
            {
                return;
            }

            targetElement.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            targetElement.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
            targetElement = null;
        }

        private void OnPointerEnter(PointerEnterEvent evt)
        {
            if (tooltipService == null || tooltipData == null || targetElement == null)
            {
                return;
            }

            var panel = targetElement.panel;
            if (panel == null)
            {
                return;
            }

            Vector2 screenPosition = PanelToScreen(panel, evt.position);
            tooltipService.ShowTooltip(tooltipData, screenPosition);
        }

        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            if (tooltipService == null)
            {
                return;
            }

            tooltipService.HideTooltip();
        }

        private static Vector2 PanelToScreen(IPanel panel, Vector2 panelPosition)
        {
            if (!panelToScreenChecked)
            {
                panelToScreenChecked = true;
                var methods = typeof(RuntimePanelUtils).GetMethods(BindingFlags.Public | BindingFlags.Static);
                for (int i = 0; i < methods.Length; i++)
                {
                    if (methods[i].Name == "PanelToScreen" && methods[i].GetParameters().Length == 2)
                    {
                        panelToScreenMethod = methods[i];
                        break;
                    }
                }
            }

            if (panelToScreenMethod != null)
            {
                return (Vector2)panelToScreenMethod.Invoke(null, new object[] { panel, panelPosition });
            }

            if (panel?.visualTree != null)
            {
                Vector2 world = panel.visualTree.LocalToWorld(panelPosition);
                return new Vector2(world.x, Screen.height - world.y);
            }

            return new Vector2(panelPosition.x, Screen.height - panelPosition.y);
        }
    }
}
