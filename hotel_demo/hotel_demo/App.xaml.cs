using hotel_demo.Model;
using System.Configuration;
using System.Data;
using System.Windows;

namespace hotel_demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static HotelContext contex { get; } = new HotelContext();
    }

}
