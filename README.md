# UnoMacOSTitleBar
Customize macOS window title bars in Uno Platform applications with simple XAML properties or code-behind! 

## Video Demo

https://github.com/user-attachments/assets/14cc0839-1d04-4eaf-8acb-c69bbcace666


## Installation
```
dotnet add package UnoMacOSTitleBar
```
https://www.nuget.org/packages/UnoMacOSTitleBar/

## Usage
### Option 1:

In your `Directory.Packages.props` file
```xml
 <ItemGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) != 'windows'">
     <PackageVersion Include="UnoMacOSTitleBar" Version="xxxxxxxxxxx" />
 </ItemGroup>
```
In your `csproj` file
```xml
 <ItemGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) != 'windows'">
     <PackageReference Include="UnoMacOSTitleBar" />
 </ItemGroup>
```


In `App.xaml.cs`:

```csharp
#if HAS_UNO_SKIA_MACOS || __UNO_SKIA_MACOS__
using UnoMacOSTitleBar;
#endif

protected override void OnLaunched(LaunchActivatedEventArgs args)
{
    var window = Microsoft.UI.Xaml.Window.Current;

#if HAS_UNO_SKIA_APPLE_UIKIT || HAS_UNO_SKIA_MACOS
    MacWindowHelper.Configure(
        extendContent: true,
        transparent: true,
        hideTitle: false, 
        thickTitleBar: true
    );
#endif

    // Rest of your app initialization
    var rootFrame = new Frame();
    window.Content = rootFrame;
    rootFrame.Navigate(typeof(MainPage), args.Arguments);
    window.Activate();
}
```

### Option 2: Using Flags Enum

```csharp
using UnoMacOSTitleBar;

#if HAS_UNO_SKIA_APPLE_UIKIT || HAS_UNO_SKIA_MACOS
MacWindowHelper.Configure(
    MacTitleBar.Options.ExtendContent | 
    MacTitleBar.Options.Transparent | 
    MacTitleBar.Options.ThickTitleBar
);
#endif
```

### Option 3: Direct MacTitleBar Usage

For full control, use the `MacTitleBar` class directly:

```csharp
#if HAS_UNO_SKIA_MACOS || __UNO_SKIA_MACOS__
var macBar = new UnoMacOSTitleBar.MacTitleBar
{
    ExtendContent = true,
    Transparent = true,
    HideTitle = false,
    ThickTitleBar = true,
};
macBar.Apply();
#endif
```

## Property Details

### ExtendContent
Extends your window content into the title bar area, allowing you to create custom title bar experiences. Required for `Transparent` and `ThickTitleBar` to work correctly.

### Transparent
Makes the title bar background transparent, creating a seamless look between your content and the title bar.
<img width="758" height="522"  src="https://github.com/user-attachments/assets/66ad1ac9-ec6c-4d81-944c-8d784529871a" />

### HideTitle
Controls whether the native macOS window title is visible or hidden. Set to `false` to keep your window title visible.

### ThickTitleBar
Creates a thicker draggable area by attaching an `NSToolbar` to the window. Perfect for creating custom title bars with more vertical space.

<img width="758" height="522"  src="https://github.com/user-attachments/assets/469b6786-342a-4ea1-a81b-7d8060e8371b" />
<img width="758" height="522"  src="https://github.com/user-attachments/assets/ce775b10-b396-483d-9ed2-620132c80035" />

## Examples

### Transparent Title Bar with Title Visible
>[!WARNING]
> This is currently a work  in progress with bindings to compile on other platforms.

```xml
<Window mac:MacWindowProperties.ExtendContent="True"
        mac:MacWindowProperties.Transparent="True"
        mac:MacWindowProperties.HideTitle="False"
        mac:MacWindowProperties.ThickTitleBar="True">
// This can also be done on MainPage.xaml

```

### Custom Title Bar (No Native Title)

```xml
<Window mac:MacWindowProperties.ThickTitleBar="True"
        mac:MacWindowProperties.HideTitle="True">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="40" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>
        
        <!-- Custom title bar content -->
        <Grid Grid.Row="0" Background="Transparent">
            <TextBlock Text="My Custom Title" />
        </Grid>
        
        <!-- Main content -->
        <Grid Grid.Row="1">
            <!-- Your app content -->
        </Grid>
    </Grid>
</Window>
```

## Requirements

- Uno Platform 6.0+
- .NET 10.0+
- Skia desktop target on macOS
