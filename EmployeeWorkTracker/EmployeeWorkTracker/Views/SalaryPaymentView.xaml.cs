using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EmployeeWorkTracker.Views;

public partial class SalaryPaymentView : UserControl
{
    private static readonly Regex DigitRegex = new(@"^[0-9]$");

    public SalaryPaymentView()
    {
        InitializeComponent();
    }

    private void DateText_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (sender is not TextBox textBox) return;

        var text = e.Text;

        // Разрешаем только цифры
        if (!DigitRegex.IsMatch(text))
        {
            e.Handled = true;
            return;
        }

        var currentText = textBox.Text;
        var caretIndex = textBox.CaretIndex;
        var digit = int.Parse(text);

        // Определяем позицию ввода (игнорируем точки)
        var position = GetDigitPosition(currentText, caretIndex);

        // Валидация по позиции
        bool allowed = position switch
        {
            0 => ValidateDayFirstDigit(digit),
            1 => ValidateDaySecondDigit(currentText, digit),
            2 => ValidateMonthFirstDigit(digit),
            3 => ValidateMonthSecondDigit(currentText, digit),
            4 => ValidateYearFirstDigit(digit),
            5 or 6 or 7 => true,
            _ => false
        };

        if (!allowed)
        {
            e.Handled = true;
            return;
        }

        // Автоматически добавляем точку после дня (2 цифры) и месяца (2 цифры)
        var newDigitCount = GetDigitCount(currentText) + 1;

        if (newDigitCount == 2)
        {
            var newText = InsertAt(currentText, caretIndex, text + ".");
            textBox.Text = newText;
            textBox.CaretIndex = caretIndex + 2;
            e.Handled = true;
            return;
        }

        if (newDigitCount == 4)
        {
            var newText = InsertAt(currentText, caretIndex, text + ".");
            textBox.Text = newText;
            textBox.CaretIndex = caretIndex + 2;
            e.Handled = true;
            return;
        }
    }

    private static int GetDigitPosition(string text, int caretIndex)
    {
        int digitPos = 0;
        for (int i = 0; i < caretIndex && i < text.Length; i++)
        {
            if (char.IsDigit(text[i]))
                digitPos++;
        }
        return digitPos;
    }

    private static int GetDigitCount(string text)
    {
        return text.Count(char.IsDigit);
    }

    private static string InsertAt(string text, int index, string insert)
    {
        if (index < 0) index = 0;
        if (index > text.Length) index = text.Length;
        return text.Insert(index, insert);
    }

    // Позиция 0: первая цифра дня (0-3)
    private static bool ValidateDayFirstDigit(int digit) => digit <= 3;

    // Позиция 1: вторая цифра дня
    // Если первая = 0, то вторая должна быть 1-9 (01-09, но не 00)
    // Если первая = 1 или 2, то вторая 0-9 (10-19, 20-29)
    // Если первая = 3, то вторая только 0-1 (30, 31)
    private static bool ValidateDaySecondDigit(string currentText, int digit)
    {
        var firstDigit = GetNthDigit(currentText, 1);

        if (firstDigit == 0)
            return digit >= 1; // 01-09 (но не 00)
        if (firstDigit == 3)
            return digit <= 1; // 30, 31
        return true; // 10-29
    }

    // Позиция 2: первая цифра месяца (0 или 1)
    private static bool ValidateMonthFirstDigit(int digit) => digit <= 1;

    // Позиция 3: вторая цифра месяца
    // Если первая = 0, то вторая должна быть 1-9 (01-09, но не 00)
    // Если первая = 1, то вторая 0-2 (10, 11, 12)
    private static bool ValidateMonthSecondDigit(string currentText, int digit)
    {
        var monthFirstDigit = GetNthDigit(currentText, 3);

        if (monthFirstDigit == 0)
            return digit >= 1; // 01-09 (но не 00)
        if (monthFirstDigit == 1)
            return digit <= 2; // 10, 11, 12
        return true;
    }

    // Позиция 4: первая цифра года (2-5, для диапазона 2004-5000)
    private static bool ValidateYearFirstDigit(int digit) => digit >= 2 && digit <= 5;

    private static int GetNthDigit(string text, int n)
    {
        int count = 0;
        foreach (var c in text)
        {
            if (char.IsDigit(c))
            {
                count++;
                if (count == n)
                    return c - '0';
            }
        }
        return 0;
    }
}