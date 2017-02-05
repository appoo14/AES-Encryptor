using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace WindowsFormsApplication3
{
	public partial class Form3 : Form
	{
		public Form3()
		{
			InitializeComponent();
		}

		public byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
		{
			byte[] decryptedBytes = null;
			byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

			using (MemoryStream ms = new MemoryStream())
			{
				using (RijndaelManaged AES = new RijndaelManaged())
				{
					AES.KeySize = 256;
					AES.BlockSize = 128;

					var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
					AES.Key = key.GetBytes(AES.KeySize / 8);
					AES.IV = key.GetBytes(AES.BlockSize / 8);

					AES.Mode = CipherMode.CBC;

					using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
						cs.Close();
					}
					decryptedBytes = ms.ToArray();
				}
			}

			return decryptedBytes;
		}

		public static string file = null;

		private void button1_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				file = openFileDialog1.FileName;
			}
			else
			{
				MessageBox.Show("No file Choosen!");
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if(file != null)
			{
				try
				{
					string password = textBox1.ToString();

					byte[] bytesToBeDecrypted = File.ReadAllBytes(file);
					byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
					passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

					byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

					string fileEncrypted = file.Remove(file.Length - 9);
					File.WriteAllBytes(fileEncrypted, bytesDecrypted);
					MessageBox.Show("File decrypted according to the password!");
				}
				catch
				{
					MessageBox.Show("Error!");
				}
			}
			else
				MessageBox.Show("No file choosen!");
		}

		private void button3_Click(object sender, EventArgs e)
		{
			Form3.ActiveForm.Visible = false;
			Form1 frm = new Form1();
			frm.Show();
		}


	}
}
