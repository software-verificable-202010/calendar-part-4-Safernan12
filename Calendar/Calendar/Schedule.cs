using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    class Schedule
    {
        private String title;
        private String description;
        private DateTime dateStart;
        private DateTime dateEnd;
        private readonly List<User> InviteeList = new List<User>();

        public Schedule(String title, String description, DateTime dateStart, DateTime dateEnd)
        {
            this.title = title;
            this.description = description;
            this.dateStart = dateStart;
            this.dateEnd = dateEnd;
        }

        public void EditSchedule(String title, String description, DateTime dateStart, DateTime dateEnd)
        {
            this.title = title;
            this.description = description;
            this.dateStart = dateStart;
            this.dateEnd = dateEnd;
        }

        public Boolean CheckStartingDate(int year, int month, int day)
        {
            return (dateStart.Year == year) && (dateStart.Month == month) && (dateStart.Day == day);
        }

        public void AddInvitee(User invitee)
        {
            InviteeList.Add(invitee);
        }

        public List<User> GetInviteeList()
        {
            return InviteeList;
        }

        public String GetTitle()
        {
            return title;
        }

        public String GetDescription()
        {
            return description;
        }

        public DateTime GetStartingDate()
        {
            return dateStart;
        }

        public DateTime GetEndingDate()
        {
            return dateEnd;
        }

    }
}
