using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace AppicationBot.Ver._2.Dialogs
{
    [Serializable]
    public class CancelDialog :IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"Cancel dialog");
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            await context.PostAsync($"I don't understand what do you want to say! you can try  one of the follow options: \n* Book \n* Reschedule \n* Status \n* Cancel");
            context.Wait(this.MessageReceivedAsync);

        }
    }
}