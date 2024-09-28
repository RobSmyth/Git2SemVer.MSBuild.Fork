namespace NoeticTools.Git2SemVer.Tool.Framework;

public sealed class ConsoleColourScope : IDisposable
{
    private readonly ConsoleColor _priorBackground;
    private readonly ConsoleColor _priorForeground;

    public ConsoleColourScope()
    {
        _priorForeground = Console.ForegroundColor;
        _priorBackground = Console.BackgroundColor;
    }

    public ConsoleColourScope(ConsoleColor foreground, ConsoleColor background) : this()
    {
        Console.ForegroundColor = foreground;
        Console.BackgroundColor = background;
    }

    public void Dispose()
    {
        Console.ForegroundColor = _priorForeground;
        Console.BackgroundColor = _priorBackground;
        Thread.Sleep(0);
    }
}