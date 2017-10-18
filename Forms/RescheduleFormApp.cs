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
    [Serializable]
    public class RescheduleFormApp
    {
        #region Properties
        [Prompt("Can you enter the date when you booked your appointment please, MM/dd/yyyy ? {||}")]
        public string startDate;
        [Prompt("Can you enter the new date and time for reschedule your appointment please,  MM/dd/yyyy hh:mm:ss? {||}")]
        public string newDate;
        public string date;
        public string processId;
        public string appointment;
        //una vez que tengo esas dos listas, buscamos el process id del appointment, y en la lista de cases buscamos
        //el initial process id y comparamos, si son iguales tomamos ese case 
        public static IDialogContext context { get; set; }
        #endregion
        #region Creation of IForm
        /// <summary>
        /// This method create an Iform (form flow) for reschedule an appointement
        /// </summary>
        /// <returns></returns>
        public static IForm<RescheduleFormApp> BuildForm()
        {
            OnCompletionAsyncDelegate<RescheduleFormApp> processOrder = async (context, state) =>
            {
                try
                {
                    //Get the appointment id from the option selected
                    int appoitmentId = Convert.ToInt32(Utilities.Util.GetAppoitmentIdFromBotOption(state.appointment));

                    WebAppoinmentsClientLibrary.Customers customerLibrary = new WebAppoinmentsClientLibrary.Customers();
                    //Get the actual user state of the customer
                    ACFCustomer customerState = new ACFCustomer();
                    try
                    {
                        if (!context.UserData.TryGetValue<ACFCustomer>("customerState", out customerState)) { customerState = new ACFCustomer(); }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Not exists a user session");
                    }
                    string fromDate = state.startDate;
                    string toDate = state.newDate;
                    //Declaration of Calendar Get Slots Results object
                    CalendarGetSlotsResults slotToShowInformation = new CalendarGetSlotsResults();
                    //Set the parameters for get the expected appoinments
                    int customerTypeId = 0;
                    string customerTypeName = "";
                    string typeSeriaizer = "XML";
                    int customerId = customerState.CustomerId;
                    //Declare the object for keept the value of the appoinment find it
                    CustomerGetExpectedAppointmentsResults customerGetExpectedAppointmentsResults = new CustomerGetExpectedAppointmentsResults();
                    //Declaration of the ocject to save the result of the GetExpectedAppoinment
                    ObjectCustomerGetExpectedAppointmentsResults objectCustomerGetExpectedAppointmentsResults = new ObjectCustomerGetExpectedAppointmentsResults();
                    objectCustomerGetExpectedAppointmentsResults = customerLibrary.GetExpectedAppoinment(customerTypeId, customerTypeName, customerId, fromDate, toDate, typeSeriaizer);
                    if (objectCustomerGetExpectedAppointmentsResults.ListCustomerGetExpectedAppointmentsResults.Count > 0)
                        foreach (CustomerGetExpectedAppointmentsResults listCustomer in objectCustomerGetExpectedAppointmentsResults.ListCustomerGetExpectedAppointmentsResults)
                        {
                            if (appoitmentId == listCustomer.AppointmentId) { 
                            //string data = listCustomer.AppointmentId + "" + listCustomer.AppointmentDate;
                            customerGetExpectedAppointmentsResults.AppointmentId = listCustomer.AppointmentId;
                            customerGetExpectedAppointmentsResults.CaseId = listCustomer.CaseId;
                            customerGetExpectedAppointmentsResults.ProcessId = listCustomer.ProcessId;
                            customerGetExpectedAppointmentsResults.ServiceId = listCustomer.ServiceId;
                            customerGetExpectedAppointmentsResults.AppointmentTypeId = listCustomer.AppointmentTypeId;
                            customerGetExpectedAppointmentsResults.AppointmentTypeName = listCustomer.AppointmentTypeName;
                            }
                        }
                    string appointmentId = Utilities.Util.GetOrdinalNumberFromBotOption(state.appointment);
                    // ACFAppointment appointment = new ACFAppointment();
                    // appointment = Utilities.Util.GetAppointment(state.appointment.ToString());
                    int result= AppoinmentService.RescheduleAppoinment(customerGetExpectedAppointmentsResults.ProcessId, state.newDate, customerGetExpectedAppointmentsResults.ServiceId);
                    await context.PostAsync($"The rescheduled is completed with the new Id: "+ result);
                }
                catch (Exception ex)
                {
                    await context.PostAsync(ex.Message.ToString());
                }
            };
            CultureInfo ci = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            var culture = Thread.CurrentThread.CurrentUICulture;
            var form = new FormBuilder<RescheduleFormApp>();
            var yesTerms = form.Configuration.Yes.ToList();
            var noTerms = form.Configuration.No.ToList();
            yesTerms.Add("Yes");
            noTerms.Add("No");
            form.Configuration.Yes = yesTerms.ToArray();
            return form.Message("Fill the information for reschedule the appointment, please")
                        .Field(nameof(startDate))
                        .Field(nameof(newDate))
                        .Field(new FieldReflector<RescheduleFormApp>(nameof(date)).SetActive(InactiveField))
                        .Field(new FieldReflector<RescheduleFormApp>(nameof(processId)).SetActive(InactiveField))
                        .Field(new FieldReflector<RescheduleFormApp>(nameof(appointment))
                           .SetType(null)
                           .SetDefine(async (state, value) =>
                           {
                               //Instance of library for manage customers
                               WebAppoinmentsClientLibrary.Customers customerLibrary = new WebAppoinmentsClientLibrary.Customers();

                               if (!String.IsNullOrEmpty(state.startDate) && !String.IsNullOrEmpty(state.newDate))
                               {
                                   //Get the actual user state of the customer
                                   ACFCustomer customerState = new ACFCustomer();
                                   try
                                   {
                                       if (!context.PrivateConversationData.TryGetValue<ACFCustomer>("customerState", out customerState)) { customerState = new ACFCustomer(); }
                                   }
                                   catch (Exception ex)
                                   {
                                       throw new Exception("Not exists a user session");
                                   }
                                   string fromDate = state.startDate;
                                   string toDate = state.newDate;
                                   //Declaration of Calendar Get Slots Results oobject
                                   CalendarGetSlotsResults slotToShowInformation = new CalendarGetSlotsResults();
                                   //Set the parameters for get the expected appoinments
                                   int customerTypeId = 0;
                                   string customerTypeName = "";
                                   string typeSeriaizer = "XML";
                                   int customerId = customerState.CustomerId;
                                   //Declare the object for keept the value of the appoinment find it
                                   CustomerGetExpectedAppointmentsResults customerGetExpectedAppointmentsResults = new CustomerGetExpectedAppointmentsResults();
                                   //Declaration of the ocject to save the result of the GetExpectedAppoinment
                                   ObjectCustomerGetExpectedAppointmentsResults objectCustomerGetExpectedAppointmentsResults = new ObjectCustomerGetExpectedAppointmentsResults();
                                   objectCustomerGetExpectedAppointmentsResults = customerLibrary.GetExpectedAppoinment(customerTypeId, customerTypeName, customerId, fromDate, toDate, typeSeriaizer);
                                   if (objectCustomerGetExpectedAppointmentsResults.ListCustomerGetExpectedAppointmentsResults.Count > 0)
                                       foreach (CustomerGetExpectedAppointmentsResults listCustomer in objectCustomerGetExpectedAppointmentsResults.ListCustomerGetExpectedAppointmentsResults)
                                       {
                                           //string data = listCustomer.AppointmentId + ".- "+ listCustomer.ServiceName +" at "+ listCustomer.AppointmentDate;
                                           string data = "Appointment Id:"+listCustomer.AppointmentId + ".- \n* Actual appointment date: "+ listCustomer.AppointmentDate;
                                           customerGetExpectedAppointmentsResults.AppointmentId = listCustomer.AppointmentId;
                                           customerGetExpectedAppointmentsResults.CaseId = listCustomer.CaseId;
                                           customerGetExpectedAppointmentsResults.ProcessId = listCustomer.ProcessId;
                                           customerGetExpectedAppointmentsResults.ServiceId = listCustomer.ServiceId;
                                           customerGetExpectedAppointmentsResults.AppointmentTypeId = listCustomer.AppointmentTypeId;
                                           customerGetExpectedAppointmentsResults.AppointmentTypeName = listCustomer.AppointmentTypeName;
                                           value
                                                       .AddDescription(data, data)
                                                       .AddTerms(data, data);
                                       }
                                   else
                                   {
                                       return await Task.FromResult(false);
                                   }
                               }
                               return await Task.FromResult(true);
                           }))
                       /* .Confirm("Are you selected: "
                       +"\n* {appointment} "
                       + "\n* New date and time : {newDate} " +
                       "? \n" +
                      "(yes/no)")*/
                      .Message("The process for reschdule the appoinment has been started!")
                      .OnCompletion(processOrder)
                      .Build();
        }
        private static bool InactiveField(RescheduleFormApp state)
        {
            bool setActive = false;
            return setActive;
        }
    };
    #endregion
}