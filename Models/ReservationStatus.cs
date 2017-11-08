using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppicationBot.Ver._2.Models
{
    [Serializable]

    public class ReservationStatus
    {

        #region Properties
        [Prompt("Please select an appointment: {||}")]
        public string appointmentId;
        public static IDialogContext context { get; set; }
        public int _customerId { get; private set; }
        public ReservationStatus(int customerId)
        {
            this._customerId = customerId;
        }
        public ReservationStatus(){}
        #endregion
    }
}