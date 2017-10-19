using AppicationBot.Ver._2.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace AppicationBot.Ver._2.Utilities
{
    public class Util
    {
        /// <summary>
        /// This method descompose the object id in name,last name and phone number
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public static ObjectIdModel DescomposeObjectId(string objectId)
        {
            ObjectIdModel result = new ObjectIdModel();
            Char delimiter = '_';
            String[] beforeDot = objectId.Split(delimiter);
            result.Name = beforeDot[0]; //Name
            result.LastName = beforeDot[1]; //Last name
            result.PhoneNumber = beforeDot[2]; //Phone number


            return result;
        }

        public static string GetOrdinalNumberFromBotOption(string botOption)
        {
            string result = "";
            Char delimiter = '.';
            String[] beforeDot = botOption.Split(delimiter);
            result = beforeDot[0];
            return result;
        }
        public static string GetCalendarIdFromBotOption(string botOption)
        {
            string result = "";
            Char delimiter = '.';
            String[] beforeDot = botOption.Split(delimiter);
            result = beforeDot[0];
            return result;
        }

        public static string GetDateFromBotOption(string botOption)
        {
            string result = "";
            Char delimiter = '.';
            String[] beforeDot = botOption.Split(delimiter);
            result = beforeDot[1];
            result.Replace("-","");
            return result;
        }

        public static int GetUnitIdFromBotOption(string botOption)
        {
            int result = 0;
            Char delimiter = '.';
            String[] beforeDot = botOption.Split(delimiter);
            result = Convert.ToInt32(beforeDot[0]);
            return result;
        }
        public static string GetAppoitmentIdFromBotOption(string botOption)
        {
            string result = "";
            string firstSplit = "";

            Char delimiter = '.';
            Char twoPointsDelimiter = ':';

            String[] beforeDot = botOption.Split(delimiter);
            firstSplit = beforeDot[0];
            String[] secondBotoArray = firstSplit.Split(twoPointsDelimiter);
            //This index have the appointment id

            result = secondBotoArray[1];
            return result;
        }
        public static string GetHourFromStartDate(string startDate)
        {
            string result = "";
            Char delimiter = ':';
            String[] beforeDot = startDate.Split(delimiter);
            result = beforeDot[0];
            return result;
        }
        public static Models.ACFAppointment GetAppointment(string optionBot)
        {
            Models.ACFAppointment appointment = new Models.ACFAppointment();
            try
            {
                Char delimiterPunto = '.';
                Char delimiterComa = ',';
                Char delimiterGuion = '-';
                Char delimiterTwoPoints = ':';
                //Get appointment id
                String[] arrayAppointment = optionBot.Split(delimiterPunto);
                string appointmentId = arrayAppointment[0];
                appointment.AppoinmentId = Convert.ToInt32(arrayAppointment[0]);
                String[] appointmentandservicename = optionBot.Split(delimiterComa);
                //Get service name

                string appointmentandservicenameString = appointmentandservicename[0];
                String[] onlyService = appointmentandservicenameString.Split(delimiterGuion);
                appointment.ServiceName = onlyService[1];

                //GEt Service id
                appointment.ServiceId = Convert.ToInt32(appointmentandservicename[1]);


                //Get process id
                string ProcessId = appointmentandservicename[2];

                String[] processIds = ProcessId.Split(delimiterTwoPoints);
                appointment.ProcessId = Convert.ToInt32(processIds[1]);

                //Get the date time
                appointment.Date = appointmentandservicename[3];
            }
            catch (Exception)
            {

                throw;
            }


            return appointment;
        }

        /// <summary>
        /// This method convert an image to array bytes
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public static byte[] imageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
        /// <summary>
        /// This method generate an object Id
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public static string generateObjectId(string name, string lastName, string phoneNumber)
        {
            StringBuilder objectId = new StringBuilder();
            Random rand = new Random(DateTime.Now.Millisecond);
            int RandomNumber = rand.Next(1, 9);
            int RandomNumber2 = rand.Next(1, 9);
            //Here Im concatenating the idObject
            objectId.Append(name);
            objectId.Append("_");
            objectId.Append(lastName);
            objectId.Append("_");
            objectId.Append(phoneNumber);
            return objectId.ToString();
        }
        /// <summary>
        /// This method generate an image Id
        /// </summary>
        /// <param name="attachmentName"></param>
        /// <returns></returns>
        public static string generateImageId(string _attachmentName)
        {
            string attachmentName = Path.GetFileNameWithoutExtension(_attachmentName);
            StringBuilder imageId = new StringBuilder();
            //The DateTime parameter * 18 can be any other
            Random rand = new Random(DateTime.Now.Millisecond * 18);
            int RandomNumber = rand.Next(1, 9);
            int RandomNumber2 = rand.Next(1, 9);
            //Here Im concatenating the imageId
            imageId.Append(attachmentName + "-");
            imageId.Append(RandomNumber2);
            imageId.Append(RandomNumber);

            return imageId.ToString();
        }
        /// <summary>
        /// This method get the correct email from skype
        /// </summary>
        /// <param name="_email"></param>
        /// <returns></returns>
        public static string GetEmail(string _email)
        {
            string email = _email;
            try
            {
                if (_email.ToLower().Contains("href"))
                {
                    string[] StepOne = _email.Split(new string[] { "mailto" }, StringSplitOptions.None);
                    string[] StepTwo = StepOne[1].Split(new string[] { "\"" }, StringSplitOptions.None);
                    string value = StepTwo[0].Replace("\"", "");
                    value = StepTwo[0].Replace("\\", "");
                    value = StepTwo[0].Replace(":", "");
                    email = value;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return email;

        }
    }
}