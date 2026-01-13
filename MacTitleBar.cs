using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UnoMacOSTitleBar;

/// <summary>
/// Provides configuration and functionality for customizing the appearance and behavior of the macOS window title bar
/// in a uno app.
/// </summary>
public sealed class MacTitleBar
{
    public MacTitleBar() { NormalizeOptions(); }

    public MacTitleBar(Options options)
    {
        ExtendContent = options.HasFlag(Options.ExtendContent);
        Transparent = options.HasFlag(Options.Transparent);
        HideTitle = options.HasFlag(Options.HideTitle);
        ThickTitleBar = options.HasFlag(Options.ThickTitleBar);
        NormalizeOptions();
    }

    [Flags]
    public enum Options
    {
        None = 0,
        ExtendContent = 1,
        Transparent = 2,
        HideTitle = 4,
        ThickTitleBar = 8
    }

    public bool ExtendContent { get; set; } = true;
    public bool HideTitle { get; set; } = true;
    public bool ThickTitleBar { get; set; } = false;
    public bool Transparent { get; set; } = true;
    public void Apply()
    {
        if (!OperatingSystem.IsMacOS())
        {
            return;
        }

        NormalizeOptions();

        try
        {
            Log("MacTitleBar: Apply() start. Options: ExtendContent=" + ExtendContent + ", Transparent=" + Transparent + ", HideTitle=" + HideTitle + ", ThickTitleBar=" + ThickTitleBar);

            var nsAppClass = objc_getClass("NSApplication");
            var selShared = sel_registerName("sharedApplication");
            var nsApp = objc_msgSend(nsAppClass, selShared);

            // Try several ways to obtain a NSWindow instance
            var selKeyWindow = sel_registerName("keyWindow");
            var nsWindow = objc_msgSend(nsApp, selKeyWindow);

            if (nsWindow == IntPtr.Zero)
            {
                var selMainWindow = sel_registerName("mainWindow");
                nsWindow = objc_msgSend(nsApp, selMainWindow);
            }

            if (nsWindow == IntPtr.Zero)
            {
                var selWindows = sel_registerName("windows");
                var windows = objc_msgSend(nsApp, selWindows);
                if (windows != IntPtr.Zero)
                {
                    var selLastObject = sel_registerName("lastObject");
                    nsWindow = objc_msgSend(windows, selLastObject);
                }
            }

            if (nsWindow == IntPtr.Zero)
            {
                Log("MacTitleBar: NSWindow not found.");
                return;
            }
            else
            {
                Log("MacTitleBar: NSWindow found.");
            }

            if (Transparent)
            {
                var selSetTitlebarAppearsTransparent = sel_registerName("setTitlebarAppearsTransparent:");
                objc_msgSend_bool(nsWindow, selSetTitlebarAppearsTransparent, 1);
                Log("MacTitleBar: setTitlebarAppearsTransparent -> true");
            }

            if (ExtendContent || ThickTitleBar)
            {
                var selStyleMask = sel_registerName("styleMask");
                var currentMask = objc_msgSend_ulong(nsWindow, selStyleMask);
                const ulong FullSizeContentView = 1UL << 15;
                var newMask = currentMask | FullSizeContentView;
                var selSetStyleMask = sel_registerName("setStyleMask:");
                objc_msgSend_ulong_arg(nsWindow, selSetStyleMask, newMask);
                Log("MacTitleBar: ExtendContent applied");
            }

            // Always set title visibility based on HideTitle property
            var selSetTitleVisibility = sel_registerName("setTitleVisibility:");
            if (HideTitle)
            {
                // 1 == NSWindowTitleHidden
                objc_msgSend_int(nsWindow, selSetTitleVisibility, 1);
                Log("MacTitleBar: setTitleVisibility -> Hidden");
            }
            else
            {
                // 0 == NSWindowTitleVisible
                objc_msgSend_int(nsWindow, selSetTitleVisibility, 0);
                Log("MacTitleBar: setTitleVisibility -> Visible");
            }

            if (ThickTitleBar)
            {
                try
                {
                    var toolbarClass = objc_getClass("NSToolbar");
                    if (toolbarClass != IntPtr.Zero)
                    {
                        var selAlloc = sel_registerName("alloc");
                        var toolbarAlloc = objc_msgSend(toolbarClass, selAlloc);
                        var selInit = sel_registerName("init");
                        var toolbar = objc_msgSend(toolbarAlloc, selInit);

                        // Hide the baseline separator
                        var selSetShows = sel_registerName("setShowsBaselineSeparator:");
                        objc_msgSend_bool(toolbar, selSetShows, 0);
                        Log("MacTitleBar: NSToolbar created and showsBaselineSeparator=false");

                        // Attach toolbar to window
                        var selSetToolbar = sel_registerName("setToolbar:");
                        objc_msgSend_IntPtrArg(nsWindow, selSetToolbar, toolbar);
                        Log("MacTitleBar: NSToolbar attached to window");
                    }
                }
                catch
                {
                    Log("MacTitleBar: attaching NSToolbar failed");
                }
            }

            Log("MacTitleBar: Apply() completed.");
        }
        catch
        {
            // swallow any failures but log
            Log("MacTitleBar: Apply() threw an exception");
        }
    }

    private static void Log(string message)
    {
        try
        {
            Debug.WriteLine(message);
            Console.WriteLine(message);
        }
        catch { }
    }

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_getClass")]
    private static extern IntPtr objc_getClass(string name);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern void objc_msgSend_bool(IntPtr receiver, IntPtr selector, byte val);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern IntPtr objc_msgSend_initWithFrame(IntPtr receiver, IntPtr selector, CGRect frame);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern void objc_msgSend_int(IntPtr receiver, IntPtr selector, int arg);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern IntPtr objc_msgSend_IntPtrArg(IntPtr receiver, IntPtr selector, IntPtr arg);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend_stret")]
    private static extern void objc_msgSend_stret(out CGRect stret, IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern ulong objc_msgSend_ulong(IntPtr receiver, IntPtr selector);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    private static extern void objc_msgSend_ulong_arg(IntPtr receiver, IntPtr selector, ulong arg);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "sel_registerName")]
    private static extern IntPtr sel_registerName(string name);

    private void NormalizeOptions()
    {
        // Transparent requires extended content to render correctly.
        if (Transparent)
            ExtendContent = true;

        // ThickTitleBar requires extended content but allows title visibility choice.
        if (ThickTitleBar)
            ExtendContent = true;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct CGPoint
    {
        public double x;
        public double y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CGRect
    {
        public CGPoint origin;
        public CGSize size;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CGSize
    {
        public double width;
        public double height;
    }
}