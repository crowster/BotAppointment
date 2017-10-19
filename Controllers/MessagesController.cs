using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using AppicationBot.Ver._2.Dialogs;
using AppicationBot.Ver._2.Services;
using AppicationBot.Ver._2.Models;
using System.Text;
using AppicationBot.Ver._2.Forms;
using Microsoft.Bot.Builder.FormFlow;
using System.Collections.Generic;
using Microsoft.Rest;
using Face_RecognitionLibrary;
using System.IO;
using OTempus.Library.Class;
using System.Net.Http.Headers;

namespace AppicationBot.Ver._2
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private static string serviceUrl;
        bool signIn;

        /*public async Task<Message> Post([FromBody]Message message)
        {
            Task.Delay(2000).ContinueWith(async (t) =>
            {
                var client = message.From.ChannelId == "emulator" ? new ConnectorClient(new Uri("http://localhost:9000"), new ConnectorClientCredentials()) : new ConnectorClient();
                var clearMsg = message.CreateReplyMessage();
                clearMsg.Text = $"Reseting everything for conversation: {message.ConversationId}";
                clearMsg.BotUserData = new object { };
                clearMsg.BotConversationData = new object { };
                clearMsg.BotPerUserInConversationData = new object { };
                await client.Messages.SendMessageAsync(clearMsg);

            });

            return await Conversation.SendAsync(message, () => new EchoDialog());
        }*/

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            ObjectResultRecognition result = new ObjectResultRecognition();
            string _BaseUrl = "http://migueliis.hosted.acftechnologies.com/RestServiceFRBotAppointment";

            //Creation of state client
            StateClient stateClient = activity.GetStateClient();
            string objectId = string.Empty;
            int idImageSaved = 0;
            string name = "default";
            byte[] fileNormal = null;
            int customerId = 0;
            //Instance of the object ACFCustomer for keept the customer
            Models.ACFCustomer customerState = new Models.ACFCustomer();

            BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

            /*await Task.Delay(2000).ContinueWith(async (t) =>
             {
                //Reset the custom properties
                userData.SetProperty<bool>("SignIn", false);
                 userData.SetProperty<int>("idImageSaveId", idImageSaved);
                 userData.SetProperty<ACFCustomer>("customerState", new ACFCustomer());
                 userData.SetProperty<FaceRecognitionModel>("FaceRecognitionModel", new FaceRecognitionModel());
                 userData.SetProperty<MyCustomType>("UserData", new MyCustomType());
                 objectId = "";
                 var client = new ConnectorClient(new Uri(activity.ServiceUrl), new MicrosoftAppCredentials());
                 var reply = activity.CreateReply();
                 reply.Text = $"Your session has been restarted...";
                 await client.Conversations.ReplyToActivityAsync(reply);

             });*/


            if (activity.Type == ActivityTypes.Message)
            {
                var message = " ";
                var client = new ConnectorClient(new Uri(activity.ServiceUrl), new MicrosoftAppCredentials());
                var reply = activity.CreateReply();
                try
                {
                    message = activity.Text;
                    if (message.ToLower().Contains("exit"))
                    {
                        //Reset the custom properties
                        userData.SetProperty<bool>("SignIn", false);
                        userData.SetProperty<int>("idImageSaveId", idImageSaved);
                        userData.SetProperty<ACFCustomer>("customerState", new ACFCustomer());
                        userData.SetProperty<FaceRecognitionModel>("FaceRecognitionModel", new FaceRecognitionModel());
                        userData.SetProperty<MyCustomType>("UserData", new MyCustomType());
                        objectId = "";
                        reply.Text = $"Your session has been restarted...";
                        await client.Conversations.ReplyToActivityAsync(reply);
                        activity.GetStateClient().BotState.DeleteStateForUser(activity.ChannelId, activity.From.Id);
                    }
                    if (message.ToLower().Contains("new user"))
                    {

                    }
                }
                catch (Exception)
                {
                }
                if (activity.Attachments.Count > 0)
                {
                    Activity activityRes = new Activity();
                    activityRes = activity;
                    //activity.GetStateClient().BotState.DeleteStateForUser(activity.ChannelId, activity.From.Id);

                    reply.Text = $"Please wait a moment... ";
                    await client.Conversations.ReplyToActivityAsync(reply);
                    //TEst when atachments a new picture, reset the custom properties
                    //Reset he custom properties
                    userData.SetProperty<bool>("SignIn", false);
                    userData.SetProperty<int>("idImageSaveId", idImageSaved);
                    userData.SetProperty<ACFCustomer>("customerState", new ACFCustomer());
                    userData.SetProperty<FaceRecognitionModel>("FaceRecognitionModel", new FaceRecognitionModel());
                    userData.SetProperty<MyCustomType>("UserData", new MyCustomType());
                    objectId = "";
                    //activity.GetStateClient().BotState.DeleteStateForUser(activity.ChannelId, activity.From.Id);
                    //reply.Text = $"Data reseted... ";
                    IEnumerable<byte[]> array = null;
                    List<byte[]> listArrayBytes = null;
                    //The method GetAttachmentsAsync is the encharged for obtain the array bytes, this pass a jwt to the Content url, for get the image
                    try
                    {
                        array = await GetAttachmentsAsync(activityRes);
                        listArrayBytes = array.ToList();
                        name = activityRes.Attachments[0].Name;

                        // byte[] arr = await LoadAttachmentAsBytes(activity.Attachments[0], activity.Attachments[0].ContentUrl);

                        //Get the actual attachment inside the bot 
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        if (listArrayBytes == null)
                        {
                            //Instance of web Client

                            WebClient webClient = new WebClient();

                            //Get the name of the attachment

                            name = activityRes.Attachments[0].Name;

                            //Get the url where the file is saved for the bot framework

                            var url = activityRes.Attachments[0].ContentUrl;

                            //Get the array bytes from an url, this url is generated for the botFramework, we can recovery a file withuot extensions

                            fileNormal = webClient.DownloadData(activityRes.Attachments[0].ContentUrl);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    byte[] file = null;
                    if (fileNormal != null)
                        file = fileNormal;
                    else
                    {
                        file = listArrayBytes[0];
                    }

                    //Creation of a stream through the array bytes
                    Stream stream = new MemoryStream(file);
                    //url to conect api
                    //make an instance of FRService , this instance already has setted the URL base, urlRecognitionGroup and  urlSaveImage 
                    FRService frService = new FRService();

                    //First save image
                    try
                    {
                        //idImageSaved is the id in the database , when attach an image , is saved in the database too . 
                        idImageSaved = await frService.saveImage(file);
                        userData.SetProperty<int>("idImageSaveId", idImageSaved);
                    }
                    catch (Exception ex)
                    {
                       
                        reply.Text = $"Error  " + ex.Message.ToString() + "...";
                        await client.Conversations.ReplyToActivityAsync(reply);
                    }

                    objectId = await frService.validateFaceRecognition(idImageSaved);
                    if (!objectId.Contains("not found"))
                    {
                        //Divide the object Id in: name, last name and phone number
                        ObjectIdModel objectIdModel = Utilities.Util.DescomposeObjectId(objectId);
                        userData.SetProperty<bool>("SignIn", true);
                        //Set the FaceRecognition Model, 
                        FaceRecognitionModel faceRecognitionModel = new FaceRecognitionModel();
                        faceRecognitionModel.ObjectId = objectId;
                        faceRecognitionModel.PhotoId = idImageSaved.ToString();
                        faceRecognitionModel.Name = name;
                        faceRecognitionModel.FileName = name;
                        userData.SetProperty<FaceRecognitionModel>("FaceRecognitionModel", faceRecognitionModel);

                        //Get the personal id in this case is the phone number
                        string personalId = objectIdModel.PhoneNumber;

                        //Get the user Data
                        WebAppoinmentsClientLibrary.Customers customerLibrary = new WebAppoinmentsClientLibrary.Customers();
                        Customer customer = new Customer();
                        try
                        {
                            customer = customerLibrary.GetCustomerByPersonalId(personalId, 0).Customer;

                        }
                        catch (Exception ex)
                        {
                            reply.Text = $"Error:  "+ ex.Message.ToString();
                            await client.Conversations.ReplyToActivityAsync(reply);
                        }

                        /* Create the userstate */

                        try
                        {customerId= customer.Id;}catch (Exception){}
                        customerState.CustomerId = customer.Id;
                        customerState.FirstName = customer.FirstName;
                        customerState.PhoneNumber = customer.TelNumber1;
                        customerState.Sex = Convert.ToInt32(customer.Sex);
                        customerState.PersonaId = customer.PersonalId;
                        userData.SetProperty<ACFCustomer>("customerState", customerState);
                        if (!string.IsNullOrEmpty(customer.FirstName)) { 
                        reply.Text = $"Welcome  " + customer.FirstName + " "+customer.LastName+"...";
                            await client.Conversations.ReplyToActivityAsync(reply);
                        }

                    }
                    else
                    {
                        /* var client = new ConnectorClient(new Uri(activity.ServiceUrl), new MicrosoftAppCredentials());
                         var reply = activity.CreateReply();
                         reply.Text = $"Your photo is not registered yet, Starting the process for create an user with this photo ";
                         await client.Conversations.ReplyToActivityAsync(reply);*/
                    }
                }

                MyCustomType myCustomData = new MyCustomType();
                myCustomData.Id = 2;
                try
                {
                    //GEt the actual id ImageSaveId
                    int idImageSaveIdProperty = 0;
                    idImageSaveIdProperty = userData.GetProperty<int>("idImageSaveId");
                    userData.SetProperty<MyCustomType>("UserData", myCustomData);
                    await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                    ACFCustomer currentACFCustomer = new ACFCustomer();
                    string customerName = "";
                    string firstName = "";
                    try
                    {
                        firstName = userData.GetProperty<ACFCustomer>("customerState").FirstName;
                    }
                    catch (Exception)
                    {

                    }

                    if (!string.IsNullOrEmpty(firstName))
                    {
                        currentACFCustomer = userData.GetProperty<ACFCustomer>("customerState");

                        customerName = currentACFCustomer.FirstName;
                    }
                    bool session = userData.GetProperty<bool>("SignIn");

                    if ((objectId.Contains("not found") || String.IsNullOrEmpty(objectId)|| customerId == 0) && idImageSaveIdProperty > 0 && String.IsNullOrEmpty(customerName) )
                    {
                        await Conversation.SendAsync(activity, () => new RegisterUserDialog(idImageSaved, name));

                    }
                    else if (!session && String.IsNullOrEmpty(objectId) && !objectId.ToLower().Contains("not found") && idImageSaveIdProperty == 0)
                    {
                        reply.Text = $"Please add your photo to identify yourself.. ";
                        await client.Conversations.ReplyToActivityAsync(reply);
                    }
                    else if (session)
                    {
                        if (!objectId.Contains("not found") && !String.IsNullOrEmpty(customerName) )
                        {
                            await Conversation.SendAsync(activity, () => new RootDialog());
                        }
                    }
                    /* else
                     {
                         var client = new ConnectorClient(new Uri(activity.ServiceUrl), new MicrosoftAppCredentials());
                         var reply = activity.CreateReply();
                         reply.Text = $"please add your photo, for sign in";
                         await client.Conversations.ReplyToActivityAsync(reply);
                     }*/
                }
                catch (HttpOperationException err)
                {
                    // handle error with HTTP status code 412 Precondition Failed
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<IEnumerable<byte[]>> GetAttachmentsAsync(Activity activity)
        {
            var attachments = activity?.Attachments?
           .Where(attachment => attachment.ContentUrl != null)
           .Select(c => Tuple.Create(c.ContentType, c.ContentUrl));
            if (attachments != null && attachments.Any())
            {
                var contentBytes = new List<byte[]>();
                using (var connectorClient = new ConnectorClient(new Uri(activity.ServiceUrl)))
                {
                    var token = await (connectorClient.Credentials as MicrosoftAppCredentials).GetTokenAsync();
                    foreach (var content in attachments)
                    {
                        var uri = new Uri(content.Item2);
                        using (var httpClient = new HttpClient())
                        {
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);//Bearer
                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                            contentBytes.Add(await httpClient.GetByteArrayAsync(uri));
                        }
                    }
                }
                return contentBytes;
            }
            return null;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels

                IConversationUpdateActivity update = message;
                var client = new ConnectorClient(new Uri(message.ServiceUrl), new MicrosoftAppCredentials());
                if (update.MembersAdded != null && update.MembersAdded.Any())
                {
                    foreach (var newMember in update.MembersAdded)
                    {
                        if (newMember.Id != message.Recipient.Id)
                        {
                            var reply = message.CreateReply();
                            reply.Text = $"Welcome {newMember.Name}!...  This appointmentBot will help you to manage your appointment,  How can I help You?";
                            client.Conversations.ReplyToActivityAsync(reply);
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
            return null;
        }
    }
}