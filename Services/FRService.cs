using Face_RecognitionLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AppicationBot.Ver._2.Services
{
    public class FRService
    {
        private string urlSaveImage;
        string urlRecognitionGroup;
        string _BaseUrl;
        string urlEnqueue;

        public FRService()
        {
            this._BaseUrl = "http://migueliis.hosted.acftechnologies.com/RestServiceFRBotAppointment";
            this.urlSaveImage = _BaseUrl + "/api/FaceRecognition/SaveImage";
            this.urlRecognitionGroup = _BaseUrl + "/api/FaceRecognition/RecognitionService?IdPhoto=";
            this.urlEnqueue = _BaseUrl + "/api/FaceRecognition/Enqueue?objectId=";
        }

        /// <summary>
        /// This method save the image in the database in the table ACF_FaceImage
        /// </summary>
        /// <param name="image">The attachement in the bot </param>
        public async Task<int> saveImage(byte[] image)
        {
            int imageID = 0;
            try
            {
                //is necesary to first save image
                string _ResultService = "";
                int _IdSaveImage = 0;
                string imageinJson = JsonConvert.SerializeObject(image);
                _ResultService = await WRequest(this.urlSaveImage, "post", imageinJson);
                _IdSaveImage = Convert.ToInt32(_ResultService);
                imageID = _IdSaveImage;
            }
            catch (Exception)
            {
                throw;
            }
            return imageID;
        }
        /// <summary>
        /// This method validate the image through the api with Face Recognition
        /// </summary>
        /// <param name="idSavedImage"></param>
        /// <returns></returns>
        public async Task<string> validateFaceRecognition(int idSavedImage)
        {
            string objectId;
            bool validation = false;
            string _ResultService = "";
            ObjectResultRecognition resultRecognition = new ObjectResultRecognition();
            try
            {
                //using to recognition
                _ResultService = await WRequest(urlRecognitionGroup + idSavedImage, "get", "");
                resultRecognition = JsonConvert.DeserializeObject<ObjectResultRecognition>(_ResultService);
                if (resultRecognition.Success)
                    objectId = resultRecognition.ObjectId;
                else { objectId = "not found"; }
            }
            catch (Exception)
            {
                throw;
            }
            return objectId;
        }

        public async Task<int> enqueueCustomer(string objectId, string idPhoto,string name, string fileName)
        {
            bool validation = false;
            int caseId;
            string _ResultService = "";
            ObjectResultRecognition resultRecognition = new ObjectResultRecognition();
            try
            {
                StringBuilder url = new StringBuilder();
                url.Append(urlEnqueue+objectId);
                url.Append("&idPhoto="+idPhoto);
                url.Append("&name=" + name);
                url.Append("&fileName=" + fileName);

                //using to recognition
                _ResultService = await WRequest(url.ToString(), "get", ""); 
                caseId = Convert.ToInt32(_ResultService);
            }
            catch (Exception)
            {
                throw;
            }
            return caseId;
        }



        /// <summary>
        /// This method upload an image to the group o Face Recognition, in this case TestNavy
        /// </summary>
        /// <param name="name">Name of  the user</param>
        /// <param name="lastName"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="imageName"></param>
        /// <param name="idSavedImage"></param>
        /// <returns></returns>
        public async Task<bool> uploadImage(string name, string lastName, string phoneNumber, string imageName,int idSavedImage)
        {
            bool uploaded = false;
            string _ResultService = "";
            ObjectResultRecognition resultRecognition = new ObjectResultRecognition();
            try
            {
                //using to upload image
                //string _ObjectId = "FirstName_LastName_PhoneNumber";
                string _ObjectId = Utilities.Util.generateObjectId(name,lastName,phoneNumber);
                string _ImageId = Utilities.Util.generateImageId(imageName);
                //string _ImageId = "mayra-3";
                string urlUploadImage = _BaseUrl + "/api/FaceRecognition/UploadImage?IdPhoto="
                                                  + idSavedImage.ToString() + "&ObjectId=" + _ObjectId + "&ImageId=" + _ImageId;
                _ResultService = await WRequest(urlUploadImage, "get", "");
                if (resultRecognition.Success)
                    uploaded = true;
                else { uploaded = false; }
            }
            catch (Exception)
            {
                throw;
            }
            return uploaded;
        }

        //This method create a request to RestServices of Face Recognition
        public static async Task<string> WRequest(string URL, string method, string data)
        {
            string str = "";
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    if (method.ToLower() == "post")
                    {

                        var content = new StringContent(data, Encoding.UTF8, "application/json");

                        response = await httpClient.PostAsync(URL, content);
                    }
                    if (method.ToLower() == "get")
                    {
                        response = await httpClient.GetAsync(URL).ConfigureAwait(false);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        var xml = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        str = xml;
                    }
                }
            }
            catch (System.Exception exception1)
            {
                System.Exception exception = exception1;
                throw new System.Exception("An error has occurred: " + exception.Message);
            }
            return str;
        }


    }
}
