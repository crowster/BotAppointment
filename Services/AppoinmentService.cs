using AppicationBot.Ver._2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using OTempus.Library.Class;
using OTempus.Library.Result;

namespace AppicationBot.Ver._2.Services
{
    public class AppoinmentService
    {
        #region Services

        /// <summary>
        /// This method returns a list of availables appoinments (Example)
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>> GetServices()
        {
            List<Service> listServices = new List<Service>();
            List<string> listServicesName = new List<string>();

            WebAppoinmentsClientLibrary.Appoinments appoinment = new WebAppoinmentsClientLibrary.Appoinments();
            listServices = appoinment.GetServices(1);
            foreach (Service service in listServices)
            {
                listServicesName.Add(service.Name);
            }
            return listServicesName;
        }
        /// <summary>
        /// Geth a list of services
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public static List<Service> GetListServices(int unitId)
        {
            List<Service> listServices = new List<Service>();
            List<string> listServicesName = new List<string>();
            WebAppoinmentsClientLibrary.Appoinments appoinment = new WebAppoinmentsClientLibrary.Appoinments();
            listServices = appoinment.GetServices(unitId);
            return listServices;
        }
        /// <summary>
        /// This method cancel an appoinment
        /// </summary>
        /// <param name="slotToShowInformation"></param>
        /// <param name="dateInput"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public static List<Service> listServicesByName(string serviceName, int queueManagement, string languageCode, bool onlyLinked
          , int unitId)
        {
            int result = 0;
            WebAppoinmentsClientLibrary.Services services = new WebAppoinmentsClientLibrary.Services();
            List<Service> listResult = new List<Service>();
            try
            {
                listResult = services.GetListServicesByName(serviceName, queueManagement, languageCode, onlyLinked, unitId);
                return listResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Calendars


        /// <summary>
        /// Get a list of calendars by service Id and date
        /// </summary>
        /// <param name="_serviceId"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public static List<Calendar> GetCalendars(string _serviceId, string startDate)
        {
            List<Calendar> listCalendars = new List<Calendar>();
            int serviceId = Convert.ToInt32(_serviceId);
            int appoinmentTypeId = 1;
            // DateTime dateFromString =DateTime.Parse(startDate, System.Globalization.CultureInfo.InvariantCulture);
            DateTime dateFromString =DateTime.Today;
            DateTime endDate = dateFromString.AddDays(3);
            WebAppoinmentsClientLibrary.Calendars calendar = new WebAppoinmentsClientLibrary.Calendars();
            listCalendars = calendar.GetCalendars(serviceId, dateFromString, endDate, appoinmentTypeId, "XML");
            return listCalendars;
        }
        /// <summary>
        /// Get a list of strings with the name of the services
        /// </summary>
        /// <returns></returns>
        internal static List<string> GetServicesNames()
        {
            List<Service> listServices = new List<Service>();
            List<string> listServicesName = new List<string>();

            WebAppoinmentsClientLibrary.Appoinments appoinment = new WebAppoinmentsClientLibrary.Appoinments();
            listServices = appoinment.GetServices(1);
            foreach (Service service in listServices)
            {
                listServicesName.Add(service.Name);
            }
            return listServicesName;
        }


        #endregion

        #region Appointments

        /// <summary>
        /// This method get an appoinment by id
        /// </summary>
        /// <returns></returns>
        public static AppointmentGetResults GetAppointmentById(int appointmentId)
        {
            AppointmentGetResults appoinment = new AppointmentGetResults();
            try
            {
                WebAppoinmentsClientLibrary.Appoinments _appoinmentLibray = new WebAppoinmentsClientLibrary.Appoinments();
                appoinment = _appoinmentLibray.GetAppoinment(appointmentId).AppointmentInformation;
            }
            catch (Exception)
            {

                throw;
            }
            return appoinment;
        }
        /// <summary>
        /// This method returns a list of availables appointments (Example)
        /// </summary>
        /// <returns></returns>
        public static async Task<List<ACFAppointment>> GetAppoinments()
        {
            List<ACFAppointment> listAppoinment = new List<ACFAppointment>();
            //Creating inputs for add to the appoinmnetList

            ACFAppointment appoinment = new ACFAppointment();
            appoinment.AppoinmentId = 1;
            appoinment.Name = "appoinmnet1";
            appoinment.StartDate = DateTime.Today;

            ACFAppointment appoinmentTwo = new ACFAppointment();
            appoinmentTwo.AppoinmentId = 1;
            appoinmentTwo.Name = "appoinmnet2";
            appoinmentTwo.StartDate = DateTime.Today.AddDays(2);

            //Add the itemp of appoinments to the list
            listAppoinment.Add(appoinment);
            listAppoinment.Add(appoinmentTwo);

            return listAppoinment;
        }
        /// <summary>
        /// This method get a list of available slots of the calendar, we will pass the calendar Id
        /// </summary>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public static int CancelAppoinment(int processId, string fromDate, string toDate, string comment)
        {
            int result = 0;
            WebAppoinmentsClientLibrary.Appoinments appoinment = new WebAppoinmentsClientLibrary.Appoinments();
            int cancelReasonId = 0;
            int parentCaseId = 0;
            int cancelationType = 0;
            bool removeWaitingListRequest = false;
            int customerTretmentPlaneId = 0;
            int treatmentPlantype = 0;
            DateTime fromDateString =
            DateTime.Parse(fromDate, System.Globalization.CultureInfo.InvariantCulture);
            DateTime toDatestring =
            DateTime.Parse(toDate, System.Globalization.CultureInfo.InvariantCulture);
            try
            {
                appoinment.CancelAppoinment(
                    processId,
                    cancelReasonId,
                    parentCaseId,
                    cancelationType,
                    comment,
                    removeWaitingListRequest,
                    customerTretmentPlaneId,
                    treatmentPlantype
                );
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// This method reschedule an appoinment
        /// </summary>
        /// <param name="slotToShowInformation"></param>
        /// <param name="dateInput"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public static int RescheduleAppoinment(int originalProcessId, string dateAndTimeString, int serviceId)
        {
            int result = 0;
            WebAppoinmentsClientLibrary.Appoinments appoinment = new WebAppoinmentsClientLibrary.Appoinments();
            int cancelReasonId = 0;
            int appoinmentTypeId = 0;
            int treatmentPlantype = 0;
            DateTime dateAndTime;
            try
            {
                dateAndTime =
                DateTime.Parse(dateAndTimeString, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
                throw;
            }
            try
            {
                ResultObjectBase resultObjectBase = appoinment.RescheduleAppoinment(
                    originalProcessId,
                    cancelReasonId,
                    serviceId,
                    dateAndTime,
                    appoinmentTypeId,
                    treatmentPlantype
                );
                result = resultObjectBase.Id;
                if (resultObjectBase.ReturnCode > 0)
                {
                    throw new Exception(resultObjectBase.ReturnMessage);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// This method can set or book a new appointment
        /// </summary>
        /// <param name="parentCaseId"></param>
        /// <param name="serviceId"></param>
        /// <param name="customerId"></param>
        /// <param name="slotOrdinalNumber"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public static ResultObjectBase SetAppoinment(int parentCaseId,
               int serviceId,
               int customerId,
               int slotOrdinalNumber,
               int calendarId)
        //int duration,)
        {
            ResultObjectBase resultObjectBase = new ResultObjectBase();
            WebAppoinmentsClientLibrary.Appoinments appoinment = new WebAppoinmentsClientLibrary.Appoinments();
            DateTime dateandTime = DateTime.Today;
            int appoimentTypeId = 0;
            int treatmentPlantype = 0;
            string subject = "";
            string notes = "sd";
            string extRef = "asd";
            bool preventAutoQueue = false;
            string languageCode = "en";
            bool isWalkIn = false;
            bool forceSimultaneousAppoiment = false;
            bool forceWastedDuration = false;
            bool autoFreeUp = false;
            int treatmentPlanId = 0;
            int customerTreatmentPlan = 0;
            int duration = 0;
            int basedonAppoimentRequestId = 0;
            bool simulationOnly = false;
            bool forceNoDynamicVacancy = false;
            int userId = 0;
            string typeSerializer = "XML";
            try
            {
                resultObjectBase = appoinment.SetAppoinment(parentCaseId, calendarId, serviceId, dateandTime, customerId, appoimentTypeId, treatmentPlanId, subject, notes, extRef, preventAutoQueue, languageCode, isWalkIn, forceSimultaneousAppoiment, forceWastedDuration, autoFreeUp, treatmentPlanId, customerTreatmentPlan, slotOrdinalNumber, calendarId, duration, basedonAppoimentRequestId, simulationOnly, forceWastedDuration, userId, typeSerializer
                );
                return resultObjectBase;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Calendar Slots


        public static List<CalendarGetSlotsResults> GetAvailablesSlots(int calendarId)
        {
            List<CalendarSlot> listCalendars = new List<CalendarSlot>();
            List<CalendarGetSlotsResults> listCalendarGetSlotsResult = new List<CalendarGetSlotsResults>();
            List<string> listSlotsName = new List<string>();
            bool appoinmentsOnly = false;
            string typeSerializer = "XML";
            WebAppoinmentsClientLibrary.Calendars calendars = new WebAppoinmentsClientLibrary.Calendars();
            listCalendarGetSlotsResult = calendars.GetSlotsByCalendarId(calendarId, appoinmentsOnly, typeSerializer);
            return listCalendarGetSlotsResult;
        }
        /// <summary>
        /// This method reserve an slot
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="reserveId"></param>
        public static void ReserveSlots(int calendarId, int ordinalNumber, int reserveId)
        {
            Dictionary<int, int> myDictionary = new Dictionary<int, int>();

            //here we need to add the ordinal number an the id resarvation cause, if don'0t have reservations you should create them
            myDictionary.Add(ordinalNumber, 1);

            string typeSerializer = "XML";

            WebAppoinmentsClientLibrary.Calendars calendars = new WebAppoinmentsClientLibrary.Calendars();
            calendars.ReserveSlots(calendarId, myDictionary, typeSerializer);
        }
        /// <summary>
        /// This method free up the reserved slots
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="ordinalNumber"></param>
        public static void FreeUpSlots(int calendarId, int ordinalNumber)
        {
            List<int> listIntSlots = new List<int>();

            //here we need to add the ordinal number an the id resarvation cause, if don'0t have reservations you should create them
            listIntSlots.Add(ordinalNumber);

            string typeSerializer = "XML";

            WebAppoinmentsClientLibrary.Calendars calendars = new WebAppoinmentsClientLibrary.Calendars();
            calendars.FreeUpSlots(calendarId, listIntSlots, typeSerializer);
        }
        /// <summary>
        /// This methos lock the specific slot by ordinal number
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="ordinalNumber"></param>
        public static void LockSlots(int calendarId, int ordinalNumber)
        {
            List<int> listIntSlots = new List<int>();

            try
            {
                //here we need to add the ordinal(s) number to lock
                listIntSlots.Add(ordinalNumber);
                WebAppoinmentsClientLibrary.Calendars calendars = new WebAppoinmentsClientLibrary.Calendars();
                calendars.LockSlots(calendarId, listIntSlots, 0);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static List<GroupCalendarSlot> GetGroupCalendarSlot(int calendarId)
        {
            List<GroupCalendarSlot> listGroupCalendarSlot = new List<GroupCalendarSlot>();
            bool appointmentsOnly = false;
            string userName = "";
            bool includeVacantTime = false;
            bool includeReservedTime = false;
            string typeSerializer = "";
            try
            {
                WebAppoinmentsClientLibrary.Calendars calendarLibrary = new WebAppoinmentsClientLibrary.Calendars();
                listGroupCalendarSlot = calendarLibrary.GetSlotsGrouped(calendarId, appointmentsOnly, userName, includeVacantTime, includeReservedTime, typeSerializer);
            }
            catch (Exception)
            {
                throw;
            }
            return listGroupCalendarSlot;

        }


        #endregion

        #region Cases

        /// <summary>
        /// This method get thecase y id
        /// </summary>
        /// <param name="caseId"></param>
        /// <returns></returns>
        public static Case GetCaseById(int customerId)
        {
            Case _case = new Case();
            try
            {
                WebAppoinmentsClientLibrary.Cases _casesLibray = new WebAppoinmentsClientLibrary.Cases();
                //_case = _casesLibray.GetCase(caseId).CaseList[0];
                List<Case> listCase = new List<Case>();
                listCase = _casesLibray.GetCase(customerId).CaseList;
                if (listCase.Count == 0) {
                    throw new Exception("Not exists cases for this user");
                }
                int length = listCase.Count-1;
                _case = listCase[length];
                string a = "";

            }
            catch (Exception)
            {

                throw;
            }
            return _case;
        }

        #endregion

        #region Process


        /// <summary>
        /// Thi method is not implemented yet, but this will get the proceees information by id
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public static Process GetProcessById(int processId)
        {
            Process _process = new Process();
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
            return _process;
        }


        #endregion

        #region Customer

        public static int Enqueue(int serviceId, int customerId,
            string notes)
        {
            int result = 0;
            try
            {
                int avatarId = 0;
                int userId = 0;
                string languageCode = "es";
                int receptionPointId = 0;
                string extRef = "";
                string subject = "";
                string listIntClassificationId = "";
                bool printTicket = false;
                int assingToUserId = 0;
                int actOptionId = 0;
                int agentAssignmentType = 0;
                int priorityLevel = 0;
                int suspendDuration = 0;
                int procedureId = 0;
                string typeSerializer = "xml";
                WebAppoinmentsClientLibrary.Customers customerLibrary = new WebAppoinmentsClientLibrary.Customers();
                ResultObjectBase resultObjectBase = customerLibrary.Enqueue(serviceId, receptionPointId, userId, customerId
                , languageCode, subject, notes, extRef, listIntClassificationId, printTicket, assingToUserId, agentAssignmentType,
                procedureId, actOptionId, priorityLevel, suspendDuration, avatarId, typeSerializer);
                if (resultObjectBase.Id > 0)
                {
                    result = resultObjectBase.Id;
                }
                if (resultObjectBase.ReturnCode > 0)
                {
                    throw new Exception("Error: " + resultObjectBase.ReturnException);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }


        /// <summary>
        /// This method save a customer record an returns his id 
        /// </summary>
        /// <param name="personalId"></param>
        /// <param name="customerIdTypeId"></param>
        /// <param name="customerIdTypeName"></param>
        /// <param name="customerLevelId"></param>
        /// <param name="customerLevelName"></param>
        /// <param name="customProperties"></param>
        /// <param name="email"></param>
        /// <param name="firstName"></param>
        /// <param name="isActive"></param>
        /// <param name="isCustomerGroup"></param>
        /// <param name="isMemberOfGroups"></param>
        /// <param name="languageCode"></param>
        /// <param name="lastName"></param>
        /// <param name="name"></param>
        /// <param name="notes"></param>
        /// <param name="pictureAttachmentId"></param>
        /// <param name="sex"></param>
        /// <param name="telNumber1"></param>
        /// <param name="telNumber2"></param>
        /// <param name="userId"></param>
        /// <param name="customerId"></param>
        /// <param name="isSignUp"></param>
        /// <param name="typeSerializer"></param>
        /// <returns></returns>
        public static int SaveCustomer(string personalId,
        //int customerIdTypeId, 
        //string customerIdTypeName,
        //int customerLevelId, 
        //string customerLevelName,
        //string customProperties, 
        string email,
        string firstName,
        //bool isActive,
        //bool isCustomerGroup, 
        //bool isMemberOfGroups,
        //string languageCode, 
        string lastName,
        //string name,
        //string notes, 
        //int pictureAttachmentId,
        int sex,
        string telNumber1,
        //string telNumber2,
        //int userId,
        //bool isSignUp, 
        //string typeSerializer,
        int customerId)
        {
            personalId = firstName;
            int customerIdTypeId = 0;
            int result = 0;
            string customerIdTypeName = "";
            int customerLevelId = 0;
            string customerLevelName = "";
            string customProperties = "";
            bool isActive = true;
            bool isCustomerGroup = false; //It is important to have in false for get results of...
            bool isMemberOfGroups = false;//It is important to have in false for get results of...
            string languageCode = "es";
            string name = firstName;
            string notes = "";
            int pictureAttachmentId = 0;
            sex = 1;
            string telNumber2 = "";
            int userId = 0;
            bool isSignUp = true;
            string typeSerializer = "XML";
            try
            {
                WebAppoinmentsClientLibrary.Customers customerLibrary = new WebAppoinmentsClientLibrary.Customers();
                ResultObjectBase resultObjectBase = customerLibrary.SaveCustomer(
                telNumber1, customerIdTypeId,
                customerIdTypeName,
                customerLevelId, customerLevelName,
                customProperties,
                Utilities.Util.GetEmail(email), firstName,
                isActive, isCustomerGroup,
                isMemberOfGroups,
                languageCode, lastName,
                name, notes, pictureAttachmentId,
                sex, telNumber1,
                telNumber2, userId,
                customerId, isSignUp,
                typeSerializer);
                if (resultObjectBase.ReturnCode > 0)
                {
                    throw new Exception(resultObjectBase.ReturnMessage);
                }
                else
                {
                    result = resultObjectBase.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }


        #endregion

        #region Units


        /// <summary>
        /// This method get a list of currents Units configured
        /// </summary>
        /// <returns></returns>
        public static List<Unit> GetListUnitsConfigured()
        {
            List<Unit> listUnit = new List<Unit>();
            WebAppoinmentsClientLibrary.Units unitLibrary = new WebAppoinmentsClientLibrary.Units();
            try
            {
                listUnit = unitLibrary.GetListUnitsConfigured();
            }
            catch (Exception)
            {
                throw;
            }
            return listUnit;
        }


        #endregion

    }
}