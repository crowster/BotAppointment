using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using AppicationBot.Ver._2;
using AppicationBot.Ver._2.Forms;
using Microsoft.Bot.Builder.FormFlow;
using System.Text;
using AppicationBot.Ver._2.Services;
using AppicationBot.Ver._2.Models;
using System.Threading;


/*
 Menu 1: includes "Sign In" and  "Register a new user" option
 Menu 2: includes "Book appointment","Get Status of the appointment", "Re-schedule the appointment" and "Cancel appointment
 */

namespace AppicationBot.Ver._2.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        #region Properties
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

        protected int count = 1;
        #endregion
        #region Methods

        #region Async methods
        public async Task StartAsync(IDialogContext context)
        {
            bool creatingUSer=false;
            if (!context.UserData.TryGetValue<bool>("creatingUser", out creatingUSer)) { creatingUSer = false; }
            if (creatingUSer == true)
            {
                PromptDialog.Choice(
             context,
             this.AfterChoiceSelected2,
             new[] {
                /*  SaveCustomerOption, AuthenticateOption },*/
                  AppointmentsOption, EnqueueOption },
             "How can I help you?",
             "I am sorry but I didn't understand that. I need you to select one of the options below",
             attempts: 2);
            }
            else {
                context.Wait(this.MessageReceivedAsync);
            }

            //bool creatingUser = false;
            //context.UserData.SetValue<bool>("creatingUser", creatingUser);

        }
        static IDialog<object> GotToBook()
        {
            return Chain.From(() => FormDialog.FromForm(AppoinmentForm.BuildForm));
        }
        //This method show the menu when the user is logged with correct credentials    
        public virtual async Task MessageReceivedAsync2(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            ACFCustomer customerState2 = new ACFCustomer();
            if (!context.PrivateConversationData.TryGetValue<ACFCustomer>("customerState", out customerState2)) { customerState2 = new ACFCustomer(); }
            int testCustomerStateId = customerState2.CustomerId;
            this.ShowOptions(context);
        }
        public virtual async Task MessageReceivedAsyncFromUserDialog(IDialogContext context)
        {
            PromptDialog.Choice(
              context,
              this.AfterChoiceSelected2,
              new[] {
                /*  SaveCustomerOption, AuthenticateOption },*/
                  AppointmentsOption, EnqueueOption },
              "What do you want to do today?",
              "I am sorry but I didn't understand that. I need you to select one of the options below",
              attempts: 2);
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            PromptDialog.Choice(
              context,
              this.AfterChoiceSelected2,
              new[] {
                /*  SaveCustomerOption, AuthenticateOption },*/
                  AppointmentsOption, EnqueueOption },
              "What do you want to do today?",
              "I am sorry but I didn't understand that. I need you to select one of the options below",
              attempts: 2);
        }

        #endregion
        #region Menú options
        /// <summary>
        /// This method show the menu 2 (see in the instructions on the top)
        /// </summary>
        /// <param name="context"></param>
        public void ShowOptions(IDialogContext context)
        {
            ACFCustomer customerState2 = new ACFCustomer();
            if (!context.PrivateConversationData.TryGetValue<ACFCustomer>("customerState", out customerState2)) { customerState2 = new ACFCustomer(); }
            int testCustomerStateId = customerState2.CustomerId;
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { BookOption, StatusOption, RescheduleOption, CancelOption, MainMenu,LogOut }, "Please select an option:", "Not a valid option", 3);
        }
        /// <summary>
        /// This show a menu of services
        /// </summary>
        /// <param name="context"></param>
        private void ShowOptionsServices(IDialogContext context)
        {
            List<string> serviceListNames = AppoinmentService.GetServicesNames();
            PromptDialog.Choice(context, this.OnOptionSelected, serviceListNames, "Select the service please", "Not a valid option", 3);
        }
        //This method will be executed after select one option of the menu 1
        private async Task AfterChoiceSelected(IDialogContext context, IAwaitable<string> result)
        {
            var selection = await result;

            switch (selection)
            {
                case SaveCustomerOption:
                    /*SaveCustomerForm.context = context;
                    var saveFormDialog = FormDialog.FromForm(SaveCustomerForm.BuildForm, FormOptions.PromptInStart);
                    context.Call(saveFormDialog, ResumeAfterSaveCustomerDialog);*/
                    SaveCustomerFormApp.context = context;
                    var saveFormDialog = FormDialog.FromForm(SaveCustomerFormApp.BuildForm, FormOptions.PromptInStart);
                    context.Call(saveFormDialog, ResumeAfterSaveCustomerDialog);
                    break;

                case AuthenticateOption:
                    AuthenticationForm.context = context;
                    var authenticationFormDialog = FormDialog.FromForm(AuthenticationForm.BuildForm, FormOptions.PromptInStart);
                    context.Call(authenticationFormDialog, ResumeAfterAutheticateCustomerDialog);
                    break;
            }
        }
        private async Task AfterChoiceSelected2(IDialogContext context, IAwaitable<string> result)
        {
            var selection = await result;
            //Here I set the creating user to false
            bool creatingUser = false;
            context.UserData.SetValue<bool>("creatingUser", creatingUser);
            switch (selection)
            {
                case AppointmentsOption:
                    ShowOptions(context);
                    break;

                case EnqueueOption:
                    EnqueueForm.context = context;
                    var enqueueFormDialog = FormDialog.FromForm(EnqueueForm.BuildForm, FormOptions.PromptInStart);
                    context.Call(enqueueFormDialog, ResumeAfterEnqueueCustomerDialog);
                    break;
                default:
                    context.Done(string.Empty);
                    break;
            }
        }
        //This method will be executed after select one of the option of book,cancel,get status, or reschedule an appointment
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
                        /*RescheduleFormAppThree.context = context;
                        var rescheduleFormDialog = FormDialog.FromForm(RescheduleFormAppThree.BuildForm, FormOptions.PromptInStart);
                        context.Call(rescheduleFormDialog, ResumeAfterRescheduleDialog);*/
                        ACFCustomer customerState3 = new ACFCustomer();
                        if (!context.UserData.TryGetValue<ACFCustomer>("customerState", out customerState3)) { customerState3 = new ACFCustomer(); }

                        var form3 = new FormDialog<ReservationReschedule>(
                        new ReservationReschedule(customerState3.CustomerId),
                        RescheduledFormAppReservation.BuildForm,
                        FormOptions.PromptInStart
                        //,null//,result.Entities}
                        );
                        context.Call(form3, ResumeAfterRescheduleDialog);

                        break;
                    case CancelOption:
                        // CancelForm.context = context;
                        //var cancelFormDialog = FormDialog.FromForm(CancelForm.BuildForm, FormOptions.PromptInStart);
                        //context.Call(cancelFormDialog, ResumeAfterCancelDialog);
                        /* CancelFormApp.context = context;
                         var cancelFormDialog = FormDialog.FromForm(CancelFormApp.BuildForm, FormOptions.PromptInStart);
                         context.Call(cancelFormDialog, ResumeAfterCancelDialog);*/
                        ACFCustomer customerState = new ACFCustomer();
                        if (!context.UserData.TryGetValue<ACFCustomer>("customerState", out customerState)) { customerState = new ACFCustomer(); }

                        var form = new FormDialog<ReservationCancel>(
                        new ReservationCancel(customerState.CustomerId),
                        CancelFormAppReservation.BuildForm,
                        FormOptions.PromptInStart
                        //,null//,result.Entities}
                        );
                        context.Call(form, ResumeAfterCancelDialog);
                        break;
                        case StatusOption:
                        /*StatusForm.context = context;
                        var statusFormDialog = FormDialog.FromForm(StatusFormApp.BuildForm, FormOptions.PromptInStart);
                        context.Call(statusFormDialog, ResumeAfterStatusDialog*/
                        /*StatusFormAppTwo.context = context;
                        var statusFormDialog = FormDialog.FromForm(StatusFormAppTwo.BuildForm, FormOptions.PromptInStart);
                        context.Call(statusFormDialog, ResumeAfterStatusDialog);*/
                        ACFCustomer customerState2 = new ACFCustomer();
                        if (!context.UserData.TryGetValue<ACFCustomer>("customerState", out customerState2)) { customerState2 = new ACFCustomer(); }

                        var form2 = new FormDialog<ReservationStatus>(
                        new ReservationStatus(customerState2.CustomerId),
                        StatusFormAppReservation.BuildForm,
                        FormOptions.PromptInStart
                        //,null//,result.Entities}
                        );
                        context.Call(form2, ResumeAfterStatusDialog);

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
                //context.Done(string.Empty);
                await StartAsync(context);

            }
        }
        #endregion
        #region Resumes Options
    
        /// <summary>
        /// This method will be executed after book an appointment
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
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
        private async Task ResumeAfterRescheduleDialog(IDialogContext context, IAwaitable<ReservationReschedule> result)
        {
            try
            {
                try { ReservationReschedule reservationReschedule= await result; }
                catch (Exception e)
                {
                    await context.PostAsync($"No records found");
                }
                // await context.PostAsync("Thank you for your time, Come back soon!  \n***********************************************************");
                this.ShowOptions(context);

            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
        }
        /// <summary>
        /// This method will be executed after cancel an appointment
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task ResumeAfterCancelDialog(IDialogContext context, IAwaitable<ReservationCancel> result)
        {
            try
            {
                try { ReservationCancel reservationCancel = await result; } catch (Exception e) {
                    await context.PostAsync($"No records found");
                }
                //await context.PostAsync(" Thank you for your time, Come back soon!  \n***********************************************************");
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
        private async Task ResumeAfterStatusDialog(IDialogContext context, IAwaitable<ReservationStatus> result)
        {
            try
            {
                try { ReservationStatus reservationCancel = await result; }
                catch (Exception e)
                {
                    await context.PostAsync($"No records found");
                }
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
                AuthenticationForm authenticationForm= await result;
                ACFCustomer customerState2 = new ACFCustomer();
                if (!context.PrivateConversationData.TryGetValue<ACFCustomer>("customerState", out customerState2)) { customerState2 = new ACFCustomer(); }
                int testCustomerStateId=customerState2.CustomerId;

                if (authenticationForm.logged == true)
                {
                    await context.PostAsync("Welcome " + authenticationForm.UserName +"!");
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
    }
    #endregion
         #endregion
}
