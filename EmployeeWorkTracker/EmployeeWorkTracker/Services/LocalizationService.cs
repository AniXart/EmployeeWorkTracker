using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace EmployeeWorkTracker.Services;

public enum AppLanguage
{
    English,
    Russian,
    Ukrainian,
    Czech
}

public static class LocalizationService
{
    private static readonly string SettingsFilePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "AppData", "settings.json");

    private static readonly Dictionary<string, Dictionary<AppLanguage, string>> Translations = new()
    {
        ["MainMenu"] = new()
        {
            [AppLanguage.English] = "Main Menu",
            [AppLanguage.Russian] = "Главное меню",
            [AppLanguage.Ukrainian] = "Головне меню",
            [AppLanguage.Czech] = "Hlavní nabídka"
        },
        ["EmployeesManagement"] = new()
        {
            [AppLanguage.English] = "Employees Management",
            [AppLanguage.Russian] = "Управление сотрудниками",
            [AppLanguage.Ukrainian] = "Управління співробітниками",
            [AppLanguage.Czech] = "Správa zaměstnanců"
        },
        ["EmployeesManagementDesc"] = new()
        {
            [AppLanguage.English] = "Add, edit and search employees",
            [AppLanguage.Russian] = "Добавление, редактирование и поиск сотрудников",
            [AppLanguage.Ukrainian] = "Додавання, редагування та пошук співробітників",
            [AppLanguage.Czech] = "Přidávání, úpravy a vyhledávání zaměstnanců"
        },
        ["Database"] = new()
        {
            [AppLanguage.English] = "Database",
            [AppLanguage.Russian] = "База данных",
            [AppLanguage.Ukrainian] = "База даних",
            [AppLanguage.Czech] = "Databáze"
        },
        ["DatabaseDesc"] = new()
        {
            [AppLanguage.English] = "Storage of all employee cards",
            [AppLanguage.Russian] = "Хранение карточек всех сотрудников",
            [AppLanguage.Ukrainian] = "Зберігання карток усіх співробітників",
            [AppLanguage.Czech] = "Ukládání karet všech zaměstnanců"
        },
        ["SalaryPayment"] = new()
        {
            [AppLanguage.English] = "Salary Payment",
            [AppLanguage.Russian] = "Начисление зарплаты",
            [AppLanguage.Ukrainian] = "Нарахування зарплати",
            [AppLanguage.Czech] = "Výpočet mzdy"
        },
        ["SalaryPaymentDesc"] = new()
        {
            [AppLanguage.English] = "Salary payment to employees",
            [AppLanguage.Russian] = "Начисление зарплаты сотрудникам",
            [AppLanguage.Ukrainian] = "Нарахування зарплати співробітникам",
            [AppLanguage.Czech] = "Výpočet mzdy zaměstnancům"
        },
        ["ViewReports"] = new()
        {
            [AppLanguage.English] = "View Reports",
            [AppLanguage.Russian] = "Посмотреть отчёты",
            [AppLanguage.Ukrainian] = "Переглянути звіти",
            [AppLanguage.Czech] = "Zobrazit zprávy"
        },
        ["ViewReportsDesc"] = new()
        {
            [AppLanguage.English] = "Analytics and work hours statistics",
            [AppLanguage.Russian] = "Аналитика и статистика рабочих часов",
            [AppLanguage.Ukrainian] = "Аналітика та статистика робочих годин",
            [AppLanguage.Czech] = "Analýza a statistika pracovních hodin"
        },
        ["EmployeesDatabase"] = new()
        {
            [AppLanguage.English] = "Employees Database",
            [AppLanguage.Russian] = "База данных сотрудников",
            [AppLanguage.Ukrainian] = "База даних співробітників",
            [AppLanguage.Czech] = "Databáze zaměstnanců"
        },
        ["AddEmployee"] = new()
        {
            [AppLanguage.English] = "Add Employee",
            [AppLanguage.Russian] = "Добавить сотрудника",
            [AppLanguage.Ukrainian] = "Додати співробітника",
            [AppLanguage.Czech] = "Přidat zaměstnance"
        },
        ["Copied"] = new()
        {
            [AppLanguage.English] = "Copied",
            [AppLanguage.Russian] = "Скопировано",
            [AppLanguage.Ukrainian] = "Скопійовано",
            [AppLanguage.Czech] = "Zkopírováno"
        },
        ["Edit"] = new()
        {
            [AppLanguage.English] = "Edit",
            [AppLanguage.Russian] = "Редактировать",
            [AppLanguage.Ukrainian] = "Редагувати",
            [AppLanguage.Czech] = "Upravit"
        },
        ["Delete"] = new()
        {
            [AppLanguage.English] = "Delete",
            [AppLanguage.Russian] = "Удалить",
            [AppLanguage.Ukrainian] = "Видалити",
            [AppLanguage.Czech] = "Smazat"
        },
        ["EditEmployee"] = new()
        {
            [AppLanguage.English] = "Edit Employee",
            [AppLanguage.Russian] = "Редактирование сотрудника",
            [AppLanguage.Ukrainian] = "Редагування співробітника",
            [AppLanguage.Czech] = "Úprava zaměstnance"
        },
        ["NewEmployee"] = new()
        {
            [AppLanguage.English] = "New Employee",
            [AppLanguage.Russian] = "Новый сотрудник",
            [AppLanguage.Ukrainian] = "Новий співробітник",
            [AppLanguage.Czech] = "Nový zaměstnanec"
        },
        ["LastName"] = new()
        {
            [AppLanguage.English] = "Last Name",
            [AppLanguage.Russian] = "Фамилия",
            [AppLanguage.Ukrainian] = "Прізвище",
            [AppLanguage.Czech] = "Příjmení"
        },
        ["FirstName"] = new()
        {
            [AppLanguage.English] = "First Name",
            [AppLanguage.Russian] = "Имя",
            [AppLanguage.Ukrainian] = "Ім'я",
            [AppLanguage.Czech] = "Jméno"
        },
        ["MiddleName"] = new()
        {
            [AppLanguage.English] = "Middle Name",
            [AppLanguage.Russian] = "Отчество",
            [AppLanguage.Ukrainian] = "По батькові",
            [AppLanguage.Czech] = "Otčestvo"
        },
        ["PassportNumber"] = new()
        {
            [AppLanguage.English] = "Passport No.",
            [AppLanguage.Russian] = "№ Паспорта",
            [AppLanguage.Ukrainian] = "№ Паспорта",
            [AppLanguage.Czech] = "Číslo pasu"
        },
        ["Phone"] = new()
        {
            [AppLanguage.English] = "Phone",
            [AppLanguage.Russian] = "Телефон",
            [AppLanguage.Ukrainian] = "Телефон",
            [AppLanguage.Czech] = "Telefon"
        },
        ["Position"] = new()
        {
            [AppLanguage.English] = "Position",
            [AppLanguage.Russian] = "Должность",
            [AppLanguage.Ukrainian] = "Посада",
            [AppLanguage.Czech] = "Pozice"
        },
        ["PaymentType"] = new()
        {
            [AppLanguage.English] = "Payment Type",
            [AppLanguage.Russian] = "Тип оплаты",
            [AppLanguage.Ukrainian] = "Тип оплати",
            [AppLanguage.Czech] = "Typ platby"
        },
        ["Hourly"] = new()
        {
            [AppLanguage.English] = "Hourly",
            [AppLanguage.Russian] = "Почасовая",
            [AppLanguage.Ukrainian] = "Погодинна",
            [AppLanguage.Czech] = "Hodinová"
        },
        ["Fixed"] = new()
        {
            [AppLanguage.English] = "Fixed",
            [AppLanguage.Russian] = "Фиксированная",
            [AppLanguage.Ukrainian] = "Фіксована",
            [AppLanguage.Czech] = "Fixní"
        },
        ["HourlyRate"] = new()
        {
            [AppLanguage.English] = "Rate, Kč/h",
            [AppLanguage.Russian] = "Ставка, Kč/h",
            [AppLanguage.Ukrainian] = "Ставка, Kč/h",
            [AppLanguage.Czech] = "Sazba, Kč/h"
        },
        ["FixedAmount"] = new()
        {
            [AppLanguage.English] = "Amount, Kč",
            [AppLanguage.Russian] = "Сумма, Kč",
            [AppLanguage.Ukrainian] = "Сума, Kč",
            [AppLanguage.Czech] = "Částka, Kč"
        },
        ["Comment"] = new()
        {
            [AppLanguage.English] = "Comment",
            [AppLanguage.Russian] = "Комментарий",
            [AppLanguage.Ukrainian] = "Коментар",
            [AppLanguage.Czech] = "Komentář"
        },
        ["Cancel"] = new()
        {
            [AppLanguage.English] = "Cancel",
            [AppLanguage.Russian] = "Отмена",
            [AppLanguage.Ukrainian] = "Скасувати",
            [AppLanguage.Czech] = "Zrušit"
        },
        ["Save"] = new()
        {
            [AppLanguage.English] = "Save",
            [AppLanguage.Russian] = "Сохранить",
            [AppLanguage.Ukrainian] = "Зберегти",
            [AppLanguage.Czech] = "Uložit"
        },
        ["LoadPhoto"] = new()
        {
            [AppLanguage.English] = "Load Photo",
            [AppLanguage.Russian] = "Загрузить фото",
            [AppLanguage.Ukrainian] = "Завантажити фото",
            [AppLanguage.Czech] = "Nahrát fotku"
        },
        ["DeletePhoto"] = new()
        {
            [AppLanguage.English] = "Delete Photo",
            [AppLanguage.Russian] = "Удалить фото",
            [AppLanguage.Ukrainian] = "Видалити фото",
            [AppLanguage.Czech] = "Smazat fotku"
        },
        ["SearchByPassport"] = new()
        {
            [AppLanguage.English] = "Search employee by passport",
            [AppLanguage.Russian] = "Поиск сотрудника по паспорту",
            [AppLanguage.Ukrainian] = "Пошук співробітника за паспортом",
            [AppLanguage.Czech] = "Hledání zaměstnance podle pasu"
        },
        ["Find"] = new()
        {
            [AppLanguage.English] = "Find",
            [AppLanguage.Russian] = "Найти",
            [AppLanguage.Ukrainian] = "Знайти",
            [AppLanguage.Czech] = "Najít"
        },
        ["WorkPeriod"] = new()
        {
            [AppLanguage.English] = "Work Period",
            [AppLanguage.Russian] = "Период работы",
            [AppLanguage.Ukrainian] = "Період роботи",
            [AppLanguage.Czech] = "Pracovní období"
        },
        ["DateFrom"] = new()
        {
            [AppLanguage.English] = "From (dd.mm.yyyy)",
            [AppLanguage.Russian] = "С (дд.мм.гггг)",
            [AppLanguage.Ukrainian] = "Від (дд.мм.рррр)",
            [AppLanguage.Czech] = "Od (dd.mm.rrrr)"
        },
        ["DateTo"] = new()
        {
            [AppLanguage.English] = "To (dd.mm.yyyy)",
            [AppLanguage.Russian] = "По (дд.мм.гггг)",
            [AppLanguage.Ukrainian] = "До (дд.мм.рррр)",
            [AppLanguage.Czech] = "Do (dd.mm.rrrr)"
        },
        ["WorkPlaces"] = new()
        {
            [AppLanguage.English] = "Work Places",
            [AppLanguage.Russian] = "Места работы",
            [AppLanguage.Ukrainian] = "Місця роботи",
            [AppLanguage.Czech] = "Místa práce"
        },
        ["City"] = new()
        {
            [AppLanguage.English] = "City",
            [AppLanguage.Russian] = "Город",
            [AppLanguage.Ukrainian] = "Місто",
            [AppLanguage.Czech] = "Město"
        },
        ["Address"] = new()
        {
            [AppLanguage.English] = "Address",
            [AppLanguage.Russian] = "Адрес",
            [AppLanguage.Ukrainian] = "Адреса",
            [AppLanguage.Czech] = "Adresa"
        },
        ["Object"] = new()
        {
            [AppLanguage.English] = "Object (optional)",
            [AppLanguage.Russian] = "Объект (необязательно)",
            [AppLanguage.Ukrainian] = "Об'єкт (необов'язково)",
            [AppLanguage.Czech] = "Objekt (volitelné)"
        },
        ["ReceiptPhoto"] = new()
        {
            [AppLanguage.English] = "Receipt photo",
            [AppLanguage.Russian] = "Фото росписки о получении денег",
            [AppLanguage.Ukrainian] = "Фото розписки про отримання грошей",
            [AppLanguage.Czech] = "Fotka potvrzení o přijetí peněz"
        },
        ["SavePayment"] = new()
        {
            [AppLanguage.English] = "Save Payment",
            [AppLanguage.Russian] = "Сохранить начисление",
            [AppLanguage.Ukrainian] = "Зберегти нарахування",
            [AppLanguage.Czech] = "Uložit výpočet"
        },
        ["Reports"] = new()
        {
            [AppLanguage.English] = "Payment Reports",
            [AppLanguage.Russian] = "Отчёты по начислениям",
            [AppLanguage.Ukrainian] = "Звіти про нарахування",
            [AppLanguage.Czech] = "Zprávy o výpočtech"
        },
        ["SearchByPassportShort"] = new()
        {
            [AppLanguage.English] = "Search by passport",
            [AppLanguage.Russian] = "Поиск по паспорту",
            [AppLanguage.Ukrainian] = "Пошук за паспортом",
            [AppLanguage.Czech] = "Hledat podle pasu"
        },
        ["SearchByDate"] = new()
        {
            [AppLanguage.English] = "Search by date (dd.mm.yyyy)",
            [AppLanguage.Russian] = "Поиск по дате (дд.мм.гггг)",
            [AppLanguage.Ukrainian] = "Пошук за датою (дд.мм.рррр)",
            [AppLanguage.Czech] = "Hledat podle data (dd.mm.rrrr)"
        },
        ["ClearSearch"] = new()
        {
            [AppLanguage.English] = "Clear",
            [AppLanguage.Russian] = "Сбросить",
            [AppLanguage.Ukrainian] = "Скинути",
            [AppLanguage.Czech] = "Vymazat"
        },
        ["DeletePayment"] = new()
        {
            [AppLanguage.English] = "Delete Payment",
            [AppLanguage.Russian] = "Удалить начисление",
            [AppLanguage.Ukrainian] = "Видалити нарахування",
            [AppLanguage.Czech] = "Smazat výpočet"
        },
        ["Payment"] = new()
        {
            [AppLanguage.English] = "Payment",
            [AppLanguage.Russian] = "Начисление",
            [AppLanguage.Ukrainian] = "Нарахування",
            [AppLanguage.Czech] = "Výpočet"
        },
        ["ReceiptPhotoShort"] = new()
        {
            [AppLanguage.English] = "Receipt Photo",
            [AppLanguage.Russian] = "Фото росписки",
            [AppLanguage.Ukrainian] = "Фото розписки",
            [AppLanguage.Czech] = "Fotka potvrzení"
        },
        ["AddPhoto"] = new()
        {
            [AppLanguage.English] = "Add Photo",
            [AppLanguage.Russian] = "Добавить фото",
            [AppLanguage.Ukrainian] = "Додати фото",
            [AppLanguage.Czech] = "Přidat fotku"
        },
        ["NoPhoto"] = new()
        {
            [AppLanguage.English] = "No Photo",
            [AppLanguage.Russian] = "Нет фото",
            [AppLanguage.Ukrainian] = "Немає фото",
            [AppLanguage.Czech] = "Žádná fotka"
        },
        ["Created"] = new()
        {
            [AppLanguage.English] = "Created",
            [AppLanguage.Russian] = "Создано",
            [AppLanguage.Ukrainian] = "Створено",
            [AppLanguage.Czech] = "Vytvořeno"
        },
        ["Settings"] = new()
        {
            [AppLanguage.English] = "Settings",
            [AppLanguage.Russian] = "Настройки",
            [AppLanguage.Ukrainian] = "Налаштування",
            [AppLanguage.Czech] = "Nastavení"
        },
        ["Language"] = new()
        {
            [AppLanguage.English] = "Language",
            [AppLanguage.Russian] = "Язык",
            [AppLanguage.Ukrainian] = "Мова",
            [AppLanguage.Czech] = "Jazyk"
        },
        ["SelectLanguage"] = new()
        {
            [AppLanguage.English] = "Select interface language",
            [AppLanguage.Russian] = "Выберите язык интерфейса",
            [AppLanguage.Ukrainian] = "Оберіть мову інтерфейсу",
            [AppLanguage.Czech] = "Vyberte jazyk rozhraní"
        },
        ["LangRussian"] = new()
        {
            [AppLanguage.English] = "Russian",
            [AppLanguage.Russian] = "Русский",
            [AppLanguage.Ukrainian] = "Російська",
            [AppLanguage.Czech] = "Ruština"
        },
        ["LangUkrainian"] = new()
        {
            [AppLanguage.English] = "Ukrainian",
            [AppLanguage.Russian] = "Украинский",
            [AppLanguage.Ukrainian] = "Українська",
            [AppLanguage.Czech] = "Ukrajinština"
        },
        ["LangCzech"] = new()
        {
            [AppLanguage.English] = "Czech",
            [AppLanguage.Russian] = "Чешский",
            [AppLanguage.Ukrainian] = "Чеська",
            [AppLanguage.Czech] = "Čeština"
        },
        ["LangEnglish"] = new()
        {
            [AppLanguage.English] = "English",
            [AppLanguage.Russian] = "Английский",
            [AppLanguage.Ukrainian] = "Англійська",
            [AppLanguage.Czech] = "Angličtina"
        },
        ["HomeTooltip"] = new()
        {
            [AppLanguage.English] = "Main Menu",
            [AppLanguage.Russian] = "Главное меню",
            [AppLanguage.Ukrainian] = "Головне меню",
            [AppLanguage.Czech] = "Hlavní nabídka"
        },
        ["EmployeesTooltip"] = new()
        {
            [AppLanguage.English] = "Employees",
            [AppLanguage.Russian] = "Сотрудники",
            [AppLanguage.Ukrainian] = "Співробітники",
            [AppLanguage.Czech] = "Zaměstnanci"
        },
        ["DatabaseTooltip"] = new()
        {
            [AppLanguage.English] = "Database",
            [AppLanguage.Russian] = "База данных",
            [AppLanguage.Ukrainian] = "База даних",
            [AppLanguage.Czech] = "Databáze"
        },
        ["SalaryTooltip"] = new()
        {
            [AppLanguage.English] = "Salary Payment",
            [AppLanguage.Russian] = "Начисление зарплаты",
            [AppLanguage.Ukrainian] = "Нарахування зарплати",
            [AppLanguage.Czech] = "Výpočet mzdy"
        },
        ["ReportsTooltip"] = new()
        {
            [AppLanguage.English] = "Reports",
            [AppLanguage.Russian] = "Отчёты",
            [AppLanguage.Ukrainian] = "Звіти",
            [AppLanguage.Czech] = "Zprávy"
        },
        ["SettingsTooltip"] = new()
        {
            [AppLanguage.English] = "Settings",
            [AppLanguage.Russian] = "Настройки",
            [AppLanguage.Ukrainian] = "Налаштування",
            [AppLanguage.Czech] = "Nastavení"
        },
        ["Ready"] = new()
        {
            [AppLanguage.English] = "Ready",
            [AppLanguage.Russian] = "Готов",
            [AppLanguage.Ukrainian] = "Готово",
            [AppLanguage.Czech] = "Připraveno"
        },
        ["FirstRunTitle"] = new()
        {
            [AppLanguage.English] = "Welcome!",
            [AppLanguage.Russian] = "Добро пожаловать!",
            [AppLanguage.Ukrainian] = "Ласкаво просимо!",
            [AppLanguage.Czech] = "Vítejte!"
        },
        ["FirstRunMessage"] = new()
        {
            [AppLanguage.English] = "Please select your language to continue",
            [AppLanguage.Russian] = "Пожалуйста, выберите язык для продолжения",
            [AppLanguage.Ukrainian] = "Будь ласка, оберіть мову для продовження",
            [AppLanguage.Czech] = "Vyberte prosím jazyk pro pokračování"
        },
        ["Continue"] = new()
        {
            [AppLanguage.English] = "Continue",
            [AppLanguage.Russian] = "Продолжить",
            [AppLanguage.Ukrainian] = "Продовжити",
            [AppLanguage.Czech] = "Pokračovat"
        }
    };

    public static AppLanguage CurrentLanguage { get; private set; } = AppLanguage.English;
    public static bool IsFirstRun { get; private set; } = true;

    static LocalizationService()
    {
        LoadSettings();
        UpdateResources();
    }

    public static string Get(string key)
    {
        if (Translations.TryGetValue(key, out var langDict))
        {
            if (langDict.TryGetValue(CurrentLanguage, out var text))
            {
                return text;
            }
        }
        return key;
    }

    public static void SetLanguage(AppLanguage language)
    {
        CurrentLanguage = language;
        IsFirstRun = false;
        SaveSettings();
        UpdateResources();
    }

    private static void UpdateResources()
    {
        var app = Application.Current;
        if (app is null) return;

        foreach (var translation in Translations)
        {
            app.Resources[translation.Key] = Get(translation.Key);
        }
    }

    private static void LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                if (settings is not null)
                {
                    CurrentLanguage = settings.Language;
                    IsFirstRun = settings.IsFirstRun;
                    return;
                }
            }
        }
        catch
        {
        }
        CurrentLanguage = AppLanguage.English;
        IsFirstRun = true;
    }

    private static void SaveSettings()
    {
        try
        {
            var dir = Path.GetDirectoryName(SettingsFilePath);
            if (dir is not null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var settings = new AppSettings
            {
                Language = CurrentLanguage,
                IsFirstRun = IsFirstRun
            };
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFilePath, json);
        }
        catch
        {
        }
    }
}

public class AppSettings
{
    public AppLanguage Language { get; set; } = AppLanguage.English;
    public bool IsFirstRun { get; set; } = true;
}