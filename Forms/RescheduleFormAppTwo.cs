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
    public class RescheduleFormAppTwo
    {
        #region Properties
        [Prompt("Can you select one option, please? {||}")]
        public string processIdServiceId;
        [Prompt("Can you enter the new date and time for reschedule your appointment please,  MM/dd/yyyy hh:mm:ss? {||}")]
        public string newDate;
        public static IDialogContext context { get; set; }
        #endregion
        #region Creation of IForm
        /// <summary>
        /// This method create an Iform (form flow) for reschedule an appointement
        /// </summary>
        /// <returns></returns>
        public static IForm<RescheduleFormAppTwo> BuildForm()
        {
            //Instance of library for manage appoinments
            WebAppoinmentsClientLibrary.Appoinments appointmentLibrary = new WebAppoinmentsClientLibrary.Appoinments();
            OnCompletionAsyncDelegate<RescheduleFormAppTwo> processOrder = async (context, state) =>
            {
                try
                {
                    Char delimiter = '.';
                    string[] arrayInformation = state.processIdServiceId.Split(delimiter);
                    int processId = Convert.ToInt32(arrayInformation[0]);
                    int serviceId = Convert.ToInt32(arrayInformation[1]);
                    int result = AppoinmentService.RescheduleAppoinment(processId, state.newDate,serviceId);
                    await context.PostAsync($"The rescheduled is completed with the new Id: " + result);
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
            var form = new FormBuilder<RescheduleFormAppTwo>();
            var yesTerms = form.Configuration.Yes.ToList();
            var noTerms = form.Configuration.No.ToList();
            yesTerms.Add("Yes");
            noTerms.Add("No");
            form.Configuration.Yes = yesTerms.ToArray();
            return form
                           .Field(new FieldReflector<RescheduleFormAppTwo>(nameof(processIdServiceId))
                           .SetType(null)
                           .SetDefine(async(state, field) =>
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
                                           string processIdAndServiceId =appointment.ProcessId+"."+ appointment.ServiceId;
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
                           .Field(nameof(newDate))
                      /* .Confirm("Are you selected: "
                      +"\n* {appointment} "
                      + "\n* New date and time : {newDate} " +
                      "? \n" +
                      "(yes/no)")*/
                      .Message("The process for reschedule the appointment has been started!")
                      .OnCompletion(processOrder)
                      .Build();
        }
        private static bool InactiveField(RescheduleFormAppTwo state)
        {
            bool setActive = false;
            return setActive;
        }
    };
    #endregion
}