using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace AppicationBot.Ver._2.Forms
{
    [Serializable]
    public class CustomerForm
    {
        public int customerId;
        public string firstName;
        public string middleName;
        public string phoneNumber;
        public string email;
        public string male;

        public static IForm<CustomerForm> BuildForm() {
            OnCompletionAsyncDelegate<CustomerForm> processOrder = async (context, state) =>
            {
                await context.PostAsync($" The actual information is \n* Status:  ") ;
            };
            CultureInfo ci = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            FormBuilder<CustomerForm> form = new FormBuilder<CustomerForm>();

            return form.Message("Fill the information for register you, please")
                        .Field(nameof(customerId))
                        .Field(nameof(firstName))
                        .Field(nameof(middleName))
                        .Field(nameof(phoneNumber))
                        .Field(nameof(email))
                        .Field(nameof(male))

                      .Confirm("Are you selected the follow information: \n*   customerId: {customerId} \n* " +
                      "FirstName: {firstName}  \n* MiddleName:{middleName} \n* Phone Number: {phoneNumber} \n* Email: {email} "+
                      "\n* Male: {male}?"
                      + "(yes/no)")
                      .AddRemainingFields()
                      .Message("The process for create the customer has been started!")
                      .OnCompletion(processOrder)
                      .Build();

        }
    }
    private static bool ActiveField(CustomerForm state)
    {
        bool setActive = false;
        if (state.CustomerId > 0) ;
        setActive = false;


        return setActive;
    }
}