using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace proyecto.libraries
{
    public class Encrypt
    {
        public string encriptar(string originalPassword)
        {
            if (string.IsNullOrEmpty(originalPassword))
            {
                return "";
            }
            else
            {
                byte[] originalBytes;
                byte[] encodedBytes;
                MD5 md5;
                //'Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
                md5 = new MD5CryptoServiceProvider();
                originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
                encodedBytes = md5.ComputeHash(originalBytes);

                //'Convert encoded bytes back to a 'readable' string
                return BitConverter.ToString(encodedBytes);
            }
        }
    }
}