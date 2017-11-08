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

namespace AppicationBot.Ver._2.Forms
{
    public enum DayPeriod2
    {
        //All,
        Morning,
        Noon,
        Afternoon

    }
    // Evening,
    // Night
    [Serializable]
    public class RescheduleFormAppThree
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
        public DayPeriod? dayPeriod;
        [Prompt("Please select your preferred time: {||} ")]
        public string Hour;
        public static IDialogContext context { get; set; }
        #endregion
        #region Methods
        /// <summary>
        /// Get calendars by service id and an specific date
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        static List<OTempus.Library.Class.Calendar> GetCalendar(string serviceId, DateTime date)
        {
            try
            {
                return AppoinmentService.GetCalendars(serviceId, date);
            }
            catch (Exception ex)
            {
                throw new Exception("error in GetCalendar: " + ex.Message);
            }
        }

        #endregion
        #region Creation of IForm
        /// <summary>
        /// This method create an Iform (form flow) for reschedule an appointement
        /// </summary>
        /// <returns></returns>
        public static IForm<RescheduleFormAppThree> BuildForm()
        {
            //Instance of library for manage appoinments
            WebAppoinmentsClientLibrary.Appoinments appointmentLibrary = new WebAppoinmentsClientLibrary.Appoinments();
            #region On complete, process Order
            OnCompletionAsyncDelegate<RescheduleFormAppThree> processOrder = async (context, state) =>
            {
                try
                {
                    Char delimiter = '.';
                    string[] arrayInformation = state.processIdServiceId.Split(delimiter);
                    int processId = Convert.ToInt32(arrayInformation[0]);
                    int serviceId = Convert.ToInt32(arrayInformation[1]);

                    string[] dateInformation = state.Date.Split(delimiter);
                    string stringDate = dateInformation[1];
                    stringDate = Utilities.Util.GetDateWithOutTime(stringDate);
                    //Here I create the new date
                    string newDate = stringDate + " " + state.Hour;
                    string newDat2 = Utilities.Util.GetDateWithCorrectPositionOfTheMonth(newDate);
                    int result = 0;
                    try
                    {
                        result = AppoinmentService.RescheduleAppoinment(processId, newDat2, serviceId);
                    }
                    catch (Exception)
                    {
                        result = AppoinmentService.RescheduleAppoinment(processId, newDate, serviceId);
                    }
                    AppointmentGetResults _appointment = AppoinmentService.GetAppointmentById(result);
                    await context.PostAsync($"The appointment was rescheduled, Ticket: " + _appointment.QCode + _appointment.QNumber);
                }
                catch (Exception ex)
                {
                    await context.PostAsync(ex.Message.ToString());
                }
            };
            #endregion
            #region set language and create a container for form builder
            CultureInfo ci = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            var culture = Thread.CurrentThread.CurrentUICulture;
            var form = new FormBuilder<RescheduleFormAppThree>();
            var yesTerms = form.Configuration.Yes.ToList();
            var noTerms = form.Configuration.No.ToList();
            yesTerms.Add("Yes");
            noTerms.Add("No");
            form.Configuration.Yes = yesTerms.ToArray();
            return form
            #endregion
            #region process and service ids
                           .Field(new FieldReflector<RescheduleFormAppThree>(nameof(processIdServiceId))
                           .SetType(null)
                           .SetDefine(async (state, field) =>
                           {
                               //Get the actual user state of the customer
                               ACFCustomer customerState = new ACFCustomer();
                               try
                               {
                                   if (!context.UserData.TryGetValue("customerState", out customerState)) { customerState = new ACFCustomer(); }
                               }
                               catch (Exception ex)
                               {
                                   throw new Exception("Not exists a user session");
                               }
                               int customerIdState = 0;
                               customerIdState = customerState.CustomerId;
                               string personalIdState = string.Empty;
                               personalIdState = customerState.PersonaId;
                               //Instance of library for manage customers
                               WebAppoinmentsClientLibrary.Customers customerLibrary = new WebAppoinmentsClientLibrary.Customers();
                               //Instance of library for manage cases
                               WebAppoinmentsClientLibrary.Cases caseLibrary = new WebAppoinmentsClientLibrary.Cases();
                               //Here we will to find the customer by customer id or personal id
                               Customer customer = null;
                               if (!string.IsNullOrEmpty(customerIdState.ToString()))
                               {
                                   //Get the object ObjectCustomer and inside of this the object Customer
                                   try
                                   {
                                       customer = customerLibrary.GetCustomer(customerIdState).Customer;
                                   }
                                   catch (Exception)
                                   {
                                       // throw; here we not send the exception beacuse we need to do the next method below 
                                   }
                               }
                               //If not found by customer id , we will try to find by personal id
                               else
                               {
                                   int idType = 0;
                                   //Get the object ObjectCustomer and inside of this the object Customer
                                   try
                                   {
                                       customer = customerLibrary.GetCustomerByPersonalId(personalIdState, idType).Customer;
                                   }
                                   catch (Exception)
                                   {
                                       //throw;
                                   }
                               }
                               if (customer == null)
                               {
                                   await context.PostAsync($"The user is not valid");
                                   return await Task.FromResult(false);
                               }
                               else
                               {
                                   //Declaration of Calendar Get Slots Results object
                                   CalendarGetSlotsResults slotToShowInformation = new CalendarGetSlotsResults();
                                   //Set the parameters for get the expected appoinments
                                   int customerTypeId = 0;
                                   string customerTypeName = "";
                                   int customerId = customer.Id;
                                   DateTime startDate = DateTime.Today;
                                   //here we add ten days to the startdate
                                   DateTime endDate = startDate.AddDays(10);
                                   string fromDate = startDate.ToString();
                                   string toDate = endDate.ToString();
                                   // string fromDate = "09/21/2017 ";
                                   //string toDate = "10/21/2018 ";
                                   string typeSeriaizer = "XML";
                                   //Declaration of the ocject to save the result of the GetExpectedAppoinment
                                   ObjectCustomerGetExpectedAppointmentsResults objectCustomerGetExpectedAppointmentsResults = new ObjectCustomerGetExpectedAppointmentsResults();
                                   objectCustomerGetExpectedAppointmentsResults = customerLibrary.GetExpectedAppoinment(customerTypeId, customerTypeName, customerId, startDate, endDate, typeSeriaizer);
                                   if (objectCustomerGetExpectedAppointmentsResults.ListCustomerGetExpectedAppointmentsResults.Count > 0)
                                   {
                                       foreach (CustomerGetExpectedAppointmentsResults listCustomer in objectCustomerGetExpectedAppointmentsResults.ListCustomerGetExpectedAppointmentsResults)
                                       {
                                           //At first I need to find the appoinment by appoiment id, for saw the actual status
                                           Appointment appointment = appointmentLibrary.GetAppoinment(listCustomer.AppointmentId).AppointmentInformation;
                                           string data = appointment.AppointmentDate.ToString();
                                           string processIdAndServiceId = appointment.ProcessId + "." + appointment.ServiceId + "." + Utilities.Util.GetDateWithOutTime(data);
                                           field
                                          .AddDescription(processIdAndServiceId, data + " | " + listCustomer.ServiceName)//here we put the process id and the date of the appointment of this process
                                          .AddTerms(processIdAndServiceId, data + " | " + listCustomer.ServiceName);
                                       }
                                       return await Task.FromResult(true);
                                   }
                                   else
                                   {
                                       await context.PostAsync($"No records found");
                                       throw new Exception("No records found");
                                   }
                               }
                           }))
            #endregion
            #region Date
            .Field(new FieldReflector<RescheduleFormAppThree>(nameof(Date))
            .SetType(null)
            .SetDefine(async (state, field) =>
            {
                string date;
                string service;
                List<Service> listService;
                int unitId = 0;
                List<OTempus.Library.Class.Calendar> listCalendars;
                StringBuilder response = new StringBuilder();
                List<CalendarGetSlotsResults> listGetAvailablesSlots = new List<CalendarGetSlotsResults>();
                if (!String.IsNullOrEmpty(state.processIdServiceId))
                {
                    Char delimiter = '.';
                    string[] arrayInformation = state.processIdServiceId.Split(delimiter);
                    int processId = Convert.ToInt32(arrayInformation[0]);
                    int serviceId = Convert.ToInt32(arrayInformation[1]);
                    string currentAppoinmentDate = arrayInformation[2];
                    DateTime dateFromString = DateTime.Parse(currentAppoinmentDate, System.Globalization.CultureInfo.CurrentCulture);
                    try
                    {
                        listCalendars = new List<OTempus.Library.Class.Calendar>();

                        //listCalendars = GetCalendar(serviceID.ToString(), dateAndTime);
                        //I commented this line beacuse I will take today and Three days more, for get the calendars, and then get the dates of this calendars
                        listCalendars = GetCalendar(serviceId.ToString(), dateFromString);

                        if (listCalendars.Count == 0)
                        {
                            await context.PostAsync($"No records found");
                            throw new Exception("No records found");

                        }
                        else
                        {
                            foreach (var calendar in listCalendars)
                            {
                                string data = calendar.Id + "." + calendar.CalendarDate.ToString();
                                string data1 = Utilities.Util.GetDateWithOutTime(calendar.CalendarDate.ToString());//we see this in form flow
                                field
                                  .AddDescription(data, data1)
                                  .AddTerms(data, data1);
                            }
                            return await Task.FromResult(true);
                        }//End else 

                    }//End try
                    catch (Exception e) { }
                }
                return await Task.FromResult(true);
            }))
            #endregion
            #region Period day
           .Field(nameof(dayPeriod))
            #endregion
            #region hour
           .Field(new FieldReflector<RescheduleFormAppThree>(nameof(Hour))
           .SetType(null)
           .SetDefine(async (state, value) =>
           {
               string date;
               string service;
               List<Service> listService;
               List<OTempus.Library.Class.Calendar> listCalendars;
               List<CalendarSlot> listGetAvailablesSlots;
               if (!String.IsNullOrEmpty(state.Date) && !String.IsNullOrEmpty(state.dayPeriod.ToString()) && !String.IsNullOrEmpty(state.processIdServiceId))
               {
                   Char delimiter = '.';
                   string[] arrayInformation = state.processIdServiceId.Split(delimiter);
                   int processId = Convert.ToInt32(arrayInformation[0]);
                   int serviceId = Convert.ToInt32(arrayInformation[1]);
                   string currentAppoinmentDate = arrayInformation[2];
                   DateTime dateFromString = DateTime.Parse(currentAppoinmentDate, System.Globalization.CultureInfo.CurrentCulture);


                   int periodDay = Convert.ToInt32(Utilities.Util.GetIntPeriodDay(state.dayPeriod.ToString()));
                   string calendarId = Utilities.Util.GetCalendarIdFromBotOption(state.Date);
                   //asign the calendar id
                   //state.CalendarId = calendarId;
                   try
                   {
                       listCalendars = new List<OTempus.Library.Class.Calendar>();
                       date = state.Date;
                       listCalendars = GetCalendar(serviceId.ToString(), dateFromString);
                       //List<Appoinment> listAppoinments = await Services.AppoinmentService.GetAppoinments();
                       listGetAvailablesSlots = new List<CalendarSlot>();

                       StringBuilder response = new StringBuilder();
                       response.Append("Not exists slots").ToString();
                   }
                   catch (Exception ex)
                   {
                       throw new Exception("Here are the error: " + ex.Message);
                   }
                   date = state.Date.ToString();
                   if (listCalendars.Count > 0)
                   {
                       listGetAvailablesSlots = AppoinmentService.GetSlotsByPeriod(Convert.ToInt32(calendarId), periodDay.ToString(), 0.ToString());
                       if (listGetAvailablesSlots.Count > 0)
                       {

                           int cont = 0;
                           foreach (OTempus.Library.Class.CalendarSlot calendarSlots in listGetAvailablesSlots)
                           {
                               //string hour = Utilities.Util.GetHourFromStartDate(calendarSlots.DisplayStartTime.ToString()); ;
                               if (calendarSlots.Status.ToString() == "Vacant")
                               {
                                   //I commented this line because I need to cut the message
                                   // string data =calendarSlots.OrdinalNumber+".-"+ calendarSlots.DisplayStartTime.ToString() +"-"+calendarSlots.DisplayEndTime+"=>"+calendarSlots.Status;
                                   string data = calendarSlots.StartTime.ToString();
                                   DateTime date1 = DateTime.Today;
                                   date1 = date1.AddMinutes(calendarSlots.StartTime);
                                   string data1 = date1.ToShortTimeString();
                                   //assign the calendar id
                                   value
                                  .AddDescription(data1, data1)
                                  .AddTerms(data1, data1);
                                   cont++;
                               }
                           }
                           return await Task.FromResult(true);
                       }
                       else
                       {
                           await context.PostAsync($"No records found");

                           throw new Exception("No records found");
                       }
                   }
                   else
                   {
                       await context.PostAsync($"No records found");

                       throw new Exception("No records found");
                   }
               }
               return await Task.FromResult(false);
           }))
            #endregion
             /* .Confirm("Are you selected: "
             +"\n* {appointment} "
             + "\n* New date and time : {newDate} " +
             "? \n" +
             "(yes/no)")*/
             .OnCompletion(processOrder)
             .Build();
        }
        private static bool InactiveField(RescheduleFormAppThree state)
        {
            bool setActive = false;
            return setActive;
        }
    };
    #endregion
}