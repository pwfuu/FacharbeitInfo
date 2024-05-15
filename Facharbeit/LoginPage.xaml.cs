using Google.Protobuf;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg;
using System.Globalization;

namespace Facharbeit;

public partial class LoginPage : ContentPage
{
    string connectionString = "server=localhost;user=root;password=Arthursql1!;database=Facharbeit"; // ich muss angeben, mit welchem Server und welcher Facharbeit ich mich verbinden möchte

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void Login_clicked(object sender, EventArgs e) 
    {
        (Boolean successfull, int id) = MySQLService.login(entryUsername.Text, entryPassword.Text);
        if (successfull)
        {
            await Shell.Current.GoToAsync($"/MainPage?personID={id}"); 
        }
        else
        {
            await DisplayAlert("Fehler", "Falsches Passwort oder falscher Benutzername", "Ok");
        }
    }

    private async void Register_clicked(object sender, EventArgs e)
    {
        if (MySQLService.Register(entryUsername.Text, entryPasswordReg.Text, DateTime.Now, entryPasswordRegSafe.Text))
        {
            await DisplayAlert("Erfolg", "Der Account wurde erstellt", "Ok");
        }
        else
        {
            await DisplayAlert("Fehler", "Du bist schlecht, Passwörter müssen übereinstimmen", "Ok");
        }
    }
}