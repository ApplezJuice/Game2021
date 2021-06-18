using System.Collections;
using System.Collections.Generic;

public class ServerLog
{
    private static ServerLog _instance;
    protected ServerLog() { }
    public static ServerLog Instance()
    {
        if (_instance == null)
        {
            _instance = new ServerLog();
        }

        return _instance;
    }

    public enum LogType
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3
    }

    public static void Log(LogType type, string message)
    {
        switch (type)
        {
            case LogType.Debug:
                System.Console.ResetColor();
                break;
            case LogType.Info:
                System.Console.ForegroundColor = System.ConsoleColor.Green;
                break;
            case LogType.Warn:
                System.Console.ForegroundColor = System.ConsoleColor.Yellow;
                break;
            case LogType.Error:
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                break;
            default:
                System.Console.ResetColor();
                break;
        }
        System.Console.WriteLine($"[{type}] - {message}");
        System.Console.ResetColor();
    }
}
