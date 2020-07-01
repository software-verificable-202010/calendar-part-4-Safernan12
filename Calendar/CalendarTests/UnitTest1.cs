using NUnit.Framework;
using System.Runtime.CompilerServices;
using Calendar;
using System.Collections.Generic;
using System;

namespace CalendarTests
{
    public class Tests
    {
        private readonly User testUser = new User("TestName");
        private readonly List<User> userList = new List<User>();
        private readonly Schedule testSchedule = new Schedule("TestTitle", "Test Description", DateTime.Now, DateTime.Now.AddHours(3));
        private readonly List<Schedule> scheduleList = new List<Schedule>();
        private readonly Dictionary<string, string> testGrid = new Dictionary<string, string>();
        [SetUp]
        public void Setup()
        {
            userList.Clear();
            scheduleList.Clear();
            testGrid.Clear();
            userList.Add(testUser);
            scheduleList.Add(testSchedule);
            testGrid.Add("StartingHour",
                         testSchedule.GetStartingDate().Hour.ToString());
            testGrid.Add("EndingHour",
                         testSchedule.GetEndingDate().Hour.ToString());
            testGrid.Add("BlocksPainted", "4");

        }

        [Test]
        public void UserCreationTest()
        {
            string newUsername = "NewName";
            User newUser;
            if (userList.Exists(x => x.GetUsername() == newUsername))
            {
                newUser = userList.Find(x => x.GetUsername() == newUsername);
            }
            else
            {
                User user = new User(newUsername);
                userList.Add(user);
                newUser = user;
            }

            Assert.AreEqual(newUsername, newUser.GetUsername());
        }

        [Test]
        public void UserLoginTest()
        {
            string usedUsername = "TestName";
            User currentUser;
            userList.Add(testUser);
            if (userList.Exists(x => x.GetUsername() == usedUsername))
            {
                currentUser = userList.Find(x => x.GetUsername() == usedUsername);
            }
            else
            {
                User user = new User(usedUsername);
                userList.Add(user);
                currentUser = user;
            }

            Assert.AreEqual(usedUsername, currentUser.GetUsername());
        }

        [Test]
        public void PaintWeekCalendarTest()
        {
            int startingHour = testSchedule.GetStartingDate().Hour;
            int endingHour = testSchedule.GetEndingDate().Hour;
            int blocksPainted = 1;

            Dictionary<string, string> newGrid = new Dictionary<string, string>();

            if (endingHour == 0)
            {
                endingHour = 24;
            }

            newGrid.Add("StartingHour",
                         startingHour.ToString());

            startingHour++;

            for (int row = startingHour; row <= endingHour; row++)
            {
                blocksPainted++;
            }

            newGrid.Add("EndingHour",
                         endingHour.ToString());
            newGrid.Add("BlocksPainted",
                         blocksPainted.ToString());

            Assert.IsTrue(testGrid["BlocksPainted"] == newGrid["BlocksPainted"]);
        }
    }
}