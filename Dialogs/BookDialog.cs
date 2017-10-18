using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using AppicationBot.Ver._2.Forms;
using Microsoft.Bot.Builder.FormFlow;

namespace AppicationBot.Ver._2.Dialogs
{
    [Serializable]
    public class BookDialog : IDialog<int>
    {
        static IDialog<AppoinmentForm> MakeRoot()
        {
            return Chain.From(() => FormDialog.FromForm(AppoinmentForm.BuildForm));
        }

        public async Task StartAsync(IDialogContext context)
        {
            RootDialog rd = new RootDialog();
            var bookAppoinmentform = FormDialog.FromForm(AppoinmentForm.BuildForm,FormOptions.PromptInStart);
           // context.Call(bookAppoinmentform, ));

            //context.Wait(this.MessageReceivedAsync);
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            await context.PostAsync($"Thank you for your preference, come back soon !");
            context.Wait(this.MessageReceivedAsync);
        }


        #region Resumes Options
        private async Task ResumeAfterBookDialog(IDialogContext context, IAwaitable<AppoinmentForm> result)
        {
            var ticketNumber = await result;
           string color= ticketNumber.Color.Value.ToString();
            await context.PostAsync($"The color is: {color}.");
           // context.Done(string.Empty);

            context.Wait(this.MessageReceivedAsync);
        }
        #endregion
    }
}