using Spectre.Console;
using Spectre.Console.Json;
using System.Collections.Immutable;
using YuDB.Security;

namespace YuDB.Views
{
    public class View : AbstractView
    {
        private static int MAX_PASSWORD_SIZE = 512;

        private static int MAX_USERNAME_SIZE = 512;

        private static Func<string, ValidationResult> PASSWORD_VALIDATOR = (string password) =>
        {
            if (password.Length < 8 || password.Length > MAX_PASSWORD_SIZE)
                return ValidationResult.Error($"[italic red]Password length must be between 8 and {MAX_PASSWORD_SIZE}, inclusive[/]");
            else
                return ValidationResult.Success();
        };

        private static Func<string, ValidationResult> USERNAME_VALIDATOR = (string username) =>
                {
                    if (username.Length < 1 || username.Length > MAX_USERNAME_SIZE)
                        return ValidationResult.Error($"[italic red]Username length must be between 1 and {MAX_USERNAME_SIZE}, inclusive[/]");
                    else
                        return ValidationResult.Success();
                };

        public override List<string> ChangePassword()
        {
            AnsiConsole.Write(new Rule("Change Password"));
            var oldPasswordPrompt = new TextPrompt<string>("Old password:")
                .Validate(PASSWORD_VALIDATOR)
                .Secret();
            var newPasswordPrompt = new TextPrompt<string>("New password:")
                .Validate(PASSWORD_VALIDATOR)
                .Secret();
            return ImmutableArray.Create(
                AnsiConsole.Prompt(oldPasswordPrompt),
                AnsiConsole.Prompt(newPasswordPrompt))
                .ToList();
        }

        public override string ChangeUsername()
        {
            AnsiConsole.Write(new Rule("Change Username"));
            var newUsernamePrompt = new TextPrompt<string>("New username:")
                .Validate(USERNAME_VALIDATOR);
            return AnsiConsole.Prompt(newUsernamePrompt);
        }

        public override void Clear()
        {
            AnsiConsole.Clear();
        }

        public override void DisplayError(string error)
        {
            var panel = new Panel(new Markup($"[italic red]{error}[/]"))
                .Header("Error")
                .BorderColor(Color.Red)
                .Expand();
            AnsiConsole.Write(panel);
        }

        public override void DisplayOutput(string output)
        {
            var json = new JsonText(output);
            var panel = new Panel(json)
                .Header("Result")
                .Expand();
            AnsiConsole.Write(panel);
        }

        public override string GetUserPrompt()
        {
            AnsiConsole.Write("> ");
            return Console.ReadLine()!;
        }

        public override Credentials SignIn(Func<Credentials, bool> authenticate)
        {
            AnsiConsole.Write(new Rule("Sign In"));
            while (true)
            {
                var credentials = GetCredentials();
                if (authenticate(credentials))
                    return credentials;
                AnsiConsole.Write(new Markup("[italic red]Invalid credentials, please try again\n[/]"));
            }
        }

        public override Credentials SignUp()
        {
            AnsiConsole.Write(new Rule("Sign Up"));
            return GetCredentials();
        }

        public override void Welcome()
        {
            var title = new FigletText("YuDB").Color(Color.Green).Centered();

            var creators = new Table();
            creators.AddColumns("", "", "")
                .AddRow("Khalid Nusserat", "Hamza Idries", "Ayman Malkawi")
                .HideHeaders()
                .Centered();

            var table = new Table();
            table.AddColumn(new TableColumn("").Centered())
                .AddRow(title)
                .AddRow("Version 1.0.0")
                .AddRow(creators)
                .HideHeaders()
                .BorderStyle("conceal");

            AnsiConsole.Write(table);
        }

        private Credentials GetCredentials()
        {
            var usernamePrompt = new TextPrompt<string>("Username:")
                .PromptStyle("green")
                .Validate(USERNAME_VALIDATOR);
            var passwordPrompt = new TextPrompt<string>("Password:")
                .Validate(PASSWORD_VALIDATOR)
                .Secret();
            return new Credentials(
                    AnsiConsole.Prompt(usernamePrompt),
                    AnsiConsole.Prompt(passwordPrompt));
        }
    }
}