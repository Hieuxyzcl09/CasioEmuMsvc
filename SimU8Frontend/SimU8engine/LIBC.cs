using System;
using System.Text;

namespace SimU8engine;

public class LIBC
{
	private static Random _random;

	public const int RAND_MAX = 1073741823;

	public static int time()
	{
		return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
	}

	public static int strcpy_s(byte[] dest, byte[] src)
	{
		Array.Copy(src, dest, Math.Min(src.Length, dest.Length));
		return 0;
	}

	public static int strcpy_s(out string dest, string src)
	{
		dest = src;
		return 0;
	}

	public static int strcpy_s(byte[] dest, string src)
	{
		return strcpy_s(dest, Encoding.UTF8.GetBytes(src));
	}

	public static void srand(int seed)
	{
		_random = new Random(seed);
	}

	public static int rand()
	{
		return _random.Next(1073741823);
	}

	public static void SplitPath(string ppath, out string pdrive, out string pdir)
	{
		int num = ppath.IndexOf("\\");
		pdrive = ppath.Substring(0, num);
		pdir = ppath.Substring(num + 1);
	}

	public static void AdjustMacPath(string ppath, out string cpath)
	{
		cpath = ppath.Substring(0, ppath.IndexOf("MonoBundle", StringComparison.Ordinal));
	}
}
