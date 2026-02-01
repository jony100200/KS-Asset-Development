using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KalponicStudio.Tooltips
{
    /// <summary>
    /// UGUI implementation of tooltip display using RectTransform and TextMeshPro.
    /// </summary>
    public class UGUITooltipDisplay : MonoBehaviour, ITooltipDisplay
    {
        [Header("UI References")]
        [SerializeField] private RectTransform tooltipRoot;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private LayoutElement layoutElement;
        [SerializeField] private Canvas canvas;

        private bool isVisible;

        private void Awake()
        {
            if (tooltipRoot == null)
            {
                tooltipRoot = GetComponent<RectTransform>();
            }

            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
            }

            HideTooltip();
        }

        public void ShowTooltip(ITooltipData data, Vector2 position)
        {
            if (data == null || tooltipRoot == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = data.Title;
                titleText.color = data.TitleColor;
            }

            if (descriptionText != null)
            {
                descriptionText.text = data.Description;
                descriptionText.color = data.DescriptionColor;
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = data.BackgroundTint;
            }

            if (layoutElement != null)
            {
                layoutElement.preferredWidth = data.MaxWidth;
            }

            UpdatePosition(position);

            tooltipRoot.gameObject.SetActive(true);
            isVisible = true;
        }

        public void HideTooltip()
        {
            if (tooltipRoot != null)
            {
                tooltipRoot.gameObject.SetActive(false);
            }

            isVisible = false;
        }

        public void UpdatePosition(Vector2 position)
        {
            if (tooltipRoot == null || canvas == null)
            {
                return;
            }

            RectTransform canvasRect = canvas.transform as RectTransform;
            if (canvasRect == null)
            {
                return;
            }

            Camera eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, position, eventCamera, out Vector2 localPoint))
            {
                tooltipRoot.anchoredPosition = localPoint;
            }
        }

        public bool IsVisible => isVisible;

        public Vector2 GetTooltipSize()
        {
            return tooltipRoot != null ? tooltipRoot.rect.size : Vector2.zero;
        }
    }
}
