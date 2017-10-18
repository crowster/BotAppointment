using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace AppicationBot.Ver._2.Dialogs
{
    [Serializable]
    public class StatusDialog : RootDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"status dialog");
            context.Wait(base.MessageReceivedAsync);



        }
    }
}