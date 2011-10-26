using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace Screenary
{
    public class URI
    {
        public String user;
        public String password;
        public String host;
        public int host_port;
        public Hashtable parameters;
        public Hashtable headers;


        /*
         *  A uri can be created by using the following arguments or by sending a complete uri as a host parameter
         *  if host is similar to test@ip then we consider its a full uri
         *  otherwise use the whole parameters
         *  
         * for example:
         * URI request_uri = new URI("usre1:password1@192.188.2.2:33;param1=valueparam1;param2=valueparam2?header1=valueheader1");
         * 
         * is similar to:
         *          
         * URI request_uri = new URI("192.188.2.2", 33, user1, password1, paramhashtable, headershashtable);
         * 
         */
        public URI(String host = "", int host_port = 0, String user = "", String password = "", Hashtable parameters = null, Hashtable headers = null)
        {

            //matching a full uri
            String[] m = Regex.Split(host, "(.*)@(.*)");
            if (m.Length > 1)
            {
                this.parseUri(host);
            }
            //otherwise
            else
            {
                this.user = user;
                this.password = password;
                this.host = host;
                this.host_port = host_port;
                this.parameters = (parameters != null) ? parameters : new Hashtable();
                this.headers = (headers != null) ? headers : new Hashtable();
            }
        }


        public void addParameter(String key, String value)
        {
            this.parameters.Add(key, value);
        }

        public void addHeader(String key, String value)
        {
            this.headers.Add(key, value);
        }

        public String parametersToString()
        {
            String ret = "";
            foreach (String key in this.parameters.Keys)
                ret += ";" + key + "=" + this.parameters[key];

            return ret;
        }

        public String headersToString()
        {
            String ret = (this.headers.Count > 0) ? "?" : "";
            foreach (String key in this.headers.Keys)
                ret += key + "=" + this.headers[key] + "&";

            return ret;
        }

        /*
         * will parse a String representation of a uri
         * returns true on success, false otherwise
         */
        public bool parseUri(String uri)
        {
            //initialize with defaults
            this.host = "";
            this.host_port = 0;
            this.parameters =  new Hashtable();
            this.headers =  new Hashtable();

            //parse the string
            String[] m = Regex.Split(uri, "(.*)@(.*)");
            String userinfo = m[1];
            String[] userinfo_m = Regex.Split(userinfo, "(.*):(.*)");
            if (userinfo_m.Length > 1)
            {
                this.user = userinfo_m[1];
                this.password = userinfo_m[2];
            }
            else
            {
                this.user = userinfo_m[0];
                this.password = "";
            }
            String hostinfo = m[2];          
            String[] hostinfo_m = Regex.Split(hostinfo, "([^:|^;|^\\?]+)(:[^;|^\\?]+)?(;[^\\?]+)?(\\?[^\\?]+)?");
    
            
            if (hostinfo_m.Length > 1)
            {
                for (int i = 0; i < hostinfo_m.Length; i++)
                {                   
                    if (hostinfo_m[i] != "")
                    {
                        if (hostinfo_m[i][0].Equals(':'))
                        {
                            this.host_port = Convert.ToInt32(hostinfo_m[i].Replace(":", ""));
                        }
                        else if (hostinfo_m[i][0].Equals(';'))
                        {                            
                            String[] parameters = hostinfo_m[i].Split(';');
                            for (int j = 0; j < parameters.Length; j++)
                            {
                                String[] keyval = parameters[j].Split('=');
                                if(keyval[0] != "")
                                    this.parameters[keyval[0]] = (keyval.Length > 1) ? keyval[1] : "";
                            }
                        }
                        else if (hostinfo_m[i][0].Equals('?'))
                        {
                            String[] headers = hostinfo_m[i].Replace("?", "").Split('&');
                            for (int j = 0; j < headers.Length; j++)
                            {
                                String[] keyval = headers[j].Split('=');
                                this.headers[keyval[0]] = (keyval.Length > 1) ? keyval[1] : "";
                            }
                        }
                        else
                            this.host = hostinfo_m[i];
                    }
                }

            }
           

            return true;
        }

        public override String ToString()
        {            

            String ret = ((this.user.Length > 0) ? this.user : "") + ((this.password.Length > 0) ? ":" + this.password : "")
                         + ((this.user.Length > 0 && this.password.Length > 0)? "@" : "")
                         + this.host + ((this.host_port > 0) ? ":" + this.host_port : "")
                         + this.parametersToString() + this.headersToString();

            return ret;
        }

    }
}
