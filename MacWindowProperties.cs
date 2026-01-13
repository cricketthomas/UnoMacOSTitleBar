using Microsoft.UI.Xaml;

namespace UnoMacOSTitleBar;

/// <summary>
/// Provides XAML attached properties for customizing macOS window title bar appearance.
/// </summary>
/// <example>
/// <code>
/// &lt;Window xmlns:mac="using:UnoMacOSTitleBar"
///         mac:MacWindowProperties.ExtendContent="True"
///         mac:MacWindowProperties.Transparent="True"
///         mac:MacWindowProperties.HideTitle="False"
///         mac:MacWindowProperties.ThickTitleBar="True"&gt;
/// &lt;/Window&gt;
/// </code>
/// </example>
public static class MacWindowProperties
{
    #region ExtendContent

    /// <summary>
    /// Identifies the ExtendContent attached property.
    /// When true, extends window content into the title bar area.
    /// </summary>
    public static readonly DependencyProperty ExtendContentProperty =
        DependencyProperty.RegisterAttached(
            "ExtendContent",
            typeof(bool),
            typeof(MacWindowProperties),
            new PropertyMetadata(true, OnPropertyChanged));

    /// <summary>
    /// Gets the ExtendContent property value.
    /// </summary>
    public static bool GetExtendContent(DependencyObject obj)
        => (bool)obj.GetValue(ExtendContentProperty);

    /// <summary>
    /// Sets the ExtendContent property value.
    /// </summary>
    public static void SetExtendContent(DependencyObject obj, bool value)
        => obj.SetValue(ExtendContentProperty, value);

    #endregion ExtendContent

    #region Transparent

    /// <summary>
    /// Identifies the Transparent attached property.
    /// When true, makes the title bar transparent.
    /// </summary>
    public static readonly DependencyProperty TransparentProperty =
        DependencyProperty.RegisterAttached(
            "Transparent",
            typeof(bool),
            typeof(MacWindowProperties),
            new PropertyMetadata(true, OnPropertyChanged));

    /// <summary>
    /// Gets the Transparent property value.
    /// </summary>
    public static bool GetTransparent(DependencyObject obj)
        => (bool)obj.GetValue(TransparentProperty);

    /// <summary>
    /// Sets the Transparent property value.
    /// </summary>
    public static void SetTransparent(DependencyObject obj, bool value)
        => obj.SetValue(TransparentProperty, value);

    #endregion Transparent

    #region HideTitle

    /// <summary>
    /// Identifies the HideTitle attached property.
    /// When true, hides the window title text.
    /// </summary>
    public static readonly DependencyProperty HideTitleProperty =
        DependencyProperty.RegisterAttached(
            "HideTitle",
            typeof(bool),
            typeof(MacWindowProperties),
            new PropertyMetadata(true, OnPropertyChanged));

    /// <summary>
    /// Gets the HideTitle property value.
    /// </summary>
    public static bool GetHideTitle(DependencyObject obj)
        => (bool)obj.GetValue(HideTitleProperty);

    /// <summary>
    /// Sets the HideTitle property value.
    /// </summary>
    public static void SetHideTitle(DependencyObject obj, bool value)
        => obj.SetValue(HideTitleProperty, value);

    #endregion HideTitle

    #region ThickTitleBar

    /// <summary>
    /// Identifies the ThickTitleBar attached property.
    /// When true, creates a thicker draggable title bar area using NSToolbar.
    /// </summary>
    public static readonly DependencyProperty ThickTitleBarProperty =
        DependencyProperty.RegisterAttached(
            "ThickTitleBar",
            typeof(bool),
            typeof(MacWindowProperties),
            new PropertyMetadata(false, OnPropertyChanged));

    /// <summary>
    /// Gets the ThickTitleBar property value.
    /// </summary>
    public static bool GetThickTitleBar(DependencyObject obj)
        => (bool)obj.GetValue(ThickTitleBarProperty);

    /// <summary>
    /// Sets the ThickTitleBar property value.
    /// </summary>
    public static void SetThickTitleBar(DependencyObject obj, bool value)
        => obj.SetValue(ThickTitleBarProperty, value);

    #endregion ThickTitleBar

    #region Options (Alternative: Single Property with Flags)

    /// <summary>
    /// Identifies the Options attached property for combined configuration using flags.
    /// </summary>
    public static readonly DependencyProperty OptionsProperty =
        DependencyProperty.RegisterAttached(
            "Options",
            typeof(MacTitleBar.Options),
            typeof(MacWindowProperties),
            new PropertyMetadata(MacTitleBar.Options.None, OnOptionsChanged));

    /// <summary>
    /// Gets the Options property value.
    /// </summary>
    public static MacTitleBar.Options GetOptions(DependencyObject obj)
        => (MacTitleBar.Options)obj.GetValue(OptionsProperty);

    /// <summary>
    /// Sets the Options property value.
    /// </summary>
    public static void SetOptions(DependencyObject obj, MacTitleBar.Options value)
        => obj.SetValue(OptionsProperty, value);

    #endregion Options (Alternative: Single Property with Flags)

    private static void ApplyTitleBarSettings()
    {
        // Get current window
        var window = Microsoft.UI.Xaml.Window.Current;
        if (window?.Content is FrameworkElement content)
        {
            var macBar = new MacTitleBar
            {
                ExtendContent = GetExtendContent(content),
                Transparent = GetTransparent(content),
                HideTitle = GetHideTitle(content),
                ThickTitleBar = GetThickTitleBar(content)
            };
            macBar.Apply();
        }
    }

    private static void ApplyTitleBarSettings(MacTitleBar.Options options)
    {
        var macBar = new MacTitleBar(options);
        macBar.Apply();
    }

    private static void ApplyWhenReady(DependencyObject d, MacTitleBar.Options? options = null)
    {
        // Since Window doesn't inherit from DependencyObject in Uno,
        // we apply immediately assuming this is being set at startup
        if (options.HasValue)
        {
            ApplyTitleBarSettings(options.Value);
        }
        else
        {
            ApplyTitleBarSettings();
        }
    }

    private static void OnOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is MacTitleBar.Options options)
        {
            ApplyWhenReady(d, options);
        }
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // The attached properties are set on the Window's Content (usually a Frame or Page)
        // We need to traverse up to find the Window or apply when Content is set
        ApplyWhenReady(d);
    }
}