using Microsoft.UI.Xaml;

namespace UnoMacOSTitleBar;

/// <summary>
/// Helper methods for customizing macOS window title bar appearance in Uno Platform.
/// </summary>
/// <example>
/// <code>
/// // In App.xaml.cs OnLaunched:
/// #if HAS_UNO_SKIA_APPLE_UIKIT || HAS_UNO_SKIA_MACOS
/// MacWindowHelper.Configure(extendContent: true,transparent: true, hideTitle: false,thickTitleBar: true);
/// #endif
/// </code>
/// </example>
public static class MacWindowHelper
{
    /// <summary>
    /// Configures the macOS window title bar with the specified options.
    /// </summary>
    /// <param name="extendContent">Extends window content into the title bar area.</param>
    /// <param name="transparent">Makes the title bar transparent. Transparent requires extended content to render correctly.</param>
    /// <param name="hideTitle">Hides the window title text.</param>
    /// <param name="thickTitleBar">Creates a thicker draggable title bar area. ThickTitleBar requires extended content but allows title visibility choice.</param>
    public static void Configure(
        bool extendContent = true,
        bool transparent = true,
        bool hideTitle = true,
        bool thickTitleBar = false)
    {
        var macTitleBar = new MacTitleBar
        {
            ExtendContent = extendContent,
            Transparent = transparent,
            HideTitle = hideTitle,
            ThickTitleBar = thickTitleBar
        };
        macTitleBar.Apply();
    }

    /// <summary>
    /// Configure macOS window title bar with combined options using flags.
    /// </summary>
    /// <param name="options">Combined options using MacTitleBar.Options flags.</param>
    public static void Configure(MacTitleBar.Options options)
    {
        var macTitleBar = new MacTitleBar(options);
        macTitleBar.Apply();
    }
}