using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using AppicationBot.Ver._2.Forms;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using System.Threading;

namespace AppicationBot.Ver._2.Dialogs
{
    [Serializable]
    public class RegisterUserDialog : IDialog<object>
    {
        byte[] arrayImage = null;
        int savedImageId = 0;
        string imageName =string.Empty;
        private const string LogOut = "Exit";
        private const string Register = "Create new User";


        public RegisterUserDialog(int _savedImageId, string _imageName)
        {
            this.savedImageId = _savedImageId;
            this.imageName = _imageName;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"Your photo is not registered yet, Starting the process for create an user with this photo ");

            context.Wait(this.MessageReceivedAsync);
        
        }


        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
           
            PromptDialog.Choice(
              context,
              this.AfterChoiceSelected,
              new[] {
                /*  SaveCustomerOption, AuthenticateOption },*/
                  Register },
              "What do you want to do?",
              "I am sorry but I didn't understand that. I need you to select one of the options below...",
              attempts: 2);
        }

        public virtual async Task MessageReceivedAsyncRedirectToRootDialog(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            
                context.Call(new RootDialog(), ResumeAfterRootDialog);
        }

        private async Task ResumeAfterRootDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Done(string.Empty);

        }

    

   

        private async Task AfterChoiceSelected(IDialogContext context, IAwaitable<string> result)
        {
            var selection = await result;

            switch (selection)
            {
                case Register:
                    SaveCustomerForm.imageName = imageName;
                    SaveCustomerForm.savedImageId = savedImageId;
                    SaveCustomerForm.context = context;
                    var saveFormDialog = FormDialog.FromForm(SaveCustomerForm.BuildForm, FormOptions.PromptInStart);
                    context.Call(saveFormDialog, ResumeAfterSaveCustomerDialog);
                    break;
                case LogOut:
                    context.Done(string.Empty);
                   // context.EndConversation(string.Empty);
                    break;
            }
        }




        private async Task ResumeAfterSaveCustomerDialog(IDialogContext context, IAwaitable<SaveCustomerForm> result)
        {
            SaveCustomerForm customer = await result;
            await context.PostAsync($"Welcome..." +customer.FirstName+" "+customer.LastName+  "!... ");
           
                context.Call(new RootDialog(), ResumeAfterRootDialog);

            // context.Wait(this.MessageReceivedAsyncRedirectToRootDialog);

            //context.Call(root, Microsoft.Bot.Builder.Dialogs.ResumeAfter);
        }
    }
}