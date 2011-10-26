using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Screenary
{
    class RequestMessage : Message
    {
        /*
         * method can be:
         * INVITE
         * ACK
         * REGISTER
         * BROADCAST
         * ...
         * 
         */

        protected String method { get; set; }
        protected URI request_uri { get; set; }

        public RequestMessage(MessageHeader header, String messageBody = "", String method = "", URI request_uri = null)
            : base(header, messageBody)
        {
            this.method = method;
            this.request_uri = request_uri;
        }

        public RequestMessage(String msg)
        {
            this.header = new MessageHeader();
            this.parseMessage(msg);
        }

        public override String ToString()
        {
            String rt = "";
            rt += this.method + " " + ((this.request_uri != null) ? this.request_uri.ToString() : "") + this.delimiter +base.ToString();

            return rt;
        }


        /*
         * Parses and loads a message from string
         * return true on success, false otherwise
         */ 

        public void parseMessage(String msg)
        {
            String[] msglines = Regex.Split(msg, this.delimiter);

            String request_line = msglines[0];
            String[] request_fields = request_line.Split(' ');

            if (request_fields.Length < 2)
                throw new System.ArgumentException("Fields missing", "Request Line");

            this.method = request_fields[0];
            this.request_uri = new URI(request_fields[1]);

            this.parseHeader(msglines);                       
            
        }
        

    }
}
