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
using System.Threading;
using System.Threading.Tasks;
namespace AppicationBot.Ver._2.Forms
{
    public class StatusFormAppReservation
    {
        #region Creation of IForm
        public static IForm<ReservationStatus> BuildForm()
        {
            //Instance of library for manage appoinments
            WebAppoinmentsClientLibrary.Appoinments appointmentLibrary = new WebAppoinmentsClientLibrary.Appoinments();
            OnCompletionAsyncDelegate<ReservationStatus> processOrder = async (context, state) =>
            {
                //Get the appoinment by appoinment id
                AppointmentGetResults _appointment = AppoinmentService.GetAppointmentById(Convert.ToInt32(state.appointmentId));

                if (_appointment != null)
                {
                    await context.PostAsync($"Appointment Details:" +
                        " \n* Ticket: " + _appointment.QCode + _appointment.QNumber +
                        " \n* Service name: " + _appointment.ServiceName +
                        " \n* Appointment date: " + _appointment.AppointmentDate

                        );
                }
                // in other hand we can't find the record, so we will send the appropiate message
                else
                {
                    await context.PostAsync($"I don't found record with the appointment id: \n* " + state.appointmentId);
                }

            };
            CultureInfo ci = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            var culture = Thread.CurrentThread.CurrentUICulture;
            var form = new FormBuilder<ReservationStatus>();
            var yesTerms = form.Configuration.Yes.ToList();
            var noTerms = form.Configuration.No.ToList();
            yesTerms.Add("Yes");
            noTerms.Add("No");
            form.Configuration.Yes = yesTerms.ToArray();
            return form
                               .Field(new FieldReflector<ReservationStatus>(nameof(ReservationStatus.appointmentId))
                               .SetType(null)
                               .SetDefine(async (state, field) =>
                               {
                                   //Get the actual user state of the customer
                                   ACFCustomer customerState = new ACFCustomer();
                                   try
                                   {
                                      // if (!context.UserData.TryGetValue("customerState", out customerState)) { customerState = new ACFCustomer(); }
                                   }
                                   catch (Exception ex)
                                   {
                                       throw new Exception("Not exists a user session");
                                   }
                                   int customerIdState = 0;
                                   //customerIdState = customerState.CustomerId;
                                   customerIdState = state._customerId;

                                   string personalIdState = string.Empty;
                                   //personalIdState = customerState.PersonaId;
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
                                       //context.PostAsync($"The user is not valid");
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
                                               field
                                              .AddDescription(listCustomer.AppointmentId.ToString(), data + " | " + listCustomer.ServiceName)//here we put the process id and the date of the appointment of this process
                                              .AddTerms(listCustomer.AppointmentId.ToString(), data + " | " + listCustomer.ServiceName);
                                           }
                                           return await Task.FromResult(true);

                                       }
                                       else
                                       {
                                           await ReservationCancel.context.PostAsync($"No records found");
                                           // await context.PostAsync($"No records found");
                                           throw new Exception("No records found");
                                           //return Task.FromResult(false);

                                       }
                                       return await Task.FromResult(true);
                                   }
                               }))
                                   //  .Confirm("Are you selected:  " +
                                   //"\n* date : {appointmentId}: ? \n" +
                                   //"(yes/no)")
                               .Field(new FieldReflector<ReservationStatus>(nameof(ReservationStatus._customerId)).SetActive(InactiveField))
                               .AddRemainingFields()
                               .OnCompletion(processOrder)
                               .Build();
        }
        private static bool InactiveField(ReservationStatus state)
        {
            bool setActive = false;
            return setActive;
        }
    };
    #endregion
}
 