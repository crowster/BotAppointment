using AppicationBot.Ver._2.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using OTempus.Library.Class;
using OTempus.Library.Result;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;


namespace AppicationBot.Ver._2.Models
{
    [Serializable]

    public class ReservationCancel
    {
        #region Properties
        [Prompt("Select the appointment you want to cancel: {||}")]
        public string processId { get; set; }
        public int _customerId { get; private set; }
        public ReservationCancel(int customerId)
        {
            this._customerId = customerId;
        }
        public ReservationCancel() { }
        public static IDialogContext context { get; set; }
        #endregion

    }
}