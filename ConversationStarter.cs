using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppicationBot.Ver._2
{
    public class ConversationStarter
    {
        #region Properties
        private string _ToId;
        public string ToId
        {
            get { return _ToId; }
            set { _ToId = value; }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _FromId;
        public string FromId
        {
            get { return _FromId; }
            set { _FromId = value; }
        }

        private string _FromName;
        public string FromName
        {
            get { return _FromName; }
            set { _FromName = value; }
        }

        private string _ServiceUrl;
        public string ServiceUrl
        {
            get { return _ServiceUrl; }
            set { _ServiceUrl = value; }
        }

        private string _ChannelId;
        public string ChannelId
        {
            get { return _ChannelId; }
            set { _ChannelId = value; }
        }

        private string  _ConversationId;
        public string  ConversationId
        {
            get { return _ConversationId; }
            set { _ConversationId = value; }
        }




        #endregion
    }
}