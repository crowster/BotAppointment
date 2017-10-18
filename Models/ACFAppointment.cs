using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppicationBot.Ver._2.Models
{
    /// <summary>
    /// This class have the porperties of an appoinment
    /// </summary>
    [Serializable]
    public class ACFAppointment
    {
        #region Properties


        private int _AppoinmentId;

        public int AppoinmentId
        {
            get { return _AppoinmentId; }
            set { _AppoinmentId = value; }
        }

        private int _ProcessId;

        public int ProcessId
        {
            get { return _ProcessId; }
            set { _ProcessId = value; }
        }

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private DateTime _StarDate;

        public DateTime StartDate
        {
            get { return _StarDate; }
            set { _StarDate = value; }
        }

        private string _Date;

        public string Date
        {
            get { return _Date; }
            set { _Date = value; }
        }


        private int _ServiceId;

        public int ServiceId
        {
            get { return _ServiceId; }
            set { _ServiceId = value; }
        }
        private string _ServiceName;

        public string ServiceName
        {
            get { return _ServiceName; }
            set { _ServiceName = value; }
        }
   


        #endregion
    }
}