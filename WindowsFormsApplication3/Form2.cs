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
	public partial class Form2 : Form
	{
		public Form2()
		{
			InitializeComponent();
		}

		
		public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
		{
			byte[] encryptedBytes = null;

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

					using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
					{
						cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
						cs.Close();
					}
					encryptedBytes = ms.ToArray();
				}
			}

			return encryptedBytes;
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
			string password1 = textBox1.ToString();
			string password2 = textBox2.ToString();

			if (password1 == password2 && file != null)
			{
				byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
				byte[] passwordBytes = Encoding.UTF8.GetBytes(password1);

				// Hash the password with SHA256
				passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

				byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

				string fileEncrypted = file + "encrypted";

				File.WriteAllBytes(fileEncrypted, bytesEncrypted);
				MessageBox.Show("Encryption successful!\nEncrypted file saved as <"
					+ fileEncrypted + ">");
			}
			else if(password2 != password1)
				MessageBox.Show("Passwords don't match");
			else
				MessageBox.Show("Choose a file first!");
		}

		private void button3_Click(object sender, EventArgs e)
		{
			Form2.ActiveForm.Visible = false;
			Form1 frm = new Form1();
			frm.Show();
		}
	}
}
