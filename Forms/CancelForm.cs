using AppicationBot.Ver._2.Models;
using AppicationBot.Ver._2.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using OTempus.Library.Class;
using OTempus.Library.Result;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace AppicationBot.Ver._2.Forms
{
    [Serializable]

    public class CancelForm
    {
        #region Properties
        public int appoinmentId;
        public static IDialogContext context { get; set; }
        #endregion
        #region Creation of IForm
        /// <summary>
        /// Creation of the IForm(Form flow for cancel an appointment)
        /// </summary>
        /// <returns></returns>
        public static IForm<CancelForm> BuildForm()
        {
            OnCompletionAsyncDelegate<CancelForm> processOrder = async (context, state) =>
            {
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
                int customerIdState = 0;
                customerIdState = customerState.CustomerId;
                string personalIdState = string.Empty;
                personalIdState = customerState.PersonaId;

                //Instance of library for manage customers
                WebAppoinmentsClientLibrary.Customers customerLibrary = new WebAppoinmentsClientLibrary.Customers();
                //Instance of library for manage appoinments
                WebAppoinmentsClientLibrary.Appoinments appointmentLibrary = new WebAppoinmentsClientLibrary.Appoinments();
                //Instance of library for manage cases
                WebAppoinmentsClientLibrary.Cases caseLibrary = new WebAppoinmentsClientLibrary.Cases();
                //Here we will to find the customer by customer id or personal id
                Customer customer = null;
                if (!String.IsNullOrEmpty(customerIdState.ToString()))
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
                else {
                    int idType = 0;
                    //GEt the object ObjectCustomer and inside of this the object Customer
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
                }
                else
                {
                    //Declaration of Calendar Get Slots Results oobject
                    CalendarGetSlotsResults slotToShowInformation = new CalendarGetSlotsResults();
                    //Set the parameters for get the expected appoinments
                    int customerTypeId = 0;
                    string customerTypeName = "";
                    int customerId = customer.Id;
                    //This variables are in hard code because... 
                    string fromDate= "09/21/2017 ";
                    string toDate = "10/21/2018 ";

                    //DateTime fromDate = DateTime.Today;
                    //DateTime toDate = DateTime.Today.AddDays(5);
                    string typeSeriaizer = "XML";
                    //get the appoinment id fore filter in the below results
                    int appoinmentId = state.appoinmentId;

                    //At first I need to find the appoinment by appoiment id, for saw the actual status
                    Appointment appoinment=appointmentLibrary.GetAppoinment(appoinmentId).AppointmentInformation;

                    //Get the case for get the status of the appoinment (check for more information QFlow documentation)
                    // Case cases= caseLibrary.GetCase(customerId).CaseList;
                    //cases.
                    //Declare the object for keept the value of the appoinment find it
                    CustomerGetExpectedAppointmentsResults customerGetExpectedAppointmentsResults = new CustomerGetExpectedAppointmentsResults();
                    //Declaration of the ocject to save the result of the GetExpectedAppoinment
                    ObjectCustomerGetExpectedAppointmentsResults objectCustomerGetExpectedAppointmentsResults = new ObjectCustomerGetExpectedAppointmentsResults();
                    objectCustomerGetExpectedAppointmentsResults = customerLibrary.GetExpectedAppoinment(customerTypeId, customerTypeName, customerId, fromDate, toDate, typeSeriaizer);
                    foreach (CustomerGetExpectedAppointmentsResults listCustomer in objectCustomerGetExpectedAppointmentsResults.ListCustomerGetExpectedAppointmentsResults)
                    {
                        if (listCustomer.AppointmentId.Equals(appoinmentId)) {
                            customerGetExpectedAppointmentsResults.CaseId = listCustomer.CaseId;
                            customerGetExpectedAppointmentsResults.ProcessId = listCustomer.ProcessId;
                            customerGetExpectedAppointmentsResults.ServiceId = listCustomer.ServiceId;
                            customerGetExpectedAppointmentsResults.AppointmentTypeId = listCustomer.AppointmentTypeId;
                            customerGetExpectedAppointmentsResults.AppointmentTypeName = listCustomer.AppointmentTypeName;
                        }
                        //Maybe we not found the appoiment that we want, because when pass a short time, a job of sql is in charge to change the status of the appoinments
                        //it means , by default the state is in expected but when pass determinate time, it can change to absent for example
                        //So the recomendation is find the appointment by id and administrate the error
                    }

                    //Then when we have our object , we can cancel the appoinment because we have his process id
                   
                    if (customerGetExpectedAppointmentsResults.ProcessId>0)
                    {
                        appointmentLibrary.CancelAppoinment(customerGetExpectedAppointmentsResults.ProcessId,
                            0,0,0,"notes",false,0,0
                            );
                    }
                    // in other hand we can't find the record, so we will send the appropiate message
                    else
                    {
                        await context.PostAsync($"I don't found a record with appoinment Id: \n*" + state.appoinmentId);
                    }
                }
            };
            CultureInfo ci = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            var culture = Thread.CurrentThread.CurrentUICulture;
            var form = new FormBuilder<CancelForm>();
            var yesTerms = form.Configuration.Yes.ToList();
            var noTerms = form.Configuration.No.ToList();
            yesTerms.Add("Yes");
            noTerms.Add("No");
            form.Configuration.Yes = yesTerms.ToArray();
           
            return form.Message("Fill the information for cancel the appoinment, please")
                      .Field(nameof(appoinmentId))
                      .Confirm("Are you selected:  "+
                      "\n* appoinmentId: {appoinmentId}: ? \n"+
                      "(yes/no)")
                      .AddRemainingFields()
                      .Message("The process for cancel the appoinment has been started!")
                      .OnCompletion(processOrder)
                      .Build();
        }
    };
    #endregion
}