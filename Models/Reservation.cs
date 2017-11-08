using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppicationBot.Ver._2.Models
{
  
    [Serializable]
    public class Reservation
    {
        public IDialogContext Context { get; private set; }
        public Reservation(IDialogContext context)
        {
            this.Context = context;
        }
      

    }
}