# KS Tooltip System

A clean, SOLID, game-agnostic tooltip system for Unity with modular UGUI and UI Toolkit support.

## Design Goals

- KISS: minimal interfaces, clear responsibilities
- SOLID: SRP and dependency inversion
- Game-agnostic: works with any game type or UI framework
- Testable: no singletons, easy to mock

## Compatibility

- Primary: Unity 6.3 LTS
- Optional: Unity 2022.3 LTS (supported if your project already uses it)

## Architecture

Core interfaces and data:

- `ITooltipData`
- `ITooltipService`
- `ITooltipDisplay`
- `ITooltipPositioner`
- `TooltipConfig`

Concrete implementations:

- `TooltipData` (ScriptableObject)
- `TooltipManager` (service implementation)
- `MouseFollowPositioner`
- `UGUITooltipDisplay` + `TooltipTrigger` (UGUI)
- `UIToolkitTooltipDisplay` + `UIToolkitTooltipTrigger` (UI Toolkit)

## Assemblies

- `KalponicStudio.Tooltips.Core`
- `KalponicStudio.Tooltips.UGUI`
- `KalponicStudio.Tooltips.UIToolkit`

## Quick Setup (UGUI)

1. Create a GameObject and add `TooltipManager`.
2. Add `UGUITooltipDisplay` on the tooltip UI root.
3. Assign:
   - `tooltipRoot` (RectTransform)
   - `titleText` and `descriptionText` (TMP_Text)
   - Optional: `backgroundImage`, `layoutElement`, `canvas`
4. Add `TooltipTrigger` to any UI element with a `Graphic` or `Selectable`.
5. Assign a `TooltipData` asset to the trigger.

Notes:
- This implementation uses TextMeshPro. Add the TMP package if not already in the project.
- Works best with Screen Space Overlay canvas, but Screen Space Camera is supported.

## Quick Setup (UI Toolkit)

1. Create a GameObject and add `TooltipManager`.
2. Add `UIToolkitTooltipDisplay` and assign the `UIDocument`.
3. Ensure your UXML contains the required elements:

```xml
<VisualElement name="TooltipContainer">
    <Label name="TooltipTitle" />
    <Label name="TooltipDescription" />
</VisualElement>
```

4. Add `UIToolkitTooltipTrigger` and assign:
   - `UIDocument`
   - `targetElementName` (name of the VisualElement)
   - `TooltipData`

Notes:
- The tooltip container should be positioned absolute in USS.
- Element names are configurable on `UIToolkitTooltipDisplay`.

## Runtime Usage

```csharp
var tooltipData = TooltipData.CreateRuntime("Title", "Description");
tooltipManager.ShowTooltip(tooltipData);
```

## Custom Configuration

```csharp
var config = new TooltipConfig
{
    ShowDelay = 0.4f,
    HideDelay = 0.2f,
    Offset = new Vector2(12f, 12f),
    FollowMouse = true,
    ClampToScreen = true
};

tooltipManager.UpdateConfiguration(config);
```

## Extension Points

Custom display:

```csharp
public class CustomDisplay : MonoBehaviour, ITooltipDisplay
{
    public void ShowTooltip(ITooltipData data, Vector2 position) { }
    public void HideTooltip() { }
    public void UpdatePosition(Vector2 position) { }
    public bool IsVisible => false;
    public Vector2 GetTooltipSize() => Vector2.zero;
}
```

Custom positioner:

```csharp
public class AnchoredPositioner : ITooltipPositioner
{
    public Vector2 CalculatePosition(Vector2 trigger, Vector2 size, Vector2 screen) => trigger;
    public void SetOffset(Vector2 offset) { }
    public void SetBounds(Vector2 bounds) { }
    public void SetClampToBounds(bool clamp) { }
}
```

## Troubleshooting

- Tooltip not showing (UGUI): ensure `TooltipTrigger` is on a UI element with raycast target enabled.
- Tooltip not showing (UI Toolkit): check `targetElementName` and UXML element names.
- Position looks wrong: ensure UI Toolkit tooltip container uses absolute positioning.
- Null references: verify `UIDocument` or `Canvas` references are assigned.

---

Made by Kalponic Studio
