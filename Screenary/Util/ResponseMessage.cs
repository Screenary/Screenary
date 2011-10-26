using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Screenary
{
    class ResponseMessage : Message
    {        
        
        public int status_code { get; set; }
        public String reason_phrase { get; set; }

        //constants for status codes
        public static int TRYING = 100;
	    public static int RINGING = 180;
	    public static int CALL_BEING_FORWARDED = 181;
	    public static int CALL_QUEUED = 182;
	    public static int SESSION_PROGRESS = 183;
	    public static int OK = 200;
	    public static int ACCEPTED = 202;
	    public static int MULTIPLE_CHOICES = 300;
	    public static int MOVED_PERMANENTLY = 301;
	    public static int MOVED_TEMPORARILY = 302;
	    public static int USE_PROXY = 305;
	    public static int ALTERNATIVE_SERVICE = 380;
	    public static int BAD_REQUEST = 400;
	    public static int UNAUTHORIZED = 401;
	    public static int PAYMENT_REQUIRED = 402;
	    public static int FORBIDDEN = 403;
	    public static int NOT_FOUND = 404;
	    public static int METHOD_NOT_ALLOWED = 405;
	    public static int NOT_ACCEPTABLE = 406;
	    public static int PROXY_AUTHENTICATION_REQUIRED = 407;
	    public static int REQUEST_TIMEOUT = 408;
	    public static int GONE = 410;
	    public static int REQUEST_ENTITY_TOO_LARGE = 413;
	    public static int REQUEST_URI_TOO_LONG = 414;
	    public static int UNSUPPORTED_MEDIA_TYPE = 415;
	    public static int UNSUPPORTED_URI_SCHEME = 416;
	    public static int BAD_EXTENSION = 420;
	    public static int EXTENSION_REQUIRED = 421;
	    public static int INTERVAL_TOO_BRIEF = 423;
	    public static int TEMPORARLY_UNAVAILABLE = 480;
	    public static int CALL_LEG_DONE = 481;
	    public static int LOOP_DETECTED = 482;
	    public static int TOO_MANY_HOPS = 483;
	    public static int ADDRESS_INCOMPLETE = 484;
	    public static int AMBIGUOUS = 485;
	    public static int BUSY_HERE = 486;
	    public static int REQUEST_TERMINATED = 487;
	    public static int NOT_ACCEPTABLE_HERE = 488;
	    public static int REQUEST_PENDING = 491;
	    public static int UNDECIPHERABLE = 493;
	    public static int SERVER_INTERNAL_ERROR = 500;
	    public static int NOT_IMPLEMENTED = 501;
	    public static int BAD_GATEWAY = 502;
	    public static int SERVICE_UNAVAILABLE = 503;
	    public static int SERVER_TIMEOUT = 504;
	    public static int VERSION_NOT_SUPPORTED = 505;
	    public static int MESSAGE_TOO_LARGE = 513;
	    public static int BUSY_EVERYWHERE = 600;
	    public static int DECLINE = 603;
	    public static int DOES_NOT_EXIT_ANYWHERE = 604;
	    public static int NOT_ACCEPTABLE_ANYWHERE = 606;


        public ResponseMessage(MessageHeader header, int status_code, String messageBody="", String reason_phrase = "")
            : base(header, messageBody)
        {
            this.status_code = status_code;
            this.reason_phrase = reason_phrase;
        }

        public ResponseMessage(String msg)
        {
            this.header = new MessageHeader();
            this.parseMessage(msg);
        }

        public override String ToString()
        {
            String rt = "";
            rt += this.status_code + " " + ((this.reason_phrase != "") ? this.reason_phrase : "") + this.delimiter + base.ToString();

            return rt;
        }

        public void parseMessage(String msg)
        {
            String[] msglines = Regex.Split(msg, this.delimiter);

            String response_line = msglines[0];
            String[] response_lines = Regex.Split(response_line, "([^\\s]+)\\s?(.*)?");            

            this.status_code = Convert.ToInt32(response_lines[1]);
            this.reason_phrase = response_lines[2];

            this.parseHeader(msglines);     

        }

    }
}
