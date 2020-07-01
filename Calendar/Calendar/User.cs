using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    public class User
    {
        private readonly string username;
        private readonly List<Schedule> scheduleList = new List<Schedule>();

        public User(string name)
        {
            username = name;
        }

        public void AddSchedule(Schedule schedule)
        {
            scheduleList.Add(schedule);
        }

        public void RemoveSchedule(Schedule schedule)
        {
            scheduleList.Remove(schedule);
        }

        public List<Schedule> GetSchedule()
        {
            return scheduleList;
        }

        public String GetUsername()
        {
            return username;
        }
    }
}
