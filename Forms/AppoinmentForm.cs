using AppicationBot.Ver._2.Models;
using AppicationBot.Ver._2.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using OTempus.Library.Class;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppicationBot.Ver._2.Forms
{

    public enum Color
    {
        green = 1, yellow = 2

    }

    [Serializable]
    public class AppoinmentForm
    {
        public string Office;
        public string Service;
        public string StartDate;
        public string Calendar;

        public string Test;

        public Color? Color;


        static List<OTempus.Library.Class.Service> GetServicesOtempues()
        {
            return AppoinmentService.GetListServices(0);
        }

        static List<OTempus.Library.Class.Calendar> GetCalendar(string serviceId, string date)
        {
            return AppoinmentService.GetCalendars(serviceId, date);
        }

        private bool ActiveTimeZone(AppoinmentForm state)
        {
            bool setActive = true;
            if (!state.StartDate.Contains("09"));
                setActive = false;


            return setActive;

        }
        //private static ConcurrentDictionary<CultureInfo, IForm<AppoinmentForm>> _forms = new ConcurrentDictionary<CultureInfo, IForm<AppoinmentForm>>();

        public static IForm<AppoinmentForm> BuildForm()
        {


        OnCompletionAsyncDelegate<AppoinmentForm> processOrder = async (context, state) =>
            {
                await context.PostAsync("We will send you the status.");
            };
            CultureInfo ci = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            FormBuilder<AppoinmentForm> form = new FormBuilder<AppoinmentForm>();

            return form.Message("Fill the information for schedule an appoinment, please")
                        .Field("Office", validate:
                         async (state, value) =>
                       {
                           ValidateResult vResult = new ValidateResult();
                           List<ACFAppointment> listAppoinments = await Services.AppoinmentService.GetAppoinments();
                           StringBuilder response = new StringBuilder();
                           if (listAppoinments.Count <= 0)
                           {
                               response.Append("No se encontraron items ").ToString();
                               vResult.Feedback = string.Format(response.ToString());
                               vResult.IsValid = false;
                           }
                           else
                           {
                               foreach (ACFAppointment item in listAppoinments)
                               {
                                   response.Append(item.Name + ", \n");
                               }
                               //vResult.Feedback = string.Format(response.ToString());
                               vResult.Feedback = "sdas";
                               vResult.Value = "asdas";
                               vResult.IsValid = true;
                           }
                           return vResult;
                        })
                        .Field(new FieldReflector<AppoinmentForm>(nameof(Service))
                           .SetType(null)
                           .SetDefine((state, field) =>
                            {
                                string office;
                                if (!String.IsNullOrEmpty(state.Office))
                                {
                                    office = state.Office.ToString();
                                }
                                if (GetServicesOtempues().Count > 0)
                                {
                                    foreach (var prod in GetServicesOtempues())
                                        field
                                            .AddDescription(prod.Name, prod.Name)
                                            .AddTerms(prod.Name, prod.Name);
                                    return Task.FromResult(true);
                                }
                                else
                                {
                                    return Task.FromResult(false);
                                }
                            }))
                             .Field("StartDate", validate:
                             async (state, response) =>
                             {
                                 ValidateResult vResult = new ValidateResult();
                                 var result = new ValidateResult { IsValid = true, Value = response };
                                 var address = (response as string).Trim();

                                 List<OTempus.Library.Class.Calendar> listCalendars = new List<OTempus.Library.Class.Calendar>();
                                 if (GetServicesOtempues().Count > 0) {
                                     //Service two and date...
                                      listCalendars = GetCalendar("2", "9/21/2017");
                                 }
                                 List<ACFAppointment> listAppoinments = await Services.AppoinmentService.GetAppoinments();
                                 List<CalendarGetSlotsResults> listGetAvailablesSlots = new List<CalendarGetSlotsResults>();
                                 StringBuilder responses = new StringBuilder();
                                 responses.Append("No existen slots").ToString();
                                 vResult.Feedback = string.Format(responses.ToString());

                                 vResult.IsValid = false;
                                 if (listCalendars.Count > 0)
                                 {
                                     listGetAvailablesSlots= AppoinmentService.GetAvailablesSlots(listCalendars[0].Id);
                                     bool availablesSlots=true;
                                     foreach (OTempus.Library.Class.CalendarGetSlotsResults calendarSlots in listGetAvailablesSlots) {
                                         //The status 0 is available, the status 4 is locked, 1 reserved
                                         if (calendarSlots.Status!=0) {
                                             availablesSlots = false;
                                             //In the example here we reserve the first that we find
                                             //AppoinmentService.ReserveSlots(listCalendars[0].Id,calendarSlots.OrdinalNumber,1);
                                           
                                             /*Example to freeup slots*/
                                             AppoinmentService.FreeUpSlots(listCalendars[0].Id, calendarSlots.OrdinalNumber);
                                             /*Example to lock slots*/
                                             //AppoinmentService.LockSlots(listCalendars[0].Id, calendarSlots.OrdinalNumber);

                                         }
                                     }
                                     if (availablesSlots) { 
                                         vResult.Feedback = "Si hay slots";
                                         vResult.Value = "slot 1";
                                         vResult.IsValid = true;
                                     }
                                     else
                                     {
                                         responses.Append("No existen slots disponibles").ToString();
                                         vResult.Feedback = string.Format(response.ToString());
                                         vResult.IsValid = false;
                                     }
                                 }
                                 return vResult;
                             })
                           .Field(new FieldReflector<AppoinmentForm>(nameof(Calendar))
                           .SetType(null)
                           .SetDefine((state, field) =>
                           {
                               string date;
                               string service;
                               if (!String.IsNullOrEmpty(state.StartDate) && !String.IsNullOrEmpty(state.Service)){
                                   date = state.StartDate.ToString();
                                   service = state.Service.ToString();
                                   if (GetServicesOtempues().Count > 0)
                                   {
                                       // foreach (var prod in GetCalendar("2", "9/19/2017"))
                                       foreach (var prod in GetCalendar(service, date))
                                       field
                                               .AddDescription(prod.CalendarDate.ToString(), prod.CalendarDate.ToString())
                                               .AddTerms(prod.CalendarDate.ToString(), prod.CalendarDate.ToString());
                                       return Task.FromResult(true);
                                   }
                                   else
                                   {
                                       return Task.FromResult(false);
                                   }
                               }

                               return Task.FromResult(false);

                           }))                       
                             /*.Field(nameof(Calendar),validate: async (state, value) =>
                             {
                               ValidateResult vResult = new ValidateResult();
                               var values = ((List<object>)value).OfType<Appoinment>();
                               var result = new ValidateResult { IsValid = true, Value = values };


                               List<Appoinment> listAppoinments = await AppoinmentService.GetAppoinments();
                               StringBuilder response = new StringBuilder();
                               //vResult.Feedback = string.Format(response.ToString());
                               // vResult.Feedback = state.Office.ToString();
                               vResult.IsValid = true;
                               vResult.Value = state.Office.ToString() + " " + state.Service.ToString();
                               vResult.Feedback = state.Office.ToString() + " " + state.Service.ToString();
                               return vResult;
                           })*/
                          .Field("Test", validate:
                           async (state, value) =>
                           {
                               string office = state.Calendar.ToString();
                           ValidateResult vResult = new ValidateResult();
                           List<ACFAppointment> listAppoinments =  await AppoinmentService.GetAppoinments();
                           StringBuilder response = new StringBuilder();
                               //vResult.Feedback = string.Format(response.ToString());
                               // vResult.Feedback = state.Office.ToString();
                               vResult.IsValid = true;
                               vResult.Value = state.Office.ToString()+" "+state.Service.ToString();
                               vResult.Feedback = state.Office.ToString() + " " + state.Service.ToString();
                               return vResult;
                        })
                        .Field(nameof(Color))

                      .Confirm("Are you selected the office: {Office} \n, the color: {Color}  \n  And the Service {Service}? (yes/no)")
                      .AddRemainingFields()
                      .Message("The process for create the appoinment has been started!")
                      .OnCompletion(processOrder)
                      .Build();

        }
    };

}