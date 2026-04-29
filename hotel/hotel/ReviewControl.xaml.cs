using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hotel
{
    /// <summary>
    /// Логика взаимодействия для ReviewControl.xaml
    /// </summary>
    public partial class ReviewControl : UserControl
    {
        public ReviewControl()
        {
            InitializeComponent();
        }

        public void SetData(string commentText, string clientFullName, int stars, DateTime createdAt)
        {
            CommentTextBlock.Text = commentText ?? "Без текста";
            ClientNameBlock.Text = string.IsNullOrEmpty(clientFullName) ? "Аноним" : clientFullName;
            DateBlock.Text = createdAt.ToString("dd.MM.yyyy");

            int starCount = Math.Max(1, Math.Min(5, stars));
            StarsPanel.ItemsSource = new int[starCount];
        }
    }
}
