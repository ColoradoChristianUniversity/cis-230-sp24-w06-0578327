using System.Drawing;
using System.Security.Principal;

public class Menu
{
    private readonly Point _start;
    private readonly string[] _options;
    private int _currentIndex = 1;

    public Menu(Point start, params string[] options)
    {
        _start = start;
        _options = options;
        Console.CursorVisible = false;
    }

    public int Show()
    {
        while (true)
        {
            RedrawMenu();

            var key = Console.ReadKey(true).Key;

            if (TryNumber(key, out var number)) return number;

            if (TryArrow(key)) continue;

            if (key == ConsoleKey.Escape)
            {
                return 0; // this means exit
            }
            else if (key == ConsoleKey.Enter)
            {
                return _currentIndex;
            }
        }

        bool TryArrow(ConsoleKey key)
        {
            if (key == ConsoleKey.UpArrow)
            {
                if (--_currentIndex < 1) _currentIndex = _options.Length;
                return true;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                if (++_currentIndex > _options.Length) _currentIndex = 1;
                return true;
            }
            return false;
        }

        bool TryNumber(ConsoleKey key, out int number)
        {
            for (int i = 0; i < _options.Length; i++)
            {
                if (key == ConsoleKey.D1 + i || key == ConsoleKey.NumPad1 + i)
                {
                    number = i + 1;
                    return true;
                }
            }
            number = default;
            return false;
        }
    }

    private void RedrawMenu()
    {
        Helper.Write("Please choose:", _start.X, _start.Y, ConsoleColor.White, ConsoleColor.Black, true);

        for (var i = 0; i < _options.Length; i++)
        {
            Helper.Write($"{(_currentIndex == i + 1 ? ">" : " ")} {i + 1}. {_options[i]}", _start.X, _start.Y + i + 1, clearFirst: true);
        }
        Helper.Write(_currentIndex.ToString(), _start.X + 2, _start.Y + _currentIndex, ConsoleColor.White, ConsoleColor.Black);
    }
}
