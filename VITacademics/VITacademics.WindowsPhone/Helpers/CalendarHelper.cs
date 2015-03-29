﻿using Academics.ContentService;
using Academics.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VITacademics.Managers;
using Windows.ApplicationModel.Appointments;

namespace VITacademics.Helpers
{
    public static class CalendarHelper
    {
        private static AppointmentStore _store;
        private static AppointmentCalendar _calendar;

        public static async Task LoadCalendar()
        {
            if (UserManager.CurrentUser == null)
                throw new InvalidOperationException("There is no user available.");

            if (_calendar != null)
                return;

            if(_store == null)
            {
                _store = await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AppCalendarsReadWrite);
            }
            var appCalendars = await _store.FindAppointmentCalendarsAsync();
            if (appCalendars.Count == 0)
                _calendar = await _store.CreateAppointmentCalendarAsync("Academics Calendar");
            else
                _calendar = appCalendars[0];

            _calendar.OtherAppWriteAccess = AppointmentCalendarOtherAppWriteAccess.None;
            await _calendar.SaveAsync();
        }

        public static async Task DeleteCalendar()
        {
            if (_calendar != null)
                await _calendar.DeleteAsync();
        }

        public static async Task WriteAppointment(CalenderAwareInfoStub infoStub, string message)
        {
            if (_calendar == null)
                throw new InvalidOperationException();

            Appointment appt;
            if (infoStub.AppointmentInfo == null)
            {
                appt = new Appointment();
                appt.StartTime = infoStub.ContextDate.Date.Add(infoStub.SessionHours.StartHours.TimeOfDay);
                appt.Duration = infoStub.SessionHours.EndHours - infoStub.SessionHours.StartHours;
                appt.Reminder = TimeSpan.FromMinutes(15);
                appt.Subject = string.Format("{0} - {1}", infoStub.SessionHours.Parent.CourseCode, message);
                appt.Location = infoStub.SessionHours.Parent.Venue;
            }
            else
            {
                appt = await _calendar.GetAppointmentAsync(infoStub.AppointmentInfo.Item1);
                appt.Subject = message;
            }

            await _calendar.SaveAppointmentAsync(appt);
            infoStub.AppointmentInfo = new Tuple<string, string>(appt.LocalId, appt.Subject);
        }

        public static async Task RemoveAppointment(CalenderAwareInfoStub infoStub)
        {
            if (_calendar == null)
                throw new InvalidOperationException();

            if(infoStub.AppointmentInfo == null)
                return;

            await _calendar.DeleteAppointmentAsync(infoStub.AppointmentInfo.Item1);
            infoStub.AppointmentInfo = null;
        }

        public static async Task AssignAppointmentIfAvailable(CalenderAwareInfoStub infoStub)
        {
            if (_calendar == null)
                throw new InvalidOperationException();

            DateTimeOffset startDate = infoStub.ContextDate.Date.Add(infoStub.SessionHours.StartHours.TimeOfDay);
            var appts = await _calendar.FindAppointmentsAsync(startDate.AddMinutes(-1), TimeSpan.FromMinutes(2));
            if (appts.Count == 0)
                infoStub.AppointmentInfo = null;
            else
                infoStub.AppointmentInfo = new Tuple<string, string>(appts[0].LocalId, appts[0].Subject);
        }

    }

}
