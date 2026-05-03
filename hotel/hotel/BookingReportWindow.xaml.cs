using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using hotel.Data;
using System.IO;
using hotel.Models;
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
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using WordBody = DocumentFormat.OpenXml.Wordprocessing.Body;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using WordItalic = DocumentFormat.OpenXml.Wordprocessing.Italic;
using WordBold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using System.Collections;
using QuestPDF.Infrastructure;
using System.Diagnostics;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Elements.Table;

namespace hotel
{
    /// <summary>
    /// Логика взаимодействия для BookingReportWindow.xaml
    /// </summary>
    public partial class BookingReportWindow : Window
    {
        private Employee _currentEmpl;
        private List<Hotel> _hotels = new();

        private List<Book> _currentBooks;
        private string _reportTitle;
        private string _reportDateRange;
        private decimal _reportTotal;
        public BookingReportWindow(Employee employee)
        {
            InitializeComponent();
            _currentEmpl = employee;

            QuestPDF.Settings.License = LicenseType.Community;

            emplName.Text += " " + employee.Name + " " + employee.Lastname;
            emplRole.Text += " " + employee.IdroleNavigation.Name;
            LoadHotelsAsync();

            SetViewMode(false);
        }

        private async Task LoadHotelsAsync()
        {
            try
            {
                _hotels = await Api.GetHotels();
                hotelCB.ItemsSource = _hotels;
                hotelCB.DisplayMemberPath = "Name";
                hotelCB.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отелей: {ex.Message}", "Уведомление");
            }
        }

        private async Task LoadReportData()
        {
            try
            {
                if (hotelCB.SelectedItem == null)
                {
                    MessageBox.Show("Выберите отель для формирования отчёта", "Уведомление");
                    return;
                }

                var selectedHotel = (Hotel)hotelCB.SelectedItem;
                var start = startDatePicker.SelectedDate?.Date;
                var end = endDatePicker.SelectedDate?.Date;

                var books = await Api.GetBookingsByHotelAndPeriod(selectedHotel.Idhotel, start, end);

                if (books == null || books.Count == 0)
                {
                    SetViewMode(false);
                    totalAmountLabel.Text = "Итого: 0 руб.";
                    MessageBox.Show("Данные за выбранный период не найдены.", "Уведомление");
                    _currentBooks = null;
                }
                else
                {
                    SetViewMode(true);
                    reportGrid.ItemsSource = books;
                    var total = books.Sum(b => b.TotalAmount);
                    totalAmountLabel.Text = $"Итого: {total} руб.";

                    var title = $"ЖУРНАЛ ПО ПРОЖИВАНИЮ КЛИЕНТОВ В ОТЕЛЕ \"{selectedHotel.Name}\" С " +
                            $"{(start?.ToString("dd.MM.yyyy") ?? "____")} ПО " +
                            $"{(end?.ToString("dd.MM.yyyy") ?? "____")} Г.";
                    ((TextBlock)FindName("Title")).Text = title;

                    _currentBooks = books;
                    _reportTotal = total;
                    _reportTitle = $"ЖУРНАЛ ПО ПРОЖИВАНИЮ КЛИЕНТОВ В ОТЕЛЕ \"{selectedHotel.Name}\"";
                    _reportDateRange = $"С {start?.ToString("dd.MM.yyyy") ?? "__.__.____"} ПО {end?.ToString("dd.MM.yyyy") ?? "__.__.____"} Г.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось найти данные для отчета!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void SetViewMode(bool hasData)
        {
            if (hasData)
            {
                reportGrid.Visibility = Visibility.Visible;
                notfound.Visibility = Visibility.Collapsed;
            }
            else
            {
                reportGrid.Visibility = Visibility.Collapsed;
                notfound.Visibility = Visibility.Visible;
            }
        }


           
        private async void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            await LoadReportData();
        }
        private async void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            await LoadReportData();

            if (_currentBooks != null && _currentBooks.Any())
            {
                GeneratePdf(_currentBooks, _reportTitle, _reportDateRange, _reportTotal);
            }
            else
            {
                MessageBox.Show("Нет данных для экспорта", "Уведомление");
            }
        }

        private void GeneratePdf(List<Book> books, string title, string dateRange, decimal total)
        {
            string fileName = $"Отчёт_по_проживанию_{DateTime.Now:yyyy-MM-dd_HH-mm}.pdf";
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            string logoPath = @"C:\Users\Пользователь\source\repos\hotel\hotel\images\local-hotel_118960.png";

            string employeeName = $"{_currentEmpl.Lastname} {_currentEmpl.Name} {_currentEmpl.Patronymic}".Trim();

            try
            {
                QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(20, Unit.Millimetre);
                        page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                        page.Content().Column(col =>
                        {
                            col.Item().Row(row =>
                            {
                                if (File.Exists(logoPath))
                                {
                                    row.RelativeItem(1).Height(40).AlignMiddle().Image(logoPath).FitHeight();
                                }

                                row.RelativeItem(3).AlignCenter().Column(headerCol =>
                                {
                                    headerCol.Item().Text(title.ToUpper()).FontSize(16).Bold();
                                    headerCol.Item().Text(dateRange).FontSize(14).SemiBold();
                                });
                            });

                            col.Item().PaddingVertical(20).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1f);   
                                    columns.RelativeColumn(1.5f); 
                                    columns.RelativeColumn(3f);  
                                    columns.RelativeColumn(1f);  
                                    columns.RelativeColumn(2f);
                                    columns.RelativeColumn(2f); 
                                    columns.RelativeColumn(1.5f);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("№").Bold();
                                    header.Cell().Element(CellStyle).Text("Номер").Bold();
                                    header.Cell().Element(CellStyle).Text("Ф.И.О. клиента").Bold();
                                    header.Cell().Element(CellStyle).Text("Кол-во гостей").Bold();
                                    header.Cell().Element(CellStyle).Text("Въезд").Bold();
                                    header.Cell().Element(CellStyle).Text("Выезд").Bold();
                                    header.Cell().Element(CellStyle).Text("Сумма (руб.)").Bold();
                                });

                                foreach (var book in books)
                                {
                                    table.Cell().Element(CellStyle).Text(book.Idbook.ToString());
                                    table.Cell().Element(CellStyle).Text(book.Room?.Name ?? "—");

                                    string fio = "Не указан";
                                    if (book.Client != null)
                                        fio = $"{book.Client.Lastname} {book.Client.Name} {book.Client.Patronymic}".Trim();
                                    else if (!string.IsNullOrEmpty(book.GuestFullName))
                                        fio = book.GuestFullName;

                                    table.Cell().Element(CellStyle).Text(fio);
                                    table.Cell().Element(CellStyle).Text(book.NumOfGuests.ToString());
                                    table.Cell().Element(CellStyle).Text(book.CheckInDate.ToString("dd.MM.yyyy"));
                                    table.Cell().Element(CellStyle).Text(book.DepartureDate.ToString("dd.MM.yyyy"));
                                    table.Cell().Element(CellStyle).Text(book.TotalAmount.ToString("N0"));
                                }
                            });

                            
                            col.Item().PaddingTop(20).Column(footerCol =>
                            {
                                footerCol.Item().AlignRight().Text($"Итого: {total:N0} руб.").FontSize(14).Bold();

                                footerCol.Item().PaddingTop(40).Row(row =>
                                {
                                    row.RelativeItem().Text("Журнал заполнен:").Bold();
                                    row.RelativeItem(2).AlignBottom().Text(employeeName);
                                });

                                footerCol.Item().PaddingTop(10).Row(row =>
                                {
                                    row.RelativeItem().Text("Дата заполнения:").Bold();
                                    row.RelativeItem(2).AlignBottom().Row(innerRow =>
                                    {
                                        innerRow.RelativeItem().Text(DateTime.Now.ToString("dd.MM.yyyy"));
                                        innerRow.RelativeItem().LineHorizontal(1f);
                                    });
                                });
                            });
                        });

                    });
                }).GeneratePdf(path);

                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                MessageBox.Show($"Отчёт сохранён:\n{fileName}", "Уведомление");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании PDF: {ex.Message}", "Уведомление");
            }
        }

        private IContainer CellStyle(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Black).Padding(6).AlignCenter().AlignMiddle();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            HeadHotelManagerWindow headHotelManagerWindow = new HeadHotelManagerWindow(this._currentEmpl);
            headHotelManagerWindow.Show();
        }

    }
}
