using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using OTempus.Library.Class;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace AppicationBot.Ver._2.Forms
{
    [Serializable]
    public class StatusForm
    {
        [Prompt("Can you enter the {&}, please? {||} ")]

        public int calendarId;

        [Prompt("Can you enter the {&}, please? {||} ")]

        public int appoinmentId;
        public static IDialogContext context { get; set; }

        public static IForm<StatusForm> BuildForm()
        {
            OnCompletionAsyncDelegate<StatusForm> processOrder = async (context, state) =>
            {
                //DEclarationof variables to consume the method
                bool appoinmentsOnly=false;
                string typeSeriaizer="XML";
                string displayEndTime=String.Empty;
                //Declaration of Calendar Get Slots Results oobject
                CalendarGetSlotsResults slotToShowInformation = new CalendarGetSlotsResults();
                //Declaration of an instance of the library WebAppoinmentsClientLibrary
                WebAppoinmentsClientLibrary.Calendars calendar = new WebAppoinmentsClientLibrary.Calendars();
                //Declaration od List for keep the CalendarGetSlotsResults, from the library
                List<CalendarGetSlotsResults> listCalendarSlots=calendar.GetSlotsByCalendarId(state.calendarId,appoinmentsOnly,typeSeriaizer);
                //Read the list for search the cappoinment id, in this case the ordinal number
                foreach (CalendarGetSlotsResults calendarGetSlotsResults in listCalendarSlots) {
                    if (calendarGetSlotsResults.OrdinalNumber == state.appoinmentId) {
                        slotToShowInformation.OrdinalNumber = calendarGetSlotsResults.OrdinalNumber;
                        slotToShowInformation.QNumber = calendarGetSlotsResults.QNumber;
                        slotToShowInformation.Duration = calendarGetSlotsResults.Duration;
                        slotToShowInformation.StartTime = calendarGetSlotsResults.StartTime;
                        slotToShowInformation.CustomerFirstName = calendarGetSlotsResults.CustomerFirstName;
                        displayEndTime = calendarGetSlotsResults.DisplayEndTime.ToString();
                        slotToShowInformation.Status = calendarGetSlotsResults.Status;
                    }
                }
                //Get the customer id saved for show it
                int mycustomerid;
                if (!context.PrivateConversationData.TryGetValue<int>("customerId", out mycustomerid)) { mycustomerid = 0; }



                //If the ordinal number is different of 0, means that we find it aone register
                if (slotToShowInformation.OrdinalNumber != 0)
                await context.PostAsync($" The customer Id is: " + mycustomerid + "The actual information is \n* Status:  " + slotToShowInformation.Status.ToString()
                    + " \n* Customer name: "+ slotToShowInformation.CustomerFirstName + "\n* start time: " 
                    + slotToShowInformation.DisplayStartTime.ToString() +"------ Or= "+
                    slotToShowInformation.StartTime.ToString() +" Past midnight"+ "\n* end time:  "
                    + displayEndTime + " \n* duration: " + slotToShowInformation.Duration.ToString()
                    );
                // in other hand we dont'find a register, so ,we will send the appropiate mmesage
                else {
                    await context.PostAsync($"The customer Id is: "+ mycustomerid +  "I don't have a register to match with calendarid: \n* "+ state.calendarId+ "appoinment Id: \n*" + state.appoinmentId);
                }
            };
            CultureInfo ci = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            var culture = Thread.CurrentThread.CurrentUICulture;
            var form = new FormBuilder<StatusForm>();
            var yesTerms = form.Configuration.Yes.ToList();
            var noTerms = form.Configuration.No.ToList();
            yesTerms.Add("Yes");
            noTerms.Add("No");
            form.Configuration.Yes = yesTerms.ToArray();

            return form.Message("Fill the information for search the appoinment, please")
                       .Field(nameof(calendarId))
                       .Field(nameof(appoinmentId))
                      .Confirm("Are you selected the calendar id {calendarId}, and slot {appoinmentId}: ? (yes/no)")
                      .AddRemainingFields()
                      //.Message("The process for create the appoinment has been started!")
                      .OnCompletion(processOrder)
                      .Build();
            
        }
    };

}

