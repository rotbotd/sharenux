using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using ShareNux.Preview;

AppBuilder.Configure<App>()
    .UsePlatformDetect()
    .StartWithClassicDesktopLifetime(args);

class App : Application
{
    public override void Initialize()
    {
        RequestedThemeVariant = ThemeVariant.Dark;
        Styles.Add(new FluentTheme());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new HashCheckerWindow();
        }
        base.OnFrameworkInitializationCompleted();
    }
}
