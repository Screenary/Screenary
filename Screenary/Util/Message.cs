using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Screenary
{
    
    public class Message
    {
        /* configurable delimiter between lines (CRLF) */
        protected String delimiter = "\r\n";

        public MessageHeader header { get; set; }
        public String messageBody { get; set; }

        public Message(MessageHeader header = null, String messageBody = "")
        {
            this.header = header;
            this.messageBody = messageBody;
        }

        public override String ToString()
        {
            String rt = "";
            rt += ((this.header != null)?this.header.ToString():"") + ((this.messageBody != "")?this.messageBody:"");
            
            return rt;
        }

        /* will parse the header */
        public void parseHeader(String[] msglines)
        {
            for (int i = 1; i < msglines.Length; i++)
            {
                if (Regex.Match(msglines[i], "Via: ").Length > 1)
                {
                    String[] viafields = msglines[i].Replace("Via: ", "").Split(' ');
                    if (viafields.Length < 2)
                        throw new System.ArgumentException("Field error", "Via");

                    this.header.setVia(viafields[0], new URI(viafields[1]));
                }
                else if (Regex.Match(msglines[i], "To: ").Length > 1)
                {
                    String[] tofields = msglines[i].Replace("To: ", "").Replace("<", "").Replace(">", "").Split(' ');
                    if (tofields.Length < 2)
                        throw new System.ArgumentException("Field error", "To");

                    this.header.setTo(tofields[0], new URI(tofields[1]));
                }
                else if (Regex.Match(msglines[i], "From: ").Length > 1)
                {
                    String[] fromfields = msglines[i].Replace("From: ", "").Replace("<", "").Replace(">", "").Split(' ');
                    if (fromfields.Length < 2)
                        throw new System.ArgumentException("Field error", "From");

                    this.header.setFrom(fromfields[0], new URI(fromfields[1]));
                }
                else if (Regex.Match(msglines[i], "tokenId: ").Length > 1)
                {
                    this.header.tokenId = msglines[i].Replace("tokenId: ", "");
                }
                else if (Regex.Match(msglines[i], "Accept-Encoding: ").Length > 1)
                {
                    this.header.character_encoding = msglines[i].Replace("Accept-Encoding: ", "");
                }
                else if (Regex.Match(msglines[i], "Content-Type: ").Length > 1)
                {
                    this.header.content_type = msglines[i].Replace("Content-Type: ", "");
                }
                else if (Regex.Match(msglines[i], "Content-Length: ").Length > 1)
                {
                    this.header.content_length = Convert.ToInt32(msglines[i].Replace("Content-Length: ", ""));
                }
                else if (Regex.Match(msglines[i], ": ").Length > 1)
                {
                    msglines[i] = msglines[i].Replace(": ", ":");
                    String[] extrafields = msglines[i].Split(':');
                    this.header.addHeader(extrafields[0], extrafields[1]);
                }
                else
                {
                    this.messageBody = msglines[i];
                }
            }

        }





    }

}
