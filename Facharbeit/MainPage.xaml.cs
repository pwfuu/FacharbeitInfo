using MySql.Data.MySqlClient;
using System.Globalization;
using Microcharts;

namespace Facharbeit
{
    [QueryProperty("PersonID", "personID")]
    public partial class MainPage : ContentPage
    {
        const string connectionString = "Server=localhost;Database=Facharbeit;Uid=root;Pwd=Arthursql1!;";
        public int PersonID { get; set; }
        private int welcherChart;
        public MainPage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            String name = MySQLService.getUsername(PersonID);
            UsernameLabel.Text = name;

        }


        private async void GewichtEintragen_clicked(object sender, EventArgs e)
        {
            // DATEN einfügen

            try
            {
                double gewicht = double.Parse(entryGewicht.Text);
                DateTime date = datePicker.Date;


                MySQLService.InsertWeight(gewicht, date, PersonID);
                await DisplayAlert("Erfolg", "Gewicht wurde eingetragen", "Ok");



            }
            catch   //falls der Wert falsch ist also z.B. Buchstaben
            {
                await DisplayAlert("Fehler", "Es wurde ein ungültiger Wert angegeben.", "Schade Schokolade");
            }
            if (welcherChart == 30)
            {
                GraphAnzeigen30(sender, e);

            }
            if (welcherChart == 90)
            {
                GraphAnzeigen90(sender, e);
            }

        }



        private async void GraphAnzeigen30(object sender, EventArgs e)
        {
            double minValue = double.MaxValue; //damit der höchste Punkt der größte wert ist
            List<(double weight, DateTime date)> daten = MySQLService.getData30(PersonID);
            List<ChartEntry> chartData = new List<ChartEntry>();
            foreach ((double weight, DateTime date) in daten)
            {
                if (minValue < weight)
                {
                    minValue = weight;
                }
                ChartEntry chartEntry = new ChartEntry((float)weight)
                {
                    Label = date.ToString("yyyy-MM-dd"), //X-Achse
                    ValueLabel = weight.ToString(),      //Y-Achse
                };
                chartData.Add(chartEntry);
            }
            chartView.Chart = new LineChart
            {
                Entries = chartData,
                MinValue = (float)minValue,
            };
            welcherChart = 30;
        }

        private async void GraphAnzeigen90(object sender, EventArgs e)
        {
            double minValue = double.MaxValue;
            List<(double weight, DateTime date)> daten = MySQLService.getData90(PersonID);
            List<ChartEntry> chartData = new List<ChartEntry>();
            foreach ((double weight, DateTime date) in daten)
            {
                if (minValue < weight)
                {
                    minValue = weight;
                }
                ChartEntry chartEntry = new ChartEntry((float)weight)
                {
                    Label = date.ToString("yyyy-MM-dd"),
                    ValueLabel = weight.ToString(),
                };
                chartData.Add(chartEntry);
            }
            chartView.Chart = new LineChart
            {
                Entries = chartData,
                MinValue = (float)minValue,
            };
            welcherChart = 90;
        }
    }
}
    



