using System.Data;
using System.Windows;
using Npgsql;

namespace odb_course_project
{
    public partial class MainWindow : Window
    {
        private string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=1;Database=HotelDB";

        public MainWindow()
        {
            InitializeComponent();
            LoadGuests();
            LoadBookings();
            LoadPayments();
        }

        private void LoadGuests()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT * FROM Guests", connection);
                var adapter = new NpgsqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                GuestsDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void LoadBookings()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT * FROM Bookings", connection);
                var adapter = new NpgsqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                BookingsDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void LoadPayments()
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("SELECT * FROM Payments", connection);
                var adapter = new NpgsqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                PaymentsDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void AddGuestButton_Click(object sender, RoutedEventArgs e)
        {
            string name = PromptInput("Введіть ім'я гостя:");
            string email = PromptInput("Введіть email гостя:");
            string phone = PromptInput("Введіть телефон гостя:");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("INSERT INTO Guests (name, email, phone) VALUES (@name, @email, @phone)", connection);
                command.Parameters.AddWithValue("name", name);
                command.Parameters.AddWithValue("email", email);
                command.Parameters.AddWithValue("phone", phone);
                command.ExecuteNonQuery();
            }
            LoadGuests();
        }

        private void EditGuestButton_Click(object sender, RoutedEventArgs e)
        {
            if (GuestsDataGrid.SelectedItem is DataRowView row)
            {
                int guestId = (int)row["guest_id"];
                string newName = PromptInput("Введіть нове ім'я:", row["name"].ToString());
                string newEmail = PromptInput("Введіть новий email:", row["email"].ToString());
                string newPhone = PromptInput("Введіть новий телефон:", row["phone"].ToString());

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("UPDATE Guests SET name = @newName, email = @newEmail, phone = @newPhone WHERE guest_id = @guestId", connection);
                    command.Parameters.AddWithValue("guestId", guestId);
                    command.Parameters.AddWithValue("newName", newName);
                    command.Parameters.AddWithValue("newEmail", newEmail);
                    command.Parameters.AddWithValue("newPhone", newPhone);
                    command.ExecuteNonQuery();
                }
                LoadGuests();
            }
        }

        private void DeleteGuestButton_Click(object sender, RoutedEventArgs e)
        {
            if (GuestsDataGrid.SelectedItem is DataRowView row)
            {
                int guestId = (int)row["guest_id"];

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("DELETE FROM Guests WHERE guest_id = @guestId", connection);
                    command.Parameters.AddWithValue("guestId", guestId);
                    command.ExecuteNonQuery();
                }
                LoadGuests();
            }
        }

        private void AddBookingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string guestIdInput = PromptInput("Введіть ID гостя:");
                string roomIdInput = PromptInput("Введіть ID номера:");
                string check_in_date_input = PromptInput("Введіть дату початку (РРРР-ММ-ДД):");
                string check_out_date_input = PromptInput("Введіть дату закінчення (РРРР-ММ-ДД):");

                if (int.TryParse(guestIdInput, out int guestId) &&
                    int.TryParse(roomIdInput, out int roomId) &&
                    DateTime.TryParse(check_in_date_input, out DateTime check_in_date) &&
                    DateTime.TryParse(check_out_date_input, out DateTime check_out_date))
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        var command = new NpgsqlCommand("INSERT INTO Bookings (guest_id, room_id, check_in_date, check_out_date) VALUES (@guestId, @roomId, @check_in_date, @check_out_date)", connection);
                        command.Parameters.AddWithValue("guestId", guestId);
                        command.Parameters.AddWithValue("roomId", roomId);
                        command.Parameters.AddWithValue("check_in_date", check_in_date);
                        command.Parameters.AddWithValue("check_out_date", check_out_date);
                        command.Parameters.AddWithValue("status", "confirmed");
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("Бронювання успішно добавлено.");
                    
                    LoadBookings(); 
                }
                else
                {
                    MessageBox.Show("Помилка: Введіть коректні дані.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при додаванні бронювання: {ex.Message}");
            }
        }

        private void EditBookingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string bookingIdInput = PromptInput("Введіть ID бронювання для редагування:");

                if (int.TryParse(bookingIdInput, out int bookingId))
                {
                    string guestIdInput = PromptInput("Введіть новий ID гостя:");
                    string roomIdInput = PromptInput("Введіть новий ID номера:");
                    string check_in_date_input = PromptInput("Введіть нову дату початку (РРРР-ММ-ДД):");
                    string check_out_date_input = PromptInput("Введіть нову дату закінчення (РРРР-ММ-ДД):");

                    if (int.TryParse(guestIdInput, out int guestId) &&
                        int.TryParse(roomIdInput, out int roomId) &&
                        DateTime.TryParse(check_in_date_input, out DateTime check_in_date) &&
                        DateTime.TryParse(check_out_date_input, out DateTime check_out_date))
                    {
                        using (var connection = new NpgsqlConnection(connectionString))
                        {
                            connection.Open();
                            var command = new NpgsqlCommand("UPDATE Bookings SET guest_id = @guestId, room_id = @roomId, check_in_date = @check_in_date, check_out_date = @check_out_date WHERE booking_id = @bookingId", connection);
                            command.Parameters.AddWithValue("bookingId", bookingId);
                            command.Parameters.AddWithValue("guestId", guestId);
                            command.Parameters.AddWithValue("roomId", roomId);
                            command.Parameters.AddWithValue("check_in_date", check_in_date);
                            command.Parameters.AddWithValue("check_out_date", check_out_date);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Бронювання успішно оновлено.");
                                LoadBookings();
                            }
                            else
                            {
                                MessageBox.Show("Бронювання зі вказаним ID не знайдено.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Помилка: Введіть коректні дані.");
                    }
                }
                else
                {
                    MessageBox.Show("Помилка: Введіть коректний ID бронювання.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помитлка при редагуванні бронювання: {ex.Message}");
            }
        }

        private void DeleteBookingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string bookingIdInput = PromptInput("Введіть ID бронювання для видалення:");

                if (int.TryParse(bookingIdInput, out int bookingId))
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        var command = new NpgsqlCommand("DELETE FROM Bookings WHERE booking_id = @bookingId", connection);
                        command.Parameters.AddWithValue("bookingId", bookingId);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Бронювання успішно видалено.");
                            LoadBookings();
                        }
                        else
                        {
                            MessageBox.Show("Бронювання зі вказаним ID не знайдено.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Помилка: Введіть коректний ID бронювання.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при видаленні бронювання: {ex.Message}");
            }
        }

        private void AddPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string bookingIdInput = PromptInput("Введіть ID бронювання для платежу:");
                string amountInput = PromptInput("Введіть суму платежу:");
                string paymentDateInput = PromptInput("Введіть дату платежу (РРРР-ММ-ДД):");

                if (int.TryParse(bookingIdInput, out int bookingId) &&
                    decimal.TryParse(amountInput, out decimal amount) &&
                    DateTime.TryParse(paymentDateInput, out DateTime paymentDate))
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        var command = new NpgsqlCommand("INSERT INTO Payments (booking_id, amount, payment_date) VALUES (@bookingId, @amount, @paymentDate)", connection);
                        command.Parameters.AddWithValue("bookingId", bookingId);
                        command.Parameters.AddWithValue("amount", amount);
                        command.Parameters.AddWithValue("paymentDate", paymentDate);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Платіж успішно додано.");
                        LoadPayments();
                    }
                }
                else
                {
                    MessageBox.Show("Помилка: Введіть коректні дані.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при додаванні платежу: {ex.Message}");
            }
        }

        private void EditPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string paymentIdInput = PromptInput("Введіть ID платежу для редагування:");

                if (int.TryParse(paymentIdInput, out int paymentId))
                {
                    string bookingIdInput = PromptInput("Введіть новий ID бронювання:");
                    string amountInput = PromptInput("Введіть новуу суму платежу:");
                    string paymentDateInput = PromptInput("Введіть новуу дату платежу (РРРР-ММ-ДД):");

                    if (int.TryParse(bookingIdInput, out int bookingId) &&
                        decimal.TryParse(amountInput, out decimal amount) &&
                        DateTime.TryParse(paymentDateInput, out DateTime paymentDate))
                    {
                        using (var connection = new NpgsqlConnection(connectionString))
                        {
                            connection.Open();
                            var command = new NpgsqlCommand("UPDATE Payments SET booking_id = @bookingId, amount = @amount, payment_date = @paymentDate WHERE payment_id = @paymentId", connection);
                            command.Parameters.AddWithValue("paymentId", paymentId);
                            command.Parameters.AddWithValue("bookingId", bookingId);
                            command.Parameters.AddWithValue("amount", amount);
                            command.Parameters.AddWithValue("paymentDate", paymentDate);

                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Платіж успішно оновлено.");
                                LoadPayments();
                            }
                            else
                            {
                                MessageBox.Show("Платіж зі вказаним ID не знайдено.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Помилка: Введіть коректні дані.");
                    }
                }
                else
                {
                    MessageBox.Show("Помилка: Введіть коректний ID платежу.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при редагуванні платежу: {ex.Message}");
            }
        }

        private void DeletePaymentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string paymentIdInput = PromptInput("Введіть ID платежу для видалення:");

                if (int.TryParse(paymentIdInput, out int paymentId))
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        var command = new NpgsqlCommand("DELETE FROM Payments WHERE payment_id = @paymentId", connection);
                        command.Parameters.AddWithValue("paymentId", paymentId);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Платіж успішно видалено.");
                            LoadPayments();
                        }
                        else
                        {
                            MessageBox.Show("Платіж зі вказаним ID не знайдено.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Помилка: Введіть коректний ID платежу.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при видаленні платежу: {ex.Message}");
            }
        }

        private void AveragePriceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT AVG(rt.price_per_night) AS average_price_per_night
                                     FROM Bookings b
                                     JOIN Rooms r ON b.room_id = r.room_id
                                     JOIN Room_Types rt ON r.room_type_id = rt.room_type_id";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var result = cmd.ExecuteScalar();
                        MessageBox.Show($"Середня ціна за ніч: {result:C}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ConfirmedBookingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("SELECT * FROM Bookings WHERE status = @status", connection);
                    command.Parameters.AddWithValue("status", "confirmed");

                    var reader = command.ExecuteReader();
                    var confirmedBookings = new List<Booking>();

                    while (reader.Read())
                    {
                        confirmedBookings.Add(new Booking
                        {
                            BookingId = reader.GetInt32(reader.GetOrdinal("booking_id")),
                            RoomId = reader.GetInt32(reader.GetOrdinal("room_id")),
                            GuestId = reader.GetInt32(reader.GetOrdinal("guest_id")),
                            CheckInDate = reader.GetDateTime(reader.GetOrdinal("check_in_date")),
                            CheckOutDate = reader.GetDateTime(reader.GetOrdinal("check_out_date")),
                            Status = reader.GetString(reader.GetOrdinal("status"))
                        });
                    }

                    BookingsDataGrid.ItemsSource = confirmedBookings;
                    MessageBox.Show("Відображено всі підтверджені бронювання.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні підтверджених бронюваннях: {ex.Message}");
            }
        }

        private void GuestPaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string guestIdInput = PromptInput("Введіть ID гостя для відображення його платежів:");

                if (int.TryParse(guestIdInput, out int guestId))
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        var command = new NpgsqlCommand(@"SELECT p.payment_id, p.booking_id, p.amount, p.payment_date 
                                                  FROM Payments p
                                                  JOIN Bookings b ON p.booking_id = b.booking_id
                                                  WHERE b.guest_id = @guestId", connection);
                        command.Parameters.AddWithValue("guestId", guestId);

                        var reader = command.ExecuteReader();
                        var guestPayments = new List<Payment>();

                        while (reader.Read())
                        {
                            guestPayments.Add(new Payment
                            {
                                PaymentId = reader.GetInt32(reader.GetOrdinal("payment_id")),
                                BookingId = reader.GetInt32(reader.GetOrdinal("booking_id")),
                                Amount = reader.GetDecimal(reader.GetOrdinal("amount")),
                                PaymentDate = reader.GetDateTime(reader.GetOrdinal("payment_date"))
                            });
                        }

                        PaymentsDataGrid.ItemsSource = guestPayments;
                        MessageBox.Show("Відображено всі платежі для обраного гостя.");
                    }
                }
                else
                {
                    MessageBox.Show("Помилка: Введіть коректний ID гостя.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні платежів гостя: {ex.Message}");
            }
        }

        private void GuestsInHotelTodayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT g.name FROM Guests g
                                     JOIN Bookings b ON g.guest_id = b.guest_id
                                     WHERE @today BETWEEN b.check_in_date AND b.check_out_date";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("today", DateTime.Today);
                        var adapter = new NpgsqlDataAdapter(command);
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        GuestsDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
                MessageBox.Show("Виведено гостей, які знаходяться в готелі сьогодні.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні гостей: {ex.Message}");
            }
        }

        // 2. Гость, который бронирует чаще всего
        private void MostFrequentGuestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT g.name, COUNT(b.booking_id) AS booking_count
                                     FROM Guests g
                                     JOIN Bookings b ON g.guest_id = b.guest_id
                                     GROUP BY g.name
                                     ORDER BY booking_count DESC
                                     LIMIT 1";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        var adapter = new NpgsqlDataAdapter(command);
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        GuestsDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
                MessageBox.Show("Виведено гостя, який бронює найчастіше.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні інформації: {ex.Message}");
            }
        }

        // 3. Кількість людей, бронювавших у листопаді 2024
        private void BookingsInNovember2024Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT COUNT(DISTINCT guest_id) AS guest_count
                                     FROM Bookings
                                     WHERE check_in_date BETWEEN '2024-11-01' AND '2024-11-30'";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        var result = command.ExecuteScalar();
                        MessageBox.Show($"Кількість гостей, що бронювали у листопаді 2024: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні інформації: {ex.Message}");
            }
        }

        // 4. Номер, який бронюють найчастіше
        private void MostFrequentRoomButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT r.room_id, COUNT(b.booking_id) AS booking_count
                                     FROM Rooms r
                                     JOIN Bookings b ON r.room_id = b.room_id
                                     GROUP BY r.room_id
                                     ORDER BY booking_count DESC
                                     LIMIT 1";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        var adapter = new NpgsqlDataAdapter(command);
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        BookingsDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
                MessageBox.Show("Виведено номер, який бронюють найчастіше.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні інформації: {ex.Message}");
            }
        }

        // 5. Максимальна кількість людей, що зупинялися за добу
        private void MaxGuestsPerDayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT check_in_date, COUNT(DISTINCT guest_id) AS guest_count
                                     FROM Bookings
                                     GROUP BY check_in_date
                                     ORDER BY guest_count DESC
                                     LIMIT 1";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        var adapter = new NpgsqlDataAdapter(command);
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        BookingsDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
                MessageBox.Show("Виведено максимальну кількість людей, що зупинялися за добу.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні інформації: {ex.Message}");
            }
        }

        private string PromptInput(string message, string defaultValue = "")
        {
            return Microsoft.VisualBasic.Interaction.InputBox(message, "Input", defaultValue);
        }
    }
}
