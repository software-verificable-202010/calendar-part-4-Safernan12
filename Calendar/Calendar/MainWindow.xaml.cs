using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int currentMonth;
        private int currentYear;
        private int focusedDay = 1;
        private readonly string[] monthsNames = { "", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
        private readonly string[] daysNames = { "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado", "Domingo" };
        private readonly string[] hoursNames = { "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00", "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00", "24:00", };
        private string currentMode = "MONTH";
        private readonly string monthMode = "MONTH";
        private readonly string weekMode = "WEEK";
        private User currentUser;
        private Schedule selectedSchedule;
        private readonly List<User> userList = new List<User>();
        private int daysInMonth;
        private readonly string errorText = "Error";
        private readonly string successText = "Success";
        private readonly NumberFormatInfo provider = new NumberFormatInfo();

        public MainWindow()
        {
            InitializeComponent();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";
            currentMonth = DateTime.Now.Month;
            currentYear = DateTime.Now.Year;
            BtnNextValue.Click += BtnNextValue_Click;
            BtnLastValue.Click += BtnLastValue_Click;
        }

        private void BuildRows(string type)
        {
            CalendarGrid.RowDefinitions.Clear();
            int firstRow = 1;
            int totalRows = 0;
            if (type == monthMode)
            {
                totalRows = 7;

            }
            else if (type == weekMode)
            {
                totalRows = 24;
            }

            for (int row = firstRow; row < totalRows; row++)
            {
                RowDefinition gridRow = new RowDefinition();
                CalendarGrid.RowDefinitions.Add(gridRow);
            }
        }

        private void NameDaysLabels()
        {
            DayNamesGrid.Children.Clear();

            int totalColumns = 7;
            int firstColumn = 0;
            int dateNumber = 0;
            int lastDayofLastMonth = 0;
            int substractionValue = -1;
            int december = 12;
            if (currentMode == weekMode)
            {
                dateNumber = GetFirstDayOfWeek(currentYear, currentMonth);
                if (dateNumber > focusedDay)
                {
                    int LastMonth = currentMonth + substractionValue;
                    if (LastMonth == 0)
                    {
                        lastDayofLastMonth = DateTime.DaysInMonth(currentYear + substractionValue, december);
                    }
                    else {
                        lastDayofLastMonth = DateTime.DaysInMonth(currentYear, LastMonth);
                    }
                }
            }
            for (int column = firstColumn; column < totalColumns; column++)
            {
                Label label = new Label();
                if (currentMode == monthMode)
                {
                    label.Content = daysNames[column];
                }
                else if (currentMode == weekMode)
                {
                    label.Content = CreateWeekLabel(dateNumber, daysNames[column]);
                    if (dateNumber == lastDayofLastMonth)
                    {
                        dateNumber = 0;
                    }
                    dateNumber++;
                }
                DayNamesGrid.Children.Add(label);
                Grid.SetRow(label, 0);
                Grid.SetColumn(label, column);
            }
        }

        private string CreateWeekLabel(int dayNumber, string dayName)
        {
            if (dayNumber > daysInMonth)
            {
                return dayName;
            }
            else
            {
                return FormattableString.Invariant($"{dayName} {dayNumber}");
            }

        }

        private void DrawMonthCalendar(int month, int year)
        {
            TimeTableGrid.Visibility = Visibility.Collapsed;
            MonthLabel.Content = monthsNames[month];
            YearLabel.Content = FormattableString.Invariant($"{year}");
            BtnLastValue.Content = "Last Month";
            BtnNextValue.Content = "Next Month";
            NameDaysLabels();
            int firstDay = GetFirstDayValue(year, month);
            daysInMonth = DateTime.DaysInMonth(year, month);
            int totalCollumns = 7;
            int totalRows = 7;
            int dateNumber = 1;
            List<Schedule> ScheduleList = GetAllSchedules();

            CalendarGrid.Children.Clear();

            BuildRows("MONTH");

            AddWeekendBackgrounds();

            for (int FirstRowCollumn = firstDay; FirstRowCollumn < totalCollumns; FirstRowCollumn++)
            {

                Grid daygrid = new Grid();
                daygrid.RowDefinitions.Add(new RowDefinition());
                daygrid.RowDefinitions.Add(new RowDefinition());

                CalendarGrid.Children.Add(daygrid);
                Grid.SetRow(daygrid, 0);
                Grid.SetColumn(daygrid, FirstRowCollumn);

                Label label = new Label
                {
                    Content = dateNumber
                };
                label.MouseDoubleClick += (s, e) => BtnFocusWeek(s, e);

                daygrid.Children.Add(label);

                if (ScheduleList.Exists(x => x.CheckStartingDate(currentYear, currentMonth, dateNumber)))
                {
                    Schedule schedule = ScheduleList.Find(x => x.CheckStartingDate(currentYear, currentMonth, dateNumber));
                    Border ScheduleBorder = new Border()
                    {
                        Background = Brushes.Blue,
                        Child = new TextBlock()
                        {
                            Text = schedule.GetTitle(),
                            Foreground = Brushes.White
                        }
                    };
                    daygrid.Children.Add(ScheduleBorder);
                    Grid.SetRow(ScheduleBorder, 1);
                }

                dateNumber++;
            }

            for (int row = 1; row < totalRows; row++)
            {
                for (int collumn = 0; collumn < totalCollumns; collumn++)
                {
                    if (dateNumber <= daysInMonth)
                    {

                        Grid daygrid = new Grid();
                        daygrid.RowDefinitions.Add(new RowDefinition());
                        daygrid.RowDefinitions.Add(new RowDefinition());

                        CalendarGrid.Children.Add(daygrid);
                        Grid.SetRow(daygrid, row);
                        Grid.SetColumn(daygrid, collumn);

                        Label label = new Label
                        {
                            Content = dateNumber
                        };
                        label.MouseDoubleClick += (s, e) => BtnFocusWeek(s, e);
                        daygrid.Children.Add(label);

                        if (ScheduleList.Exists(x => x.CheckStartingDate(currentYear, currentMonth, dateNumber)))
                        {
                            Schedule schedule = ScheduleList.Find(x => x.CheckStartingDate(currentYear, currentMonth, dateNumber));
                            Border ScheduleBorder = new Border()
                            {
                                Background = Brushes.Blue,
                                Child = new TextBlock()
                                {
                                    Text = schedule.GetTitle(),
                                    Foreground = Brushes.White
                                }
                            };
                            daygrid.Children.Add(ScheduleBorder);
                            Grid.SetRow(ScheduleBorder, 1);
                        }

                        dateNumber++;
                    }
                }
            }

        }

        private void DrawWeekCalendar(int month, int year)
        {
            TimeTableGrid.Visibility = Visibility.Visible;
            CalendarGrid.Children.Clear();
            BtnLastValue.Content = "Last Week";
            BtnNextValue.Content = "Next Week";
            BuildRows("WEEK");
            MonthLabel.Content = monthsNames[month];
            YearLabel.Content = FormattableString.Invariant($"{year}");
            daysInMonth = DateTime.DaysInMonth(year, month);
            NameDaysLabels();
            List<Schedule> ScheduleList = GetAllSchedules();
            int totalCollumns = 7;
            int firstColumn = 0;
            int lastMonth = 12;

            int substractionValue = 1;
            int dateNumber = GetFirstDayOfWeek(year, month);
            int lastDayofLastMonth = 0;
            int december = 12;
            if (dateNumber > focusedDay)
            {
                lastMonth = currentMonth - substractionValue;
                if (lastMonth == 0)
                {
                    lastMonth = december;
                    lastDayofLastMonth = DateTime.DaysInMonth(currentYear - substractionValue, december);
                }
                lastDayofLastMonth = DateTime.DaysInMonth(currentYear, lastMonth);
            }

            for (int column = firstColumn; column < totalCollumns; column++)
            {
                if (lastDayofLastMonth == 0)
                {
                    if (ScheduleList.Exists(x => x.CheckStartingDate(currentYear, currentMonth, dateNumber)))
                    {
                        PaintWeekSchedule(dateNumber, column, ScheduleList);
                    }
                }
                else
                {
                    if (ScheduleList.Exists(x => x.CheckStartingDate(currentYear, currentMonth, dateNumber)))
                    {
                        PaintWeekSchedule(dateNumber, column, ScheduleList);
                    }
                    if (lastDayofLastMonth == dateNumber)
                    {
                        dateNumber = 0;
                    }
                }
                dateNumber++;
            }


        }

        private void PaintWeekSchedule(int day, int column, List<Schedule> scheduleList)
        {
            Schedule schedule = scheduleList.Find(x => x.CheckStartingDate(currentYear, currentMonth, day));
            int startingHour = schedule.GetStartingDate().Hour;
            int endingHour = schedule.GetEndingDate().Hour;
            if (endingHour == 0)
            {
                endingHour = 24;
            }

            Border scheduleBorder = new Border()
            {
                Background = Brushes.Blue,
                Child = new TextBlock()
                {
                    Text = schedule.GetTitle(),
                    Foreground = Brushes.White
                }
            };

            CalendarGrid.Children.Add(scheduleBorder);
            Grid.SetRow(scheduleBorder, startingHour);
            Grid.SetColumn(scheduleBorder, column);

            startingHour++;

            for (int row = startingHour; row <= endingHour; row++)
            {
                Rectangle scheduleRectangle = new Rectangle()
                {
                    Fill = Brushes.Blue
                };

                CalendarGrid.Children.Add(scheduleRectangle);
                Grid.SetRow(scheduleRectangle, row);
                Grid.SetColumn(scheduleRectangle, column);
            }
        }

        private static int GetFirstDayValue(int year, int month)
        {
            int firstDayValue = 1;
            int dateValueCorrector = -1;
            int sundayValue = -1;
            DateTime firstDayOfMonth = new DateTime(year, month, firstDayValue);
            int firstDay = (int)firstDayOfMonth.DayOfWeek;
            firstDay += dateValueCorrector;
            if (firstDay == sundayValue)
            {
                firstDay = 6;
            }


            return firstDay;
        }

        private int GetFirstDayOfWeek(int year, int month)
        {
            DateTime focusedDatetime = new DateTime(year, month, focusedDay);
            DayOfWeek firstDay = DayOfWeek.Monday;
            DateTime firstDayInWeek = focusedDatetime.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
                firstDayInWeek = firstDayInWeek.AddDays(-1);

            return firstDayInWeek.Day;
        }

        private void AddWeekendBackgrounds()
        {
            int totalRows = 6;
            int firstRow = 0;
            int saturdayColumn = 5;
            int sundayColumn = 6;

            for (int row = firstRow; row < totalRows; row++)
            {
                Rectangle saturdayRectangle = new Rectangle()
                {
                    Fill = Brushes.Azure
                };

                CalendarGrid.Children.Add(saturdayRectangle);
                Grid.SetRow(saturdayRectangle, row);
                Grid.SetColumn(saturdayRectangle, saturdayColumn);

                Rectangle sundayRectangle = new Rectangle()
                {
                    Fill = Brushes.Azure
                };

                CalendarGrid.Children.Add(sundayRectangle);
                Grid.SetRow(sundayRectangle, row);
                Grid.SetColumn(sundayRectangle, sundayColumn);
            }

        }

        private int CalculateFocusedDay(int focusedDay, int month)
        {
            int daysInNewMonth = DateTime.DaysInMonth(currentYear, month);
            int newFocusedDay;
            if (focusedDay <= 0)
            {
                newFocusedDay = daysInNewMonth + focusedDay;
            }
            else
            {
                newFocusedDay = focusedDay - daysInMonth;
            }

            return newFocusedDay;
        }

        private List<Schedule> GetAllSchedules()
        {
            List<Schedule> scheduleList = currentUser.GetSchedule();

            foreach (User user in userList)
            {
                if (user != currentUser)
                {
                    List<Schedule> selectedUserSchedule = user.GetSchedule();
                    foreach (Schedule selectedSchedule in selectedUserSchedule)
                    {
                        if (selectedSchedule.GetInviteeList().Exists(x => x == currentUser))
                        {
                            scheduleList.Add(selectedSchedule);
                        }
                    }
                }
            }


            return scheduleList;
        }

        private void ClearEditScheduleForm()
        {
            EditScheduleTitleTextBox.Text = "";
            EditScheduleDescriptionTextBox.Text = "";

        }

        private void FillEditScheduleForm()
        {
            EditScheduleTitleTextBox.Text = selectedSchedule.GetTitle();
            EditScheduleDescriptionTextBox.Text = selectedSchedule.GetDescription();
        }

        private void FillScheduleComboBox()
        {
            SelectScheduleComboBox.Items.Clear();
            foreach (Schedule schedule in currentUser.GetSchedule())
            {
                SelectScheduleComboBox.Items.Add(schedule.GetTitle());
            }
        }

        private void FillUserComboBox()
        {
            AddInviteeComboBox.Items.Clear();
            foreach (User user in userList)
            {
                if (user != currentUser)
                {
                    AddInviteeComboBox.Items.Add(user.GetUsername());
                }
            }
        }


        private void BtnNextValue_Click(object sender, RoutedEventArgs e)
        {
            int dateAdditionValue = 1;
            int weekValue = 7;
            if (currentMode == monthMode)
            {
                currentMonth += dateAdditionValue;
                if (currentMonth == 13)
                {
                    currentMonth = 1;
                    currentYear += dateAdditionValue;
                }
                DrawMonthCalendar(currentMonth, currentYear);
            }
            else if (currentMode == weekMode)
            {
                focusedDay += weekValue;
                if (focusedDay >= daysInMonth)
                {
                    currentMonth += dateAdditionValue;
                    if (currentMonth == 13)
                    {
                        currentMonth = 1;
                        currentYear += dateAdditionValue;
                    }
                    focusedDay = CalculateFocusedDay(focusedDay, currentMonth);
                }
                DrawWeekCalendar(currentMonth, currentYear);
            }
        }

        private void BtnLastValue_Click(object sender, RoutedEventArgs e)
        {
            int dateSubstractionValue = -1;
            int weekValue = -7;
            if (currentMode == monthMode)
            {
                currentMonth += dateSubstractionValue;
                if (currentMonth == 0)
                {
                    currentMonth = 12;
                    currentYear += dateSubstractionValue;
                }
                DrawMonthCalendar(currentMonth, currentYear);
            }
            else if (currentMode == weekMode)
            {
                focusedDay += weekValue;
                if (focusedDay <= 0)
                {
                    currentMonth += dateSubstractionValue;
                    if (currentMonth == 0)
                    {
                        currentMonth = 12;
                        currentYear += dateSubstractionValue;
                    }
                    focusedDay = CalculateFocusedDay(focusedDay, currentMonth);
                }
                DrawWeekCalendar(currentMonth, currentYear);
            }
        }

        private void BtnFocusWeek(object sender, MouseButtonEventArgs e)
        {
            focusedDay = (int)(sender as Label).Content;
            currentMode = weekMode;
            DrawWeekCalendar(currentMonth, currentYear);
        }

        private void BtnFocusMonth(object sender, MouseButtonEventArgs e)
        {
            currentMode = monthMode;
            DrawMonthCalendar(currentMonth, currentYear);
        }

        private void BtnNewSchedule_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Visibility = Visibility.Collapsed;
            ScheduleFormGrid.Visibility = Visibility.Visible;
        }

        private void BtnUserConfirm_Click(object sender, RoutedEventArgs e)
        {
            String username = UsernameTextBox.Text;
            if (String.IsNullOrEmpty(username))
            {
                MessageBox.Show("You have to choose a Username", errorText);
            }
            else
            {
                if (userList.Exists(x => x.GetUsername() == username))
                {
                    currentUser = userList.Find(x => x.GetUsername() == username);
                }
                else
                {
                    User user = new User(username);
                    userList.Add(user);
                    currentUser = user;
                }
                MainGrid.Visibility = Visibility.Visible;
                MenuGrid.Visibility = Visibility.Collapsed;
                DrawMonthCalendar(currentMonth, currentYear);
            }
        }

        private void BtnCreateNewSchedule_Click(object sender, RoutedEventArgs e)
        {
            String title = ScheduleTitleTextBox.Text;
            String description = ScheduleDescriptionTextBox.Text;
            String startingTime = StartingHourComboBox.Text;
            String endingTime = EndingHourComboBox.Text;
            if ((String.IsNullOrEmpty(title)) || (String.IsNullOrEmpty(startingTime)) || (String.IsNullOrEmpty(endingTime)) || !(DateStartInput.SelectedDate.HasValue))
            {
                MessageBox.Show("You have to fill all inputs", errorText);
            }
            else
            {
                Double startingHour = Convert.ToDouble(startingTime.Substring(0, 2), provider);
                Double endingHour = Convert.ToDouble(endingTime.Substring(0, 2), provider);
                DateTime dateStart = (DateTime)DateStartInput.SelectedDate;
                DateTime dateEnd = (DateTime)DateStartInput.SelectedDate;
                dateStart = dateStart.AddHours(startingHour);
                dateEnd = dateEnd.AddHours(endingHour);
                currentUser.AddSchedule(new Schedule(title, description, dateStart, dateEnd));
                MainGrid.Visibility = Visibility.Visible;
                ScheduleFormGrid.Visibility = Visibility.Collapsed;
                if (currentMode == monthMode)
                {
                    DrawMonthCalendar(currentMonth, currentYear);
                }
                else if (currentMode == weekMode)
                {
                    DrawWeekCalendar(currentMonth, currentYear);
                }

            }
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Visibility = Visibility.Collapsed;
            MenuGrid.Visibility = Visibility.Visible;
            BtnUserCancel.Visibility = Visibility.Visible;
        }

        private void BtnUserCancel_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Visibility = Visibility.Visible;
            MenuGrid.Visibility = Visibility.Collapsed;
        }

        private void CancelSchedule_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Visibility = Visibility.Visible;
            ScheduleFormGrid.Visibility = Visibility.Collapsed;
            EditScheduleFormGrid.Visibility = Visibility.Collapsed;
        }

        private void StartingHourComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EndingHourComboBox.Items.Clear();
            int StartingComboBoxIndex = StartingHourComboBox.SelectedIndex;

            for (int ComboBoxIndex = StartingComboBoxIndex; ComboBoxIndex < hoursNames.Length; ComboBoxIndex++)
            {
                EndingHourComboBox.Items.Add(hoursNames[ComboBoxIndex]);
            }
        }

        private void BtnAddInvitee_Click(object sender, RoutedEventArgs e)
        {
            if (AddInviteeComboBox.SelectedItem == null || AddInviteeComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("You have to choose a User", errorText);
            }
            else
            {
                String name = AddInviteeComboBox.SelectedValue.ToString();
                selectedSchedule.AddInvitee(userList.Find(x => x.GetUsername() == name));
                MessageBox.Show("You have succesfully invited the user", successText);
            }
        }

        private void BtnRemoveSchedule_Click(object sender, RoutedEventArgs e)
        {
            ClearEditScheduleForm();
            MainGrid.Visibility = Visibility.Visible;
            EditScheduleFormGrid.Visibility = Visibility.Collapsed;
            currentUser.RemoveSchedule(selectedSchedule);
            if (currentMode == monthMode)
            {
                DrawMonthCalendar(currentMonth, currentYear);
            }
            else if (currentMode == weekMode)
            {
                DrawWeekCalendar(currentMonth, currentYear);
            }
        }

        private void BtnEditSchedule_Click(object sender, RoutedEventArgs e)
        {
            ClearEditScheduleForm();
            MainGrid.Visibility = Visibility.Collapsed;
            EditScheduleFormGrid.Visibility = Visibility.Visible;
            FillScheduleComboBox();
        }

        private void BtnEditCurrentSchedule_Click(object sender, RoutedEventArgs e)
        {
            String title = EditScheduleTitleTextBox.Text;
            String description = EditScheduleDescriptionTextBox.Text;
            String startingTime = EditStartingHourComboBox.Text;
            String endingTime = EditEndingHourComboBox.Text;
            if ((String.IsNullOrEmpty(title)) || (String.IsNullOrEmpty(startingTime)) || (String.IsNullOrEmpty(endingTime)) || !(EditDateStartInput.SelectedDate.HasValue))
            {
                MessageBox.Show("You have to fill all inputs", errorText);
            }
            else
            {
                Double starting_hour = Convert.ToDouble(startingTime.Substring(0, 2), provider);
                Double ending_hour = Convert.ToDouble(endingTime.Substring(0, 2), provider);
                DateTime date_start = (DateTime)EditDateStartInput.SelectedDate;
                DateTime date_end = (DateTime)EditDateStartInput.SelectedDate;
                date_start = date_start.AddHours(starting_hour);
                date_end = date_end.AddHours(ending_hour);
                selectedSchedule.EditSchedule(title, description, date_start, date_end);
                MainGrid.Visibility = Visibility.Visible;
                EditScheduleFormGrid.Visibility = Visibility.Collapsed;
                if (currentMode == monthMode)
                {
                    DrawMonthCalendar(currentMonth, currentYear);
                }
                else if (currentMode == weekMode)
                {
                    DrawWeekCalendar(currentMonth, currentYear);
                }

            }
        }

        private void SelectScheduleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectScheduleComboBox.SelectedItem == null || SelectScheduleComboBox.SelectedIndex == -1)
            {
                ClearEditScheduleForm();
            }
            else
            {
                String title = SelectScheduleComboBox.SelectedValue.ToString();
                selectedSchedule = currentUser.GetSchedule().Find(x => x.GetTitle() == title);
                FillEditScheduleForm();
                FillUserComboBox();
            }

        }
    }
}
