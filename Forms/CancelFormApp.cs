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

namespace AppicationBot.Ver._2.Forms
{
    [Serializable]
    public class CancelFormApp
    {
        #region Properties
        [Prompt("Select the appointment you want to cancel: {||}")]
        public string processId;
        public static IDialogContext context { get; set; }
        #endregion
        #region Creation of IForm
        /// <summary>
        /// Creation of the IForm(Form flow for cancel an appointment)
        /// </summary>
        /// <returns></returns>
        public static IForm<CancelFormApp> BuildForm()
        {
            //Instance of library for manage appoinments
            WebAppoinmentsClientLibrary.Appoinments appointmentLibrary = new WebAppoinmentsClientLibrary.Appoinments();
            OnCompletionAsyncDelegate<CancelFormApp> processOrder = async (context, state) =>
            {
              

                //Then when we have our object , we can cancel the appointment because we have his process id

                if (Convert.ToInt32(state.processId) > 0)
                {
                   ResultObjectBase result= appointmentLibrary.CancelAppoinment(Convert.ToInt32(state.processId),
                        0, 0, 0, "notes", false, 0, 0
                        );
                    if (result.ReturnCode > 0) {
                    }else
                    await context.PostAsync($"Your appointment was cancelled!" );
                }
                // in other hand we can't find the record, so we will send the appropiate message
                else
                {
                    await context.PostAsync($"I don't found a record with appointment Id: \n*" + state.processId);
                }
            };
            CultureInfo ci = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            var culture = Thread.CurrentThread.CurrentUICulture;
            var form = new FormBuilder<CancelFormApp>();
            var yesTerms = form.Configuration.Yes.ToList();
            var noTerms = form.Configuration.No.ToList();
            yesTerms.Add("Yes");
            noTerms.Add("No");
            form.Configuration.Yes = yesTerms.ToArray();

            return form        //.Message("Select one of the dates availables for cancel the appointment, please")
                              .Field(new FieldReflector<CancelFormApp>(nameof(processId))
                              .SetType(null)
                              .SetDefine(async(state, field) =>
                              {
                                  //Get the actual user state of the customer
                                  ACFCustomer customerState = new ACFCustomer();
                                  try
                                  {
                                      if (!context.UserData.TryGetValue("customerState", out customerState)) { customerState = new ACFCustomer(); }
                                      //if (!context.PrivateConversationData.TryGetValue("customerState", out customerState)) { customerState = new ACFCustomer(); }

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
                                              field
                                             .AddDescription(listCustomer.ProcessId.ToString(), data + "|" + listCustomer.ServiceName +" in "+ listCustomer.UnitName)//here we put the process id and the date of the appointment of this process
                                             .AddTerms(listCustomer.ProcessId.ToString(), data + "|" + listCustomer.ServiceName+" in "+listCustomer.UnitName);
                                          }
                                          return await Task.FromResult(true);

                                      }
                                      else
                                      {
                                         await  context.PostAsync($"No records found");
                                          throw new Exception("No records found");

                                      }
                                  }
                                  
                              }))
                              .Confirm("Are you sure you want to cancel the appointment?  " +
                              "\n* (yes/no) ")
                              .AddRemainingFields()
                              .OnCompletion(processOrder)
                              .Build();
        }
    };
    #endregion
}