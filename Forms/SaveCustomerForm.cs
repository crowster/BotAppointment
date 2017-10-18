using AppicationBot.Ver._2.Models;
using AppicationBot.Ver._2.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using OTempus.Library.Class;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace AppicationBot.Ver._2.Forms
{
    public enum Sexs {
        Unknown = 0, Male = 1, Female = 2
    }
    [Serializable]
    public class SaveCustomerForm
    {
        #region Methods
        /// <summary>
        /// This method get the int value of each sex specified
        /// </summary>
        /// <param name="sexInput"></param>
        /// <returns></returns>
        public static int GetIntSex(string sexInput)
        {
            int sex = 0;
            switch (sexInput)
            {
                case "Unknown":
                    sex = 0;
                    break;
                case "Male":
                    sex = 1;
                    break;
                case "Female":
                    sex = 2;
                    break;
            }
            return sex;
        }
        #endregion
        #region Properties
        [Prompt("Can you enter the user name, please? {||} ")]
        public string FirstName;
        public string customerId;
        [Prompt("Can you enter the {&}, please? {||} ")]
        public string LastName;
        [Prompt("Can you enter the {&}, please? {||} ")]
        public string Email;
        [Prompt("Can you enter the {&}, please? {||} ")]
        // [Pattern(@"(\(\d{3}\))?\s*\d{3}(-|\s*)\d{4}")]
        public string PhoneNumber;
        [Prompt("Can you enter your {&}, please? {||} ")]
        public Sexs? Sex;
        public static IDialogContext context { get; set; }
        public static int savedImageId { get; set; }

        public static string imageName{ get; set; }


        #endregion
        #region Creation of IForm
        /// <summary>
        /// This method create IForm for save the customer information
        /// </summary>
        /// <returns></returns>
        public static IForm<SaveCustomerForm> BuildForm()
        {
            OnCompletionAsyncDelegate<SaveCustomerForm> processOrder = async (context, state) =>
            {
                ACFCustomer customer = new ACFCustomer();
                try
                {
                    customer.CustomerId = 0;//It is setted to 0, beacuse we will create new user, in other hand we can pass the id for update the record
                    customer.FirstName = state.FirstName;
                    customer.LastName = state.LastName;
                    customer.Email = state.Email;
                    customer.PhoneNumber = state.PhoneNumber;
                    customer.Sex = GetIntSex(state.Sex.ToString());
                    int customerId = AppoinmentService.SaveCustomer(customer.PhoneNumber, customer.Email, customer.FirstName,
                    customer.LastName, customer.Sex, customer.PhoneNumber, customer.CustomerId);
                    state.customerId = customerId.ToString();
                    if (customerId > 0) {
                        FRService frService = new FRService();
                        //Get the idImage Saved
                        int idImageSaved = 0;
                        if (!context.UserData.TryGetValue<int>("idImageSaveId", out idImageSaved)) { idImageSaved = 0; }

                        //Set the FaceRecognition Model, 
                        FaceRecognitionModel faceRecognitionModel = new FaceRecognitionModel();
                        faceRecognitionModel.ObjectId = Utilities.Util.generateObjectId(customer.FirstName,customer.LastName,customer.PhoneNumber);
                        faceRecognitionModel.PhotoId = savedImageId.ToString();
                        faceRecognitionModel.Name = imageName;
                        faceRecognitionModel.FileName = imageName;
                        context.UserData.SetValue<FaceRecognitionModel>("FaceRecognitionModel", faceRecognitionModel); 

                        bool uploadImage= await frService.uploadImage(customer.FirstName,customer.LastName,customer.PhoneNumber
                            ,imageName,savedImageId);
                        await context.PostAsync("Your user has been saved succesfully with the id: " + customerId);
                        /* Create the userstate */
                        //Instance of the object ACFCustomer for keept the customer
                        ACFCustomer customerState = new ACFCustomer();
                        customerState.CustomerId = customerId;
                        customerState.FirstName = customer.FirstName;
                        customerState.PhoneNumber = customer.PhoneNumber;
                        customerState.Sex = Convert.ToInt32(customer.Sex);
                        customerState.PersonaId = customer.PhoneNumber;
                        context.UserData.SetValue<ACFCustomer>("customerState", customerState);
                        context.UserData.SetValue<bool>("SignIn", true);
                    }
                }
                catch (Exception ex)
                {
                    await context.PostAsync("We have an error: "+ex.Message);
                }
            };
            CultureInfo ci = new CultureInfo("en");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            var culture = Thread.CurrentThread.CurrentUICulture;
            var form = new FormBuilder<SaveCustomerForm>();
            var yesTerms = form.Configuration.Yes.ToList();
            var noTerms = form.Configuration.No.ToList();
            yesTerms.Add("Yes");
            noTerms.Add("No");
            form.Configuration.Yes = yesTerms.ToArray();
            return form.Message("Fill the next information, please")
                       .Field(nameof(FirstName))
                       .Field(nameof(LastName))
                       .Field(nameof(Email))
                       .Field(nameof(PhoneNumber))
                       .Field(nameof(Sex))
                       .Field(new FieldReflector<SaveCustomerForm>(nameof(customerId)).SetActive(inActiveField))
                       .Confirm("Are you selected the information: " +
                      "\n* Name: {FirstName} " +
                      "\n* Last name:  {LastName} " +
                      "\n* Email:  {Email} " +
                      "\n* Phone Number: {PhoneNumber} " +
                      "\n* Sex:  {Sex}? \n" +
                      "(yes/no)")
                     .AddRemainingFields()
                     .Message("The process for save your user has been started!")
                     .OnCompletion(processOrder)
                     .Build();
        }
        /// <summary>
        /// This method disabled an specific field
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private static bool inActiveField(SaveCustomerForm state)
        {
            return false;
        }
    };
    #endregion
}
