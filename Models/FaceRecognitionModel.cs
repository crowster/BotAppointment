using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppicationBot.Ver._2.Models
{
    public class FaceRecognitionModel
    {

        private string _ObjectId;

        public string ObjectId
        {
            get { return _ObjectId; }
            set { _ObjectId = value; }
        }

        private int _CustomerId;

        private string _PhotoId;

        public string PhotoId
        {
            get { return _PhotoId; }
            set { _PhotoId = value; }
        }

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _FileName;

        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }
    }
}