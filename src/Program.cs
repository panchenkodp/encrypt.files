using System;

namespace EncryptFiles
{
	class Program
	{
		static void Main(string[] args)
		{
			// Create fileEncrypter object
			var fileEncrypter = new FileEncrypter();
			// Fill the list of files to encrypt
			fileEncrypter.GetFilesToEncrypt();
			if(args.Length > 0)
			{
				switch (args[0])
				{
					case "/decrypt":
						Console.WriteLine("Decrypting...");
						fileEncrypter.DecryptFiles();
						break;
					default:
						Console.WriteLine("Unknown command.");
						Console.WriteLine("To decrypt files use \"/decrypt\" key");
						Console.WriteLine("To encrypt files use without any key. To configure use file with \"Config\" extention");
						break;
				}
			}
			else
			{ 
				// Encrypt files
				Console.WriteLine("Encrypting...");
				fileEncrypter.EncryptFiles();
			}
		}
	}
}
