using AppicationBot.Ver._2.Models;
using AppicationBot.Ver._2.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using OTempus.Library.Class;
using OTempus.Library.Result;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AppicationBot.Ver._2.Models
{
    [Serializable]
    public class ReservationReschedule
    {
        #region Properties
        [Prompt("Please select an appointment: {||}")]
        public string processIdServiceId;
        //I commentes this two lines beacuse I will implement a new source of book form
        //[Prompt("Can you enter the new date and time for reschedule your appointment please,  MM/dd/yyyy hh:mm:ss? {||}")]
        // public string newDate;
        [Prompt("Please select a date: {||} ")]
        public string Date;
        //public string Days;
        //This is the hour of the selected slot
        [Prompt("Please select an option: {||} ")]
        public DayPeriod2? dayPeriod;
        [Prompt("Please select your preferred time: {||} ")]
        public string Hour;
        public static IDialogContext context { get; set; }
        public int _customerId { get; private set; }
        public ReservationReschedule(int customerId)
        {
            this._customerId = customerId;
        }
        public ReservationReschedule() { }
        #endregion
        public enum DayPeriod2
        {
            //All,
            Morning,
            Noon,
            Afternoon

        }
    }
}