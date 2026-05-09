using hotel.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace hotel.Data
{
    public static class Api
    {
        public static async Task<Employee?> Auth(string email, string password)
        {
            try
            {

                using (var clint = new HttpClient())
                {
                    var result = clint.GetAsync($"https://localhost:7042/api/Employee/{email}, {password}").Result;

                    if(result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return JsonConvert.DeserializeObject<Employee>((await result.Content.ReadAsStringAsync()));
                    }
                    return null;    
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Hello, world!");
                return null;
            }
        } 

        public static async Task<List<Hotel>> GetHotels()
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync($"https://localhost:7042/api/Hotel/");
                if(result.IsSuccessStatusCode)
                {
                    var json = await result.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Hotel>>(json) ?? new List<Hotel>();
                }
                return new List<Hotel>();
            }
        }

        public static async Task<string> updateHotel(int id, Hotel hotel)
        {
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(hotel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"https://localhost:7042/api/Hotel/{id}";
            try
            {
                var response = await client.PutAsync(url, content);
                if (response.IsSuccessStatusCode)
                    return null;

                var error = await response.Content.ReadAsStringAsync();
                return error;
            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }

        public static async Task<string?> AddHotel(Hotel hotel)
        {
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(hotel);
            var content = new StringContent (json, Encoding.UTF8, "application/json");

            var respone = await client.PostAsync("https://localhost:7042/api/Hotel", content);

            if (respone.IsSuccessStatusCode)
                return null;
            return await respone.Content.ReadAsStringAsync();
        }

        public static async Task<List<Employee>> GetEmployeesForCurrentUser(int currentEmployeeId)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"https://localhost:7042/api/Employee/for-user/{currentEmployeeId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Employee>>(json) ?? new List<Employee>();
            }
            return new List<Employee>();
        }

        public static async Task<string?> AddEmployee(Employee employee)
        {
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(employee);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7042/api/Employee", content);

            if (response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string?> UpdateEmployee(int id, Employee employee)
        {
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(employee);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"https://localhost:7042/api/Employee/{id}", content);

            if (response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<List<Floor>> GetFloors()
        {
            using var client = new HttpClient();
            var res = await client.GetAsync("https://localhost:7042/api/Floor");
            return res.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<Floor>>(await res.Content.ReadAsStringAsync())
                : new List<Floor>();
        }

        public static async Task<List<Category>> GetCategories()
        {
            using var client = new HttpClient();
            var res = await client.GetAsync("https://localhost:7042/api/Category");
            return res.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<Category>>(await res.Content.ReadAsStringAsync())
                : new List<Category>();
        }

        public static async Task<List<StatusRoom>> GetStatuses()
        {
            using var client = new HttpClient();
            var res = await client.GetAsync("https://localhost:7042/api/StatusRoom");
            return res.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<StatusRoom>>(await res.Content.ReadAsStringAsync())
                : new List<StatusRoom>();
        }

        public static async Task<string> AddRoom(Room room)
        {
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(room);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await client.PostAsync("https://localhost:7042/api/Room", content);
            return res.IsSuccessStatusCode ? null : await res.Content.ReadAsStringAsync();
        }

        public static async Task<string> UpdateRoom(int id, Room room)
        {
            using var client = new HttpClient();
            var updatedRoom = new Room
            {
                Idroom = room.Idroom,
                Name = room.Name,
                Floorid = room.Floorid,
                IdCategory = room.IdCategory,
                StatusId = room.StatusId,
                Hotelid = room.Hotelid
            };
            var json = JsonConvert.SerializeObject(updatedRoom);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await client.PutAsync($"https://localhost:7042/api/Room/{updatedRoom.Idroom}", content);
            return res.IsSuccessStatusCode ? null : await res.Content.ReadAsStringAsync();
        }
        public static async Task<List<Room>> GetRooms()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7042/api/Room");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Room>>(json) ?? new List<Room>();
            }

            return new List<Room>();
        }

        public static async Task<List<Book>> GetBookingsByHotelAndPeriod(int? hotelId, DateTime? startDate = null, DateTime? endDate = null)
        {
            using var client = new HttpClient();
            var url = $"https://localhost:7042/api/Booking/hotel/{hotelId}/period";
            if (startDate.HasValue) url += $"?startDate={startDate.Value:yyyy-MM-dd}";
            if (endDate.HasValue)
                url += startDate.HasValue ? "&" : "?";
            url += $"endDate={endDate.Value:yyyy-MM-dd}";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Book>>(json) ?? new List<Book>();
            }
            return new List<Book>();
        }

        public static async Task<List<Role>> GetRoles()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("https://localhost:7042/api/Role");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Role>>(json) ?? new List<Role>();
            }
            return new List<Role>();
        }

        public static async Task<List<Book>> GetBookingsForCurrentHotel(int employeeId)
        {
            using var client = new HttpClient();
            var url = $"https://localhost:7042/api/Booking/hotel/current?employeeId={employeeId}";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                // Используем Newtonsoft.Json, как в остальных методах
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                return JsonConvert.DeserializeObject<List<Book>>(json, settings) ?? new List<Book>();
            }
            return new List<Book>();
        }

        public static async Task<Book?> AddBooking(Book booking)
            {
            using var client = new HttpClient();
            var cleanBooking = new Book
            {
                EmployeeId = booking.EmployeeId,
                RoomId = booking.RoomId,
                CheckInDate = booking.CheckInDate,
                DepartureDate = booking.DepartureDate,
                BookingDate = booking.BookingDate,
                Payment = booking.Payment,
                StatusBook = booking.StatusBook,
                ClientId = booking.ClientId,

                BookServices = new List<BookService>(),

                Employee = null,
                Client = null,
                Room = null
            };
            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(cleanBooking, settings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7042/api/Booking", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Book>(responseJson);
            }
            else
            {
                // Читаем ошибку для отладки
                var error = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Ошибка сервера: {error}");
                return null;
            }
        }

        public static async Task<string> UpdateBooking(int bookingId, sbyte newStatus)
        {
            using var client = new HttpClient();

            var url = $"https://localhost:7042/api/Booking/{bookingId}/status?status={newStatus}";
            try
            {
                var response = await client.PutAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    return null;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return string.IsNullOrEmpty(errorContent) ? $"Ошибка: {response.StatusCode}" : errorContent;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static async Task<List<Clint>> GetClients()
        {
            using var client = new HttpClient();
            var url = "https://localhost:7042/api/Client";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Clint>>(json) ?? new List<Clint>();
            }
            return new List<Clint>();
        }

        public static async Task<List<Guest>> GetGuests()
        {
            using var client = new HttpClient();
            var url = "https://localhost:7042/api/Guest";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Guest>>(json) ?? new List<Guest>();
            }
            return new List<Guest>();
        }

        public static async Task<List<Service>> GetServices()
        {
            using var client = new HttpClient();
            var url = "https://localhost:7042/api/Service";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Service>>(json) ?? new List<Service>();
            }
            return new List<Service>();
        }

        public static async Task<string?> AddServiceToBooking(BookService bookService)
        {
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(bookService);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7042/api/BookService", content);
            return response.IsSuccessStatusCode ? null : await response.Content.ReadAsStringAsync();
        }

        public static async Task<string?> AddGuestToBooking(GuestBook guestBook)
        {
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(guestBook);
            var content = new StringContent(json, Encoding.UTF8, "application/json"); // ← ПРАВИЛЬНО
            var response = await client.PostAsync("https://localhost:7042/api/GuestBook", content);
            if (response.IsSuccessStatusCode)
                return null;
            var error = await response.Content.ReadAsStringAsync();
            return $"Ошибка сервера: {response.StatusCode}\n{error}";
        }

        public static async Task<Guest?> AddGuest(Guest guest)
        {
            using var client = new HttpClient();

            var options = new System.Text.Json.JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(guest, options);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://localhost:7042/api/Guest", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<Guest>(responseJson, options);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка при создании гостя: {error}", "Ошибка");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка соединения: {ex.Message}", "Ошибка");
                return null;
            }
        }

        public static async Task<Clint?> AddClient(Clint client)
        {
            using var clientHttp = new HttpClient();

            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(client, settings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await clientHttp.PostAsync("https://localhost:7042/api/Client/Register", content);

            if (res.IsSuccessStatusCode)
            {
                var responseContent = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Clint>(responseContent);
            }
            else
            {
                return null;
            }
        }

        public static async Task<(double? rating, int count)?> GetHotelRatingAsync(int hotelId)
        {
            using var client = new HttpClient();
            try
            {
                var response = await client.GetAsync($"https://localhost:7042/api/Hotel/{hotelId}/stars");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var obj = JsonConvert.DeserializeObject<JObject>(json);
                    double? rating = obj["rating"]?.Value<double?>();
                    int count = obj["count"]?.Value<int>() ?? 0;

                    return (rating, count);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки рейтинга: {ex.Message}");
                return null;
            }
        }

        public static async Task<List<dynamic>> GetHotelReviewsAsync(int hotelId)
        {
            using var client = new HttpClient();
            try
            {
                var response = await client.GetAsync($"https://localhost:7042/api/Review/hotel/{hotelId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<dynamic>>(json) ?? new List<dynamic>();
                }
                return new List<dynamic>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки отзывов: {ex.Message}");
                return new List<dynamic>();
            }
        }

        public static async Task<List<byte[]>> GetRoomPhotosAsync(int roomId)
        {
            using var client = new HttpClient();
            try
            {
                var response = await client.GetAsync($"https://localhost:7042/api/Room/{roomId}/photos");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<byte[]>>(json) ?? new List<byte[]>();
                }
                return new List<byte[]>();
            }
            catch
            {
                return new List<byte[]>();
            }
        }

        public static async Task<string> UpdateService(Service service)
        {
            using var client = new HttpClient();
            try
            {
                var json = JsonConvert.SerializeObject(service);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(
                    $"https://localhost:7042/api/Service/{service.Idservice}",
                    content
                );

                if (response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static async Task<string> AddService(Service service)
        {
            using var client = new HttpClient();
            try
            {
                var json = JsonConvert.SerializeObject(service);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://localhost:7042/api/Service", content);

                if (response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
