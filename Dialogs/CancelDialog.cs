using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using AppicationBot.Ver._2.Forms;
using Microsoft.Bot.Builder.FormFlow;
using AppicationBot.Ver._2.Models;

namespace AppicationBot.Ver._2.Dialogs
{
    [Serializable]
    public class CancelDialog :IDialog<object>
    {
        private const string BookOption = "Book appointment";
        private const string StatusOption = "Get appointment status";
        private const string RescheduleOption = "Re-schedule appointment";
        private const string CancelOption = "Cancel appointment";
        private const string SaveCustomerOption = "Register a new user";
        private const string ShoMenuOption = "Show menu";
        private const string AuthenticateOption = "Sign In";
        private const string LogOut = "Exit";
        private const string MainMenu = "Main Menu";
        private const string EnqueueOption = "Get In Line";
        private const string AppointmentsOption = "Appointments";
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            CancelFormApp.context = context;
            var cancelFormDialog = FormDialog.FromForm(CancelFormApp.BuildForm, FormOptions.PromptInStart);
            context.Call(cancelFormDialog, ResumeAfterCancelDialog);

        }

        private async Task ResumeAfterCancelDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                
                //await context.PostAsync(" Thank you for your time, Come back soon!  \n***********************************************************");
                this.ShowOptions(context);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
        }

        /// <summary>
        /// This method show the menu 2 (see in the instructions on the top)
        /// </summary>
        /// <param name="context"></param>
        public void ShowOptions(IDialogContext context)
        {
            ACFCustomer customerState2 = new ACFCustomer();
            if (!context.PrivateConversationData.TryGetValue<ACFCustomer>("customerState", out customerState2)) { customerState2 = new ACFCustomer(); }
            int testCustomerStateId = customerState2.CustomerId;
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { BookOption, StatusOption, RescheduleOption, CancelOption, MainMenu, LogOut }, "Please select an option:", "Not a valid option", 3);
        }
        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                // ACFCustomer customerState2 = new ACFCustomer();
                //if (!context.PrivateConversationData.TryGetValue<ACFCustomer>("customerState", out customerState2)) { customerState2 = new ACFCustomer(); }
                //int testCustomerStateId = customerState2.CustomerId;
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case BookOption:
                        BookFormApp.context = context;
                        var bookFormDialog = FormDialog.FromForm(BookFormApp.BuildForm, FormOptions.PromptInStart);
                        context.Call(bookFormDialog, ResumeAfterBookDialog);
                        /*BookFormApp.context = context;
                        var bookFormDialog = FormDialog.FromForm(BookFormApp.BuildForm, FormOptions.PromptInStart);
                        context.Call(bookFormDialog, ResumeAfterBookDialog);*/
                        break;
                    case RescheduleOption:
                        //RescheduleFormApp.context = context;
                        //var rescheduleFormDialog = FormDialog.FromForm(RescheduleFormApp.BuildForm, FormOptions.PromptInStart);
                        //context.Call(rescheduleFormDialog, ResumeAfterRescheduleDialog);
                        RescheduleFormAppThree.context = context;
                        var rescheduleFormDialog = FormDialog.FromForm(RescheduleFormAppThree.BuildForm, FormOptions.PromptInStart);
                        context.Call(rescheduleFormDialog, ResumeAfterRescheduleDialog);
                        break;
                    case CancelOption:
                        // CancelForm.context = context;
                        //var cancelFormDialog = FormDialog.FromForm(CancelForm.BuildForm, FormOptions.PromptInStart);
                        //context.Call(cancelFormDialog, ResumeAfterCancelDialog);
                        /* CancelFormApp.context = context;
                         var cancelFormDialog = FormDialog.FromForm(CancelFormApp.BuildForm, FormOptions.PromptInStart);
                         context.Call(cancelFormDialog, ResumeAfterCancelDialog);*/

                        context.Call(new CancelDialog(), ResumeAfterCancelDialog);


                        //var form = new FormDialog<ReservationCancel>(
                        //new ReservationCancel(context),
                        //CancelFormAppReservation.BuildForm,
                        //FormOptions.PromptInStart
                        //,null//,result.Entities}
                        // );
                        //context.Call(form, ResumeAfterCancelDialog);
                        break;
                    case StatusOption:
                        /*StatusForm.context = context;
                        var statusFormDialog = FormDialog.FromForm(StatusFormApp.BuildForm, FormOptions.PromptInStart);
                        context.Call(statusFormDialog, ResumeAfterStatusDialog*/
                        StatusFormAppTwo.context = context;
                        var statusFormDialog = FormDialog.FromForm(StatusFormAppTwo.BuildForm, FormOptions.PromptInStart);
                        context.Call(statusFormDialog, ResumeAfterStatusDialog);

                        break;
                    case SaveCustomerOption:
                        SaveCustomerFormApp.context = context;
                        var saveCustomerFormDialog = FormDialog.FromForm(SaveCustomerFormApp.BuildForm, FormOptions.PromptInStart);
                        context.Call(saveCustomerFormDialog, ResumeAfterSaveCustomerDialog);
                        break;
                    case MainMenu:
                        //context.Wait.StartAsync(context);
                        bool creatingUser = true;
                        context.UserData.SetValue<bool>("creatingUser", creatingUser);
                        await StartAsync(context);
                        break;
                    case LogOut:
                        //context.PrivateConversationData.SetValue<ACFCustomer>("customerState", customerState);
                        context.Done(string.Empty);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");
                context.Done(string.Empty);
            }
        }

        private async Task ResumeAfterBookDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                //var ticketNumber = await result;
                // await context.PostAsync($"Thank you for your time, come back soon!  \n***********************************************************");
                //context.Wait(this.MessageReceivedAsync);
                this.ShowOptions(context);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");

            }


        }
        /// <summary>
        /// Thi method will be executed after reschedule an appointment
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task ResumeAfterRescheduleDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                // await context.PostAsync("Thank you for your time, Come back soon!  \n***********************************************************");
                this.ShowOptions(context);

            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
        }
   
        /// <summary>
        /// This method will be executed after get the status of an appointment
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task ResumeAfterStatusDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                // await context.PostAsync("Thank you for your time, Come back soon!  \n***********************************************************");
                this.ShowOptions(context);

            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
        }
        /// <summary>
        /// This mehtod will be executed after enqueue the customer
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task ResumeAfterEnqueueCustomerDialog(IDialogContext context, IAwaitable<EnqueueForm> result)
        {
            try
            {
                //await context.PostAsync("Thank you for your time, Come back soon!  \n***********************************************************");
                bool creatingUser = true;
                context.UserData.SetValue<bool>("creatingUser", creatingUser);
                await StartAsync(context);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }

        }
        /// <summary>
        /// This method will be executed after the user login in the application , if the credentials are no correct send the menu for login or create new user.
        /// in other way show the menu of options for manage an appointment
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task ResumeAfterAutheticateCustomerDialog(IDialogContext context, IAwaitable<AuthenticationForm> result)
        {
            try
            {
                AuthenticationForm authenticationForm = await result;
                ACFCustomer customerState2 = new ACFCustomer();
                if (!context.PrivateConversationData.TryGetValue<ACFCustomer>("customerState", out customerState2)) { customerState2 = new ACFCustomer(); }
                int testCustomerStateId = customerState2.CustomerId;

                if (authenticationForm.logged == true)
                {
                    await context.PostAsync("Welcome " + authenticationForm.UserName + "!");
                    context.Wait(this.MessageReceivedAsync2);
                }
                else
                {
                    await this.StartAsync(context);
                }
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
        }
        /// <summary>
        /// This method will be executed after save the customer 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task ResumeAfterSaveCustomerDialog(IDialogContext context, IAwaitable<SaveCustomerFormApp> result)
        {
            try
            {
                SaveCustomerFormApp saveCustomeForm = await result;
                // await context.PostAsync("Thank you for your time, Come back soon!* **********************************************************");
                this.ShowOptions(context);

            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }

        }

        public virtual async Task MessageReceivedAsync2(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            ACFCustomer customerState2 = new ACFCustomer();
            if (!context.PrivateConversationData.TryGetValue<ACFCustomer>("customerState", out customerState2)) { customerState2 = new ACFCustomer(); }
            int testCustomerStateId = customerState2.CustomerId;
            this.ShowOptions(context);
        }
    }
}