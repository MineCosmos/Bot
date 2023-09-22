using Spectre.Console;

namespace MineCosmos.Bot.Common;

public static class ConsoleHelper
{

    /// <summary>
    /// 成功信息
    /// </summary>
    /// <param name="message"></param>
    public static void SuccessMessage(string message)
    {
        try
        {
            AnsiConsole.MarkupLine($"[green] {message} [/] ");
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.Message);
        }
        
    }

    /// <summary>
    /// 警告信息
    /// </summary>
    /// <param name="message"></param>
    public static void WarningMessage(string message)
    {
        AnsiConsole.MarkupLine($"[blue] {message} [/] ");
    }

    /// <summary>
    /// 错误信息
    /// </summary>
    /// <param name="message"></param>
    public static void ErrorMessage(string message)
    {
        AnsiConsole.MarkupLine($"[red] {message} [/] ");
    }

    /// <summary>
    /// 错误信息
    /// </summary>
    /// <param name="message"></param>
    public static void LogMessage(string message)
    {
        AnsiConsole.MarkupLine($"[darkblue] {message} [/] ");
    }

    public static void LogMessage(string title, params string[] sections)
    {
        try
        {
            if (sections.Any())
            {
                var content = Align.Center(
                    new Markup(string.Join(Environment.NewLine, sections)),
                    VerticalAlignment.Middle);
                var panel = new Panel(content);

                panel.Border = BoxBorder.Ascii;
                panel.Border = BoxBorder.Square;
                panel.Border = BoxBorder.Rounded;
                panel.Border = BoxBorder.Heavy;
                panel.Border = BoxBorder.Double;
                panel.Border = BoxBorder.None;
                panel.Padding = new Padding(1, 1, 1, 1);
                panel.Border = BoxBorder.Rounded;
                panel.Header = new PanelHeader($"[darkblue] {title} [/] ");
                AnsiConsole.Write(panel);
            }
        }
        catch (Exception)
        {
        }
    }
}
