using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Screenary
{
    public class MessageHeader
    {
        /* configurable delimiter between lines for the message header's parameters (CRLF) */
        protected String delimiter = "\r\n";
        /* Indicates character encoding */
        public String character_encoding { get; set; }
        /* content type */
        public String content_type { get; set; }
        /* content length */
        public int content_length { get; set; }
        /* Max-forwards - there should be only one node between client and broadcaster */
        public int max_forwards { get; set; }
        
        /* token */
        public String tokenId { get; set; }

        protected VIA v;
        protected name_address to;
        protected name_address from;

        //Add other specific headers (as Authentication info or Authorization for digest)  to the following hash
        protected Hashtable extra;
        

        public struct name_address
        {
            public String display_name;
            public URI uri;
            public override String ToString()
            {
                return this.display_name + " <" + uri.ToString() + ">";
            }

        }
        public struct VIA
        {
            public String protocol;
            public URI uri;

            public override String ToString()
            {
                return this.protocol + " " + uri.ToString();
            }
        }

        //constructor
        public MessageHeader(String character_encoding = "", String content_type = "", int max_forwards = 1, String tokenId = "", int content_length = 0, String to_displayname = "", URI toUri = null, String from_displayname = "", URI fromUri = null, String via_protocol = "", URI via_uri = null, Hashtable extra = null)
        {
            this.character_encoding = character_encoding;
            this.content_type = content_type;
            this.max_forwards = max_forwards;
            this.tokenId = tokenId;
            this.to.display_name = to_displayname;
            this.to.uri = toUri;
            this.from.display_name = from_displayname;            
            this.from.uri = fromUri;
            this.v.protocol = via_protocol;
            this.v.uri = via_uri;
            this.extra = (extra != null)?extra:new Hashtable();
            this.content_length = content_length;
        }
       
        public void addHeader(String key, Object value)
        {
            extra.Add(key, value);                        
        }

        public void setTo(String display_name, URI uri)
        {
            this.to.display_name = display_name;
            this.to.uri = uri;
        }

        public name_address getTo()
        {
            return this.to;
        }

        public void setFrom(String display_name, URI uri)
        {
            this.from.display_name = display_name;
            this.from.uri = uri;
        }

        public name_address getFrom()
        {
            return this.from;
        }

        public void setVia(String protocol, URI uri)
        {
            this.v.protocol = protocol;
            this.v.uri = uri;
        }

        public VIA getVia()
        {
            return this.v;
        }

        public override String ToString()
        {
            String rt = "";
            
            if (this.v.uri != null)
            {
                rt += "Via: " + ((this.v.protocol != "") ? this.v.protocol + " " : "")+ this.v.uri.ToString() + this.delimiter;
            }
            if (this.max_forwards > 1)
            {
                rt += "Max-Forwards: " + this.content_type + this.delimiter;
            }
            if (this.to.uri != null)
            {
                rt += "To: " + this.to.ToString() + this.delimiter;
            }
            if (this.from.uri != null)
            {
                rt += "From: " + this.from.ToString() + this.delimiter;
            }
            if (this.tokenId != "")
            {
                rt += "tokenId: " + this.tokenId + this.delimiter;
            }
            if (this.character_encoding != "")
            {
                rt += "Accept-Encoding: " + this.character_encoding + this.delimiter; 
            }
            if (this.content_type != "")
            {
                rt += "Content-Type: " + this.content_type + this.delimiter;
            }
            if (this.content_length > 0)
            {
                rt += "Content-Length: " + this.content_length + this.delimiter;
            }

            foreach (String key in this.extra.Keys)
            {
                rt += key + ": " + this.extra[key].ToString() + this.delimiter;
            }
            

            return rt;
        }

    }
}
