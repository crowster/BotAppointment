using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppicationBot.Ver._2.Models
{
    public class ACFCustomer
    {
        private int _CustomerId;

        public int CustomerId
        {
            get { return _CustomerId; }
            set { _CustomerId = value; }
        }
        private string _FirstName;

        public string FirstName
        {
            get { return _FirstName; }
            set { _FirstName = value; }
        }
        private string _MiddleName;

       
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

        private string _Email;

        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }
        private int _Sex;

        public int Sex
        {
            get { return _Sex; }
            set { _Sex = value; }
        }

        private string _PersonalId;

        public string PersonaId
        {
            get { return _PersonalId; }
            set { _PersonalId = value; }
        }



    }
}