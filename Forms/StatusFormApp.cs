using AppicationBot.Ver._2.Models;
using AppicationBot.Ver._2.Services;
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

    public class StatusFormApp
    {
        #region Properties
        [Prompt("Can you enter the {&}, please? {||} ")]
        public int appoinmentId;
        public static IDialogContext context { get; set; }
        #endregion
        #region Creation of IForm
        public static IForm<StatusFormApp> BuildForm()
        {
            OnCompletionAsyncDelegate<StatusFormApp> processOrder = async (context, state) =>
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
                //Instance of library for manage customers
                WebAppoinmentsClientLibrary.Customers customerLibrary = new WebAppoinmentsClientLibrary.Customers();
                //Instance of library for manage appoinments
                WebAppoinmentsClientLibrary.Appoinments appointmentLibrary = new WebAppoinmentsClientLibrary.Appoinments();
                //Instance of library for manage cases
                WebAppoinmentsClientLibrary.Cases caseLibrary = new WebAppoinmentsClientLibrary.Cases();
                //Here we will to find the customer by customer id or personal id
                Customer customer = null;
                if (!String.IsNullOrEmpty(customerState.CustomerId.ToString()))
                {
                    //Get the object ObjectCustomer and inside of this the object Customer
                    try
                    {
                        customer = customerLibrary.GetCustomer(customerState.CustomerId).Customer;
                    }
                    catch (Exception)
                    {
                        // throw; here we not send the exception beacuse we need to do the next method below 
                    }
                }
                //If not found by customer id , we will try to find by personal id
                if (customer.Id <= 0)
                {
                    int idType = 0;
                    //GEt the object ObjectCustomer and inside of this the object Customer
                    try
                    {
                        customer = customerLibrary.GetCustomerByPersonalId(customerState.PersonaId, idType).Customer;
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
                    int customerId = customer.Id;

                    //get the appoinment id for filter in the below results
                    int appoinmentId = state.appoinmentId;


                    //Get a list of cases by customer id (no trae resultados)
                    List<Case> listCases = caseLibrary.GetCase(customerId).CaseList;
                    CaseCustomerResult cd = new CaseCustomerResult();

                    //Get the appoinment by appoinment id
                    AppointmentGetResults _appoinment = AppoinmentService.GetAppointmentById(appoinmentId);

                    //Declare a case for save the specific case that we search
                    Case caseForSaveResult = new Case();

                    string test = caseForSaveResult.OTUnitName;

                    if (_appoinment.ProcessId > 0)
                    {
                        await context.PostAsync($"The appointment have the next information " +
                            " \n* Process Id: " + _appoinment.ProcessId +
                            " \n* Appointment date: " + _appoinment.AppointmentDate +
                            " \n* Q-Code: " + _appoinment.QCode +
                            " \n* Q-Number: " + _appoinment.QNumber +
                            //" \n* Arrival date: " + _appoinment.ArrivalDate +
                            " \n* Customer Id: " + _appoinment.CustomerId +
                            // " \n* Date called: " + _appoinment.DateCalled +
                            // " \n* Name: " + _appoinment.Name +
                            " \n* Case Id: " + _appoinment.CaseId +
                            //  " \n* Is Walkin: " + _appoinment.IsWalkIn +
                            " \n* Is Active: " + _appoinment.IsActive +
                            " \n* Service Name: " + _appoinment.ServiceName
                            // " \n* Cancelreason name: " + _appoinment.CancelationReasonName 
                            );
                    }
                    // in other hand we can't find the record, so we will send the appropiate message
                    else
                    {
                        await context.PostAsync($"I don't found record with the appointment id: \n*" + state.appoinmentId);
                    }
                }
            };
            CultureInfo ci = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            var culture = Thread.CurrentThread.CurrentUICulture;
            var form = new FormBuilder<StatusFormApp>();
            var yesTerms = form.Configuration.Yes.ToList();
            var noTerms = form.Configuration.No.ToList();
            yesTerms.Add("Yes");
            noTerms.Add("No");
            form.Configuration.Yes = yesTerms.ToArray();
            return form.Message("Fill the information for search the appointment, please")
                       .Field(nameof(appoinmentId))
                      .Confirm("Are you selected the appointment id: {appoinmentId}: ? (yes/no)")
                      .AddRemainingFields()
                      .OnCompletion(processOrder)
                      .Build();
        }
        #endregion
    };
}
