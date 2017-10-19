using AppicationBot.Ver._2.Models;
using AppicationBot.Ver._2.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using OTempus.Library.Class;
using OTempus.Library.Result;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AppicationBot.Ver._2.Forms
{
    [Serializable]
    public class BookForm
    {
        #region Properties


        //This property is for save the office
        public string Office;
        //This property will keep the service of the office
        public string Service;
        //the prompt tool allow us create a customized message for send to the user in the form flow
        //i COMMENTED THIS LINE BEACUSE i WILL IPLEMENT THE OPTION FOR GET THE NEXT 3 DAYS
        // [Prompt("Can you enter the date in the follow format please, MM/dd/yyyy  ? {||}")]
        public string StartDateAndTime;
        //public string Days;

        //This is the hour of the selected slot
        public string Hour;
        //This property is for see a general hour 1,2,3,13,14,16 etc
        public string GeneralHour;
        #region public properties that you will not see in the flow
            //This properties are used for keep values, but you will not see in the form flow
            public string CalendarId;
            public string OrdinalSlot;
            /// <summary>
            /// This property allow us get or set the context data from an specific Dialog 
            /// </summary>
            public static IDialogContext context { get; set; }
        #endregion


        #endregion

        #region Methods


        /// <summary>
        /// Get a list of the actual services, optional we can filtee by the unit selected
        /// </summary>
        /// <returns></returns>
        /// 
        static List<OTempus.Library.Class.Service> GetServicesOtempues(int unitId)
        {
            return AppoinmentService.GetListServices(unitId);
        }
        /// <summary>
        /// Get a list of the actual units configured
        /// </summary>
        /// <returns></returns>
        static List<OTempus.Library.Class.Unit> GetListUnitsConfigured()
        {
            return AppoinmentService.GetListUnitsConfigured();
        }
        /// <summary>
        /// Get calendars by service id and an specific date
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        static List<OTempus.Library.Class.Calendar> GetCalendar(string serviceId, string date)
        {
            try
            {
                return AppoinmentService.GetCalendars(serviceId, date);
            }
            catch (Exception ex)
            {
                throw new Exception("error in GetCalendar: " + ex.Message);
            }
        }




        #endregion

        #region Creation of IForm

        /// <summary>
        /// Create a IForm(Form flow) based in BookForm object
        /// </summary>
        /// <returns></returns>
        /// 
        public static IForm<BookForm> BuildForm()
        {
            #region processOrder
            OnCompletionAsyncDelegate<BookForm> processOrder = async (context, state) =>
            {
                //Get the actual user state of the customer
                ACFCustomer customerState = new ACFCustomer();
                try
                {
                    if (!context.UserData.TryGetValue<ACFCustomer>("customerState", out customerState)) { customerState = new ACFCustomer(); }
                }
                catch (Exception ex)
                {
                    throw new Exception("Not exists a user session");
                }
                try
                {
                    ResultObjectBase resultObjectBase = new ResultObjectBase();
                    int serviceID = 0;
                    int unitId = 0;
                    unitId = Utilities.Util.GetUnitIdFromBotOption(state.Office);
                    string ordinalNumber = Utilities.Util.GetOrdinalNumberFromBotOption(state.Hour);
                    try
                    {
                        List<Service> listService = AppoinmentService.listServicesByName(state.Service, 1, "en", false, unitId);
                        serviceID = listService[0].Id;
                    }
                    catch (Exception)
                    {
                        throw new Exception("I don't found appointments with the service: " + state.Service);
                    }

                    try
                    {
                        resultObjectBase = AppoinmentService.SetAppoinment(0, serviceID, customerState.CustomerId, Convert.ToInt32(ordinalNumber), Convert.ToInt32(
                       state.CalendarId));
                        await context.PostAsync("Your appointment has been scheduled with the id: " + resultObjectBase.Id);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("I can't book the appointment, error: " + ex.Message);
                    }
                }
                catch (Exception ex)
                {

                    await context.PostAsync($"Failed with message: {ex.Message}");
                }
            };
            #endregion
            CultureInfo ci = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            FormBuilder<BookForm> form = new FormBuilder<BookForm>();
            return form.Message("Fill the information for schedule an appointment, please")
                               .Field(new FieldReflector<BookForm>(nameof(Office))
                               .SetType(null)
                               .SetDefine((state, field) =>
                               {
                                   List<Unit> list = GetListUnitsConfigured();
                                   string data = string.Empty;
                                   if (list.Count > 0)
                                   {
                                       foreach (var unit in list)
                                       {
                                       //This format is important, follow this structure
                                       data = unit.Id + "." + unit.Name;
                                           field
                                               .AddDescription(data, data)
                                               .AddTerms(data, data);
                                       }
                                       return Task.FromResult(true);
                                   }
                                   else
                                   {
                                       return Task.FromResult(false);
                                   }
                               }))
                              .Field(new FieldReflector<BookForm>(nameof(Service))
                              .SetType(null)
                              .SetDefine((state, field) =>
                              {
                                   int unitId = 0;
                                   if (!String.IsNullOrEmpty(state.Office))
                                   {
                                   //Get the unit id by the option above selected
                                   unitId = Utilities.Util.GetUnitIdFromBotOption(state.Office);
                                       if (GetServicesOtempues(unitId).Count > 0)
                                       {
                                           foreach (var prod in GetServicesOtempues(unitId))
                                               field
                                                   .AddDescription(prod.Name, prod.Name)
                                                   .AddTerms(prod.Name, prod.Name);
                                           return Task.FromResult(true);
                                       }
                                       else
                                       {
                                           return Task.FromResult(false);
                                       }
                                   }
                                   return Task.FromResult(true);
                               }))
                              .Field(new FieldReflector<BookForm>(nameof(StartDateAndTime))
                              .SetType(null)
                              .SetDefine((state, field) =>
                              {
                                  string date;
                                  string service;
                                  List<Service> listService;
                                  int unitId = 0;
                                  List<OTempus.Library.Class.Calendar> listCalendars;
                                  StringBuilder response = new StringBuilder();
                                  List<CalendarGetSlotsResults> listGetAvailablesSlots = new List<CalendarGetSlotsResults>();
                                  if (!String.IsNullOrEmpty(state.Service) && !String.IsNullOrEmpty(state.Office) )
                                  {
                                      unitId = Utilities.Util.GetUnitIdFromBotOption(state.Office);
                                      try
                                      {
                                          listService = AppoinmentService.listServicesByName(state.Service, 1, "en", false, unitId);
                                          int serviceID = listService[0].Id;
                                          listCalendars = new List<OTempus.Library.Class.Calendar>();

                                          if (GetServicesOtempues(unitId).Count > 0)
                                          {
                                              //listCalendars = GetCalendar(serviceID.ToString(), dateAndTime);
                                              //i commented this line beacuse i will take today and Thre days more, for get the calendars, and then get the dates of this calendars
                                              listCalendars = GetCalendar(serviceID.ToString(), DateTime.Today.ToString());

                                              if (listCalendars.Count == 0)
                                              {
                                                  response.Append("Not exists calendars in this date, try it one more time , or write 'quit' for exit").ToString();
                                                  //vResult.Feedback = string.Format(response.ToString());
                                                  //vResult.IsValid = false;
                                                  return Task.FromResult(false);
                                              }
                                              else
                                              {
                                                  foreach (var calendar in listCalendars)
                                                  {
                                                      string data =calendar.Id+".-"+calendar.CalendarDate.ToString();
                                                      field
                                                          .AddDescription(data, data)
                                                          .AddTerms(data, data);
                                                  }
                                                  return Task.FromResult(true);
                                              }//End else 
                                          }//End if
                                      }//End try
                                      catch (Exception e) { }
                                  }
                                  return Task.FromResult(true);
                              }))

                              /*.Field("StartDateAndTime", validate:
                              async (state, responses) =>
                              {
                                  string date;
                                  string service;
                                  List<Service> listService;
                                  List<OTempus.Library.Class.Calendar> listCalendars;
                                  //ValidateResult vResult = new ValidateResult();
                                  var vResult = new ValidateResult { IsValid = true, Value = responses };
                                  var dateAndTime = (responses as string).Trim();
                                  StringBuilder response = new StringBuilder();
                                  List<CalendarGetSlotsResults> listGetAvailablesSlots = new List<CalendarGetSlotsResults>();
                                  if (!String.IsNullOrEmpty(state.Service) && !String.IsNullOrEmpty(state.Office) && !String.IsNullOrEmpty(dateAndTime))
                                  {
                                      int unitId = Utilities.Util.GetUnitIdFromBotOption(state.Office);
                                      try
                                      {
                                      listService = AppoinmentService.listServicesByName(state.Service, 1, "en", false, unitId);
                                      //It find almost the time one record, but in the case that found two, 
                                      int serviceID = listService[0].Id;
                                          listCalendars = new List<OTempus.Library.Class.Calendar>();
                                          date = dateAndTime; service = state.Service;
                                          if (GetServicesOtempues(unitId).Count > 0)
                                          {
                                          //Service two and date...
                                          listCalendars = GetCalendar(serviceID.ToString(), dateAndTime);
                                              if (listCalendars.Count == 0)
                                              {
                                                  response.Append("Not exists calendars in this date, try it one more time , or write 'quit' for exit").ToString();
                                                  vResult.Feedback = string.Format(response.ToString());
                                                  vResult.IsValid = false;
                                              }
                                              else
                                              {
                                                  vResult.IsValid = true;
                                                  listGetAvailablesSlots = AppoinmentService.GetAvailablesSlots(listCalendars[0].Id);
                                                  if (listGetAvailablesSlots.Count > 0)
                                                  {
                                                      vResult.IsValid = true;
                                                  }
                                                  else
                                                  {
                                                      response.Append("There are'n t availables slots in this date, try it one more time , or write 'quit' for exit").ToString();
                                                      vResult.Feedback = string.Format(response.ToString());
                                                      vResult.IsValid = false;
                                                  }
                                              }//End else 
                                      }//End if GetServicesOtempues(unitId).Count > 0
                                  }//End try
                                  catch (Exception ex)
                                      {
                                      //throw new Exception("Here are the error: " + ex.Message);
                                      await context.PostAsync($"Failed with message: {ex.Message}");
                                      }
                                  }
                                  return vResult;
                               })*/
                               .Field(new FieldReflector<BookForm>(nameof(CalendarId)).SetActive(InactiveField))
                               .Field(new FieldReflector<BookForm>(nameof(OrdinalSlot)).SetActive(InactiveField))
                               /*new source implementation 10/09/17 */
                               .Field(new FieldReflector<BookForm>(nameof(GeneralHour))
                               .SetType(null)
                               .SetDefine(async (state, value) =>
                               {
                                   string date;
                                   string service;
                                   List<Service> listService;
                                   List<OTempus.Library.Class.Calendar> listCalendars;
                                   List<CalendarGetSlotsResults> listGetAvailablesSlots;
                                   if (!String.IsNullOrEmpty(state.StartDateAndTime) && !String.IsNullOrEmpty(state.Service) && !String.IsNullOrEmpty(state.Office))
                                   {
                                       int unitId = Utilities.Util.GetUnitIdFromBotOption(state.Office);
                                       string calendarId = Utilities.Util.GetCalendarIdFromBotOption(state.StartDateAndTime);
                                       //asign the calendar id
                                       state.CalendarId = calendarId;
                                       string dateSelected = Utilities.Util.GetDateFromBotOption(state.StartDateAndTime);


                                       try
                                       {
                                       listService = AppoinmentService.listServicesByName(state.Service, 1, "en", false, unitId);
                                       //It find almost the time one record, but in the case that found two, 
                                       int serviceID = listService[0].Id;
                                           listCalendars = new List<OTempus.Library.Class.Calendar>();
                                           date = dateSelected; service = state.Service;
                                           if (GetServicesOtempues(unitId).Count > 0)
                                           {
                                           //Service two and date...
                                           listCalendars = GetCalendar(serviceID.ToString(), date);
                                           }
                                       //List<Appoinment> listAppoinments = await Services.AppoinmentService.GetAppoinments();
                                       listGetAvailablesSlots = new List<CalendarGetSlotsResults>();
                                           StringBuilder response = new StringBuilder();
                                           response.Append("Not exists slots").ToString();
                                       }
                                       catch (Exception ex)
                                       {
                                           throw new Exception("Here are the error: " + ex.Message);
                                       }
                                       date = dateSelected;
                                       service = state.Service.ToString();
                                       if (listCalendars.Count > 0)
                                       {
                                           listGetAvailablesSlots = AppoinmentService.GetAvailablesSlots(Convert.ToInt32(state.CalendarId));
                                           int cont = 0;
                                           foreach (OTempus.Library.Class.CalendarGetSlotsResults calendarSlots in listGetAvailablesSlots)
                                           {
                                                   if (calendarSlots.Status.ToString() == "Vacant")
                                                   {
                                                   //I commented this line because I need to cut the message
                                                   // string data =calendarSlots.OrdinalNumber+".-"+ calendarSlots.DisplayStartTime.ToString() +"-"+calendarSlots.DisplayEndTime+"=>"+calendarSlots.Status;
                                                   string data = calendarSlots.OrdinalNumber + ".-" + calendarSlots.DisplayStartTime.ToString() + "-" + calendarSlots.DisplayEndTime;
                                                   string hour = Utilities.Util.GetHourFromStartDate(calendarSlots.DisplayStartTime.ToString()); ;
                                                   value
                                                              // .AddDescription(data, data).AddTerms(data, data);
                                                               .AddDescription(hour, hour).AddTerms(hour, hour);

                                                       cont++;
                                                   }
                                           }
                                           return await Task.FromResult(true);
                                       }
                                       else
                                       {
                                           return await Task.FromResult(false);
                                       }
                                   }
                                   return await Task.FromResult(false);
                                }))
                               /* New source implementation*/
                               .Field(new FieldReflector<BookForm>(nameof(Hour))
                               .SetType(null)
                               .SetDefine(async (state, value) =>
                               {
                                   string date;
                                   string service;
                                   List<Service> listService;
                                   List<OTempus.Library.Class.Calendar> listCalendars;
                                   List<CalendarGetSlotsResults> listGetAvailablesSlots;
                                   if (!String.IsNullOrEmpty(state.StartDateAndTime) && !String.IsNullOrEmpty(state.Service) && !String.IsNullOrEmpty(state.Office) && !String.IsNullOrEmpty(state.GeneralHour))
                                   {
                                       string generalHour = state.GeneralHour;
                                       int unitId = Utilities.Util.GetUnitIdFromBotOption(state.Office);
                                       try
                                       {
                                       listService = AppoinmentService.listServicesByName(state.Service, 1, "en", false, unitId);
                                       //It find almost the time one record, but in the case that found two, 
                                       int serviceID = listService[0].Id;
                                           listCalendars = new List<OTempus.Library.Class.Calendar>();
                                           date = state.StartDateAndTime; service = state.Service;
                                           if (GetServicesOtempues(unitId).Count > 0)
                                           {
                                           //Service two and date...
                                           listCalendars = GetCalendar(serviceID.ToString(), date);
                                           }
                                       //List<Appoinment> listAppoinments = await Services.AppoinmentService.GetAppoinments();
                                       listGetAvailablesSlots = new List<CalendarGetSlotsResults>();
                                           StringBuilder response = new StringBuilder();
                                           response.Append("Not exists slots").ToString();
                                       }
                                       catch (Exception ex)
                                       {
                                           throw new Exception("Here are the error: " + ex.Message);
                                       }
                                       date = state.StartDateAndTime.ToString();
                                       service = state.Service.ToString();
                                       if (listCalendars.Count > 0)
                                       {
                                           listGetAvailablesSlots = AppoinmentService.GetAvailablesSlots(listCalendars[0].Id);
                                           int cont = 0;
                                           foreach (OTempus.Library.Class.CalendarGetSlotsResults calendarSlots in listGetAvailablesSlots)
                                           {
                                            
                                                   string hour = Utilities.Util.GetHourFromStartDate(calendarSlots.DisplayStartTime.ToString()); ;

                                                   if (calendarSlots.Status.ToString() == "Vacant" && hour==generalHour)
                                                   {
                                                   //I commented this line because I need to cut the message
                                                   // string data =calendarSlots.OrdinalNumber+".-"+ calendarSlots.DisplayStartTime.ToString() +"-"+calendarSlots.DisplayEndTime+"=>"+calendarSlots.Status;
                                                   string data = calendarSlots.OrdinalNumber + ".-" + calendarSlots.DisplayStartTime.ToString() + "-" + calendarSlots.DisplayEndTime;
                                                   //assign the calendar id
                                                   //state.CalendarId = calendarSlots.CalendarId.ToString();
                                                       value
                                                               .AddDescription(data, data)
                                                               .AddTerms(data, data);
                                                       cont++;
                                                   }
                                           }
                                           return await Task.FromResult(true);
                                       }
                                       else
                                       {
                                           return await Task.FromResult(false);
                                       }
                                   }
                                   return await Task.FromResult(false);
                            }))
                           /* .Confirm("Are you selected the information: " +
                           "\n* Office: {Office} " +
                           "\n* Slot:  {Hour} " +
                           "\n* Service:  {Service}? \n" +
                           "(yes/no)") This lines are commented because, when the user select no, can crate inconsistence when user book the appointment (I try to solve by the best way)*/
                          .AddRemainingFields()
                          .Message("The process for create the appointment has been started!")
                          .OnCompletion(processOrder)
                          .Build();
         }
        /// <summary>
        /// This method is inside a delegate that inactive the specific field when form flow is running, through the bool property
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private static bool InactiveField(BookForm state)
        {
            bool setActive = false;
            return setActive;
        }

        #endregion

    };
}