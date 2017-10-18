using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppicationBot.Ver._2.Models
{
    public class ObjectIdModel
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        private string _LastName;

        public string LastName
        {
            get { return _LastName; }
            set { _LastName = value; }
        }

        private string _PhoneNumber;

        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set { _PhoneNumber = value; }
        }

    }
}