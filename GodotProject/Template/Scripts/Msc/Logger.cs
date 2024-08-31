namespace GodotUtils;

using Godot;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

/*
 * This is meant to replace all GD.Print(...) with Logger.Log(...) to make
 * logging multi-thread friendly. Remember to put Logger.Update() in
 * _PhysicsProcess(double delta) otherwise you will be wondering why Logger.Log(...)
 * is printing nothing to the console.
 */
public class Logger
{
    public event Action<string> MessageLogged;

    readonly ConcurrentQueue<LogInfo> messages = new();

    /// <summary>
    /// Log a message
    /// </summary>
    public void Log(object message, BBColor color = BBColor.Gray) =>
        messages.Enqueue(new LogInfo(LoggerOpcode.Message, new LogMessage($"{message}"), color));

    /// <summary>
    /// Log a warning
    /// </summary>
    public void LogWarning(object message, BBColor color = BBColor.Orange) =>
        Log($"[Warning] {message}", color);

    /// <summary>
    /// Log a todo
    /// </summary>
    public void LogTodo(object message, BBColor color = BBColor.White) =>
        Log($"[Todo] {message}", color);

    /// <summary>
    /// Logs an exception with trace information. Optionally allows logging a human readable hint
    /// </summary>
    public void LogErr
    (
        Exception e,
        string hint = default,
        BBColor color = BBColor.Red,
        [CallerFilePath] string filePath = default,
        [CallerLineNumber] int lineNumber = 0
    ) => LogDetailed(LoggerOpcode.Exception, $"[Error] {(string.IsNullOrWhiteSpace(hint) ? "" : $"'{hint}' ")}{e.Message}{e.StackTrace}", color, true, filePath, lineNumber);

    /// <summary>
    /// Logs a debug message that optionally contains trace information
    /// </summary>
    public void LogDebug
    (
        object message,
        BBColor color = BBColor.Magenta,
        bool trace = true,
        [CallerFilePath] string filePath = default,
        [CallerLineNumber] int lineNumber = 0
    ) => LogDetailed(LoggerOpcode.Debug, $"[Debug] {message}", color, trace, filePath, lineNumber);

    /// <summary>
    /// Log the time it takes to do a section of code
    /// </summary>
    public void LogMs(Action code)
    {
        Stopwatch watch = new Stopwatch();
        watch.Start();
        code();
        watch.Stop();
        Log($"Took {watch.ElapsedMilliseconds} ms");
    }

    /// <summary>
    /// Checks to see if there are any messages left in the queue
    /// </summary>
    public bool StillWorking() => !messages.IsEmpty;

    /// <summary>
    /// Dequeues a Requested Message and Logs it
    /// </summary>
    public void Update()
    {
        if (!messages.TryDequeue(out LogInfo result))
            return;

        switch (result.Opcode)
        {
            case LoggerOpcode.Message:
                Print(result.Data.Message, result.Color);
                Console.ResetColor();
                break;

            case LoggerOpcode.Exception:
                PrintErr(result.Data.Message, result.Color);

                if (result.Data is LogMessageTrace exceptionData && exceptionData.ShowTrace)
                    PrintErr(exceptionData.TracePath, BBColor.DarkGray);

                Console.ResetColor();
                break;

            case LoggerOpcode.Debug:
                Print(result.Data.Message, result.Color);

                if (result.Data is LogMessageTrace debugData && debugData.ShowTrace)
                    Print(debugData.TracePath, BBColor.DarkGray);

                Console.ResetColor();
                break;
        }

        MessageLogged?.Invoke(result.Data.Message);
    }

    /// <summary>
    /// Logs a message that may contain trace information
    /// </summary>
    void LogDetailed(LoggerOpcode opcode, string message, BBColor color, bool trace, string filePath, int lineNumber)
    {
        string tracePath;

        if (filePath.Contains("Scripts"))
        {
            // Ex: Scripts/Main.cs:23
            tracePath = $"  at {filePath.Substring(filePath.IndexOf("Scripts"))}:{lineNumber}";
            tracePath = tracePath.Replace('\\', '/');
        }
        else
        {
            // Main.cs:23
            string[] elements = filePath.Split('\\');
            tracePath = $"  at {elements[elements.Length - 1]}:{lineNumber}";
        }

        messages.Enqueue(
            new LogInfo(opcode,
                new LogMessageTrace(
                    message,
                    trace,
                    tracePath
                ),
            color
        ));
    }

    void Print(object v, BBColor color)
    {
        //Console.ForegroundColor = color;

        if (GOS.IsExportedRelease())
            GD.Print(v);
        else
            // Full list of BBCode color tags: https://absitomen.com/index.php?topic=331.0
            GD.PrintRich($"[color={color}]{v}");
    }

    void PrintErr(object v, BBColor color)
    {
        //Console.ForegroundColor = color;
        GD.PrintErr(v);
        GD.PushError(v);
    }
}

public class LogInfo
{
    public LoggerOpcode Opcode { get; set; }
    public LogMessage Data { get; set; }
    public BBColor Color { get; set; }

    public LogInfo(LoggerOpcode opcode, LogMessage data, BBColor color = BBColor.Gray)
    {
        Opcode = opcode;
        Data = data;
        Color = color;
    }
}

public class LogMessage
{
    public string Message { get; set; }
    public LogMessage(string message) => this.Message = message;

}
public class LogMessageTrace : LogMessage
{
    // Show the Trace Information for the Message
    public bool ShowTrace { get; set; }
    public string TracePath { get; set; }

    public LogMessageTrace(string message, bool trace = true, string tracePath = default)
    : base(message)
    {
        ShowTrace = trace;
        TracePath = tracePath;
    }
}

public enum LoggerOpcode
{
    Message,
    Exception,
    Debug
}

// Full list of BBCode color tags: https://absitomen.com/index.php?topic=331.0
public enum BBColor
{
    Gray,
    DarkGray,
    Green,
    DarkGreen,
    LightGreen,
    Aqua,
    DarkAqua,
    Deepskyblue,
    Magenta,
    Red,
    White,
    Yellow,
    Orange
}
