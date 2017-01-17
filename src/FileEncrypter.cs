using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

namespace EncryptFiles
{
	class FileEncrypter
	{
		// Files to encrypt list
		private List<string> filenames = new List<string>();
		// Extention of files
		private string filesExtentions;
		// Folder to find files
		private string sourceDirectory;
		// Key for encryption
		private string key;

		public FileEncrypter()
		{
			// Get user documents folder
			sourceDirectory = ConfigurationManager.AppSettings["folderToEncrypt"] ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			// Get files extention
			filesExtentions = ConfigurationManager.AppSettings["filesExtention"] ?? "*";
			// Get encryption key
			if (ConfigurationManager.AppSettings["cryptoKey"] == null)
			{
				// Create an instance of Symetric Algorithm. Key and IV is generated automatically.
				var desCrypto = (DESCryptoServiceProvider)DES.Create();
				// Use the Automatically generated key for Encryption. 
				key = Encoding.ASCII.GetString(desCrypto.Key);
				// Save encryption key
				var currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				currentConfig.AppSettings.Settings.Add(new KeyValueConfigurationElement("cryptoKey", key));
				currentConfig.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection("appSettings");
			}
			else
				key = ConfigurationManager.AppSettings["cryptoKey"];
		}

		// 
		// Gets a list of files from a directory recursively
		public void GetFilesToEncrypt()
		{
			foreach(string extention in filesExtentions.Split(','))
				getFilesToEncrypt(sourceDirectory, extention);
		}

		/// <summary>
		/// Gets a list of files from a directory recursively
		/// </summary>
		/// <param name="sourceDirectory">Folder to encrypt</param>
		private void getFilesToEncrypt(string sourceDirectory, string extention)
		{
			try
			{
				// Get a list of all files in current directory
				var allFiles = Directory.EnumerateFiles(sourceDirectory);
				// Filter files by extention
				var filesToEncrypt = extention.Equals("*") ? allFiles : allFiles.Where(s => s.Split('.')[s.Split('.').Count() - 1].ToLower().Equals(extention));
				// Add all necessary files to the list
				filenames.AddRange(filesToEncrypt);
				// Get a folders list of the current folder
				var dirs = Directory.EnumerateDirectories(sourceDirectory);
				// Recursively get a list of files and subfolders pass
				foreach (var directory in dirs)
					getFilesToEncrypt(directory, extention);
			}
			catch (Exception ex)
			{
				// If something goes wrong writes message to console
				Console.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// Encrypt list of files
		/// </summary>
		public void EncryptFiles()
		{
			try
			{
				foreach (var filename in filenames)
				{
					// Read file
					var fsInput = new FileStream(filename, FileMode.Open, FileAccess.Read);
					byte[] bytearrayinput = new byte[fsInput.Length];
					fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
					fsInput.Close();

					// Create crypto service provider
					var DES = new DESCryptoServiceProvider();
					DES.Key = Encoding.ASCII.GetBytes(key);
					DES.IV = Encoding.ASCII.GetBytes(key);
					var desencrypt = DES.CreateEncryptor();

					// Write encrypted bytes
					var fsEncrypted = new FileStream(filename, FileMode.Create, FileAccess.Write);
					var cryptostream = new CryptoStream(fsEncrypted, desencrypt, CryptoStreamMode.Write);
					cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
					cryptostream.Close();
					fsEncrypted.Close();
				}
			}
			catch (Exception ex)
			{
				// If something goes wrong writes message to console
				Console.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// Decrypt list of files
		/// </summary>
		public void DecryptFiles()
		{
			try
			{
				foreach (var filename in filenames)
				{
					DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
					//A 64 bit key and IV is required for this provider.
					//Set secret key For DES algorithm.
					DES.Key = Encoding.ASCII.GetBytes(key);
					//Set initialization vector.
					DES.IV = Encoding.ASCII.GetBytes(key);

					//Create a file stream to read the encrypted file back.
					var fsread = new FileStream(filename,  FileMode.Open, FileAccess.Read);
					byte[] bytearrayinput = new byte[fsread.Length];
					fsread.Read(bytearrayinput, 0, bytearrayinput.Length);
					fsread.Close();

					//Create a DES decryptor from the DES instance.
					var desdecrypt = DES.CreateDecryptor();


					var fsDecrypted = new FileStream(filename, FileMode.Create, FileAccess.Write);
					var cryptostream = new CryptoStream(fsDecrypted, desdecrypt, CryptoStreamMode.Write);
					cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
					cryptostream.Close();
					fsDecrypted.Close();
				}
			}
			catch (Exception ex)
			{
				// If something goes wrong writes message to console
				Console.WriteLine(ex.Message);
			}
		}
	}
}
