using AppicationBot.Ver._2.Models;
using AppicationBot.Ver._2.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
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
    public class AuthenticationForm
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public static IDialogContext context { get; set; }
        public bool logged;

        public static IForm<AuthenticationForm> BuildForm()
        {
            OnCompletionAsyncDelegate<AuthenticationForm> processOrder = async (context, state) =>
            {
                //Instance of library for manage customers
                WebAppoinmentsClientLibrary.Customers customerLibrary = new WebAppoinmentsClientLibrary.Customers();
                //Here we will to find the customer by customer id or personal id
                Customer customer = null;
                if (!String.IsNullOrEmpty(state.UserName.ToString()))
                {
                    //Get the object ObjectCustomer and inside of this the object Customer
                    try
                    {
                        customer = customerLibrary.GetCustomerByPersonalId(state.UserName, 0).Customer;
                        if (customer.Id > 0)
                        {
                            /* Create the userstate */
                            //Instance of the object ACFCustomer for keept the customer
                            Models.ACFCustomer customerState = new Models.ACFCustomer();
                            customerState.CustomerId = customer.Id;
                            customerState.FirstName = customer.FirstName;
                            customerState.PhoneNumber = customer.TelNumber1;
                            customerState.Sex = Convert.ToInt32(customer.Sex);
                            customerState.PersonaId = customer.PersonalId;
                            context.PrivateConversationData.SetValue<ACFCustomer>("customerState", customerState);

                            ///Show the information of the user
                            await context.PostAsync($"I find the record with the next information "
                            + " \n* First Name :" + customer.FirstName
                            + " \n* Last name :" + customer.LastName
                            + " \n* State :" + customer.IsActive
                            + " \n* Phone number :" + customer.TelNumber1
                            + " \n* Sex :" + customer.Sex
                            );
                            state.logged = true;
                            ACFCustomer customerState2 = new ACFCustomer();
                            try
                            {
                                if (!context.PrivateConversationData.TryGetValue<ACFCustomer>("customerState", out customerState2)) { customerState2 = new ACFCustomer(); }

                            }
                            catch (Exception ex)
                            {

                                throw new Exception("Not exists a user session");
                            }
                        }

                        else
                        {
                            await context.PostAsync($"The credentials are not valid...: " + state.UserName);
                            state.logged = false;
                        }
                    }
                    catch (Exception)
                    {
                        // throw; here we not send the exception beacuse we need to do the next method below 
                    }
                }
            };
            CultureInfo ci = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            var culture = Thread.CurrentThread.CurrentUICulture;
            var form = new FormBuilder<AuthenticationForm>();
            var yesTerms = form.Configuration.Yes.ToList();
            var noTerms = form.Configuration.No.ToList();
            yesTerms.Add("Yes");
            noTerms.Add("No");
            form.Configuration.Yes = yesTerms.ToArray();
            return form.Message("Fill the follow information, please")
                       .Field(nameof(UserName))
                       .Field(nameof(Password))
                       .Field(new FieldReflector<AuthenticationForm>(nameof(logged)).SetActive(InactiveField))
                       //.Confirm("Are you selected the appoinment id: {appoinmentId}: ? (yes/no)")
                       .AddRemainingFields()
                       //.Message("The process for create the appoinment has been started!")
                       .OnCompletion(processOrder)
                       .Build();
        }
        private static bool InactiveField(AuthenticationForm state)
        {
            return false;
        }
    };
}
