using System.Runtime.InteropServices;

internal class Program
{
	[DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
	//static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);
	static extern bool CreateHardLink(
	  string lpFileName,
	  string lpExistingFileName,
	  IntPtr lpSecurityAttributes
	);

	public static void MakeLink(string source, string target)
	{
		if (!File.Exists(source)) return;
		if (File.Exists(target)) return;

		CreateHardLink(target, source, IntPtr.Zero);
	}

	private static void Main(string[] args)
	{

		bool validArg = true;
		if(args.Length != 2)
		{
			validArg = false;
		}
		if (!Directory.Exists(args[0]))
		{
			validArg = false;
		}
		if (!Directory.Exists(args[1]))
		{
			validArg = false;
		}
		if (!validArg)
		{
			Console.WriteLine("hardlinkfolder <sourcedir> <destdir>");
		}
		else
		{
			string source = args[0];
			string target = args[1];
			source = Path.GetFullPath(source);
			target = Path.GetFullPath(target);

			if(Directory.GetDirectoryRoot(source) != Directory.GetDirectoryRoot(target))
			{
				throw new Exception("Both source and target need to have the same root");
			}

			source = Directory.GetParent(Path.Combine(source, "tmp.file")).FullName;
			target = Directory.GetParent(Path.Combine(target, "tmp.file")).FullName;

			var files = Directory.GetFiles(source, "*",
			new EnumerationOptions
			{
				IgnoreInaccessible = true,
				RecurseSubdirectories = true,
			});
			foreach( string file in files )
			{
				string newfile = target + file.Remove(0, source.Length);
				string newfiledir = Directory.GetParent(newfile).FullName;
				if (!Directory.Exists(newfiledir))
				{
					Directory.CreateDirectory(newfiledir);
				}
				MakeLink(file, newfile);
			}


		}


	}
}