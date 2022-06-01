using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
			string convert = Encrypt(textBox_salt.Text, textBox_key.Text, textBox_passwordhash.Text,textBox_valuetext.Text);
			textBox_result.Text = convert;

		}

		private void button_decrypt_Click(object sender, EventArgs e)
		{
			string convert = Decrypt(textBox_salt.Text, textBox_key.Text, textBox_passwordhash.Text, textBox_valuetext.Text);
			textBox_result.Text = convert;
		}

		private static string Encrypt(string saltString, string keyString, string passwordhash, string valueString)
		{
			byte[] plainTextBytes = Encoding.UTF8.GetBytes(valueString);

			byte[] keyBytes = new Rfc2898DeriveBytes(passwordhash, Encoding.ASCII.GetBytes(saltString)).GetBytes(256 / 8);
			var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
			var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(keyString));

			byte[] cipherTextBytes;

			using (var memoryStream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
				{
					cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
					cryptoStream.FlushFinalBlock();
					cipherTextBytes = memoryStream.ToArray();
					cryptoStream.Close();
				}
				memoryStream.Close();
			}
			return Convert.ToBase64String(cipherTextBytes);
		}

		public static string Decrypt(string saltString, string keyString, string passwordhash, string valueString)
		{
			byte[] cipherTextBytes = Convert.FromBase64String(valueString);
			byte[] keyBytes = new Rfc2898DeriveBytes(passwordhash, Encoding.ASCII.GetBytes(saltString)).GetBytes(256 / 8);
			var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

			var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(keyString));
			var memoryStream = new MemoryStream(cipherTextBytes);
			var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
			byte[] plainTextBytes = new byte[cipherTextBytes.Length];

			int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
			memoryStream.Close();
			cryptoStream.Close();
			return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
		}

        
    }
}
