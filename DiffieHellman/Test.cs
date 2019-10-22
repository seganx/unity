using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX.DiffieHellman
{
    public static class Test
    {
        public class GameinHeader
        {
            public string userdata;
            public string error;
            public string access;
        }

        //  hold the last recevied header to be used in other functions
        public static GameinHeader LastReceivedHeader { get; private set; }

        //  hold the computed synced final key which is shared between client and server
        public static byte[] FinalKey { get; private set; }



        //  extract gamein headers from www object
        public static GameinHeader GetHeader(WWW ws)
        {
            var res = new GameinHeader();
            ws.responseHeaders.TryGetValue("GAMEIN_USERDATA", out res.userdata);
            ws.responseHeaders.TryGetValue("GAMEIN_ERROR", out res.error);
            ws.responseHeaders.TryGetValue("GAMEIN_ACCESS", out res.access);
            return res;
        }

        //  post request to the server
        public static WWW PostWWW(string uri, string userdata, string accessCode, byte[] data)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("GAMEIN_USERDATA", userdata);
            headers.Add("GAMEIN_ACCESS", accessCode);
            return new WWW(uri, data, headers);
        }

        //  request authentication ticket
        public static IEnumerator RequestAuthenCode(string uriCode)
        {
            //  prepare diffihelman keys
            byte[] secretKey = Service.SecretKey(32);
            byte[] publicKey = Service.PublicKey(secretKey, 7, 23);

            //  spost public key to the server and wait for response
            var ws = PostWWW(uriCode, "mydata", "", publicKey);
            yield return ws;
            Debug.Log("received: " + ws.text);

            /*
             * data received from server. 
             * extract final gamein header from server to be used in other functions
             * get public key from server and compute final key
            */
            LastReceivedHeader = GetHeader(ws);
            if (LastReceivedHeader.error == "none" && LastReceivedHeader.userdata == "mydata")
            {
                byte[] rcvd_key = System.Text.Encoding.ASCII.GetBytes(ws.text);
                FinalKey = Service.FinalKey(secretKey, rcvd_key, 23);

                Debug.Log("Key: " + System.Text.ASCIIEncoding.ASCII.GetString(FinalKey));
            }
        }

        public static IEnumerator LoginWithDevice(string uriDevice, string gameKey, string deviceId)
        {
            if (LastReceivedHeader.userdata == "63")
            {
                //  prepare json data to be sent to server
                var msg = "{" +
                    "\"ver\":1" +
                    ",\"game\":" + gameKey +
                    ",\"device\":" + deviceId +
                    "}";
                Debug.Log("sending: " + msg);

                //  encrypt data via final key
                byte[] endata = Service.Encrypt(System.Text.Encoding.ASCII.GetBytes(msg), FinalKey);

                //  post encrypted data to the server and wait for response
                var ws = PostWWW(uriDevice, LastReceivedHeader.userdata, LastReceivedHeader.access, endata);
                yield return ws;
                Debug.Log("received: " + ws.text);

                /*
                 * data received from server. 
                 * extract gamein header from server to be used in other functions
                 */
                LastReceivedHeader = GetHeader(ws);
            }
        }
    }
}
