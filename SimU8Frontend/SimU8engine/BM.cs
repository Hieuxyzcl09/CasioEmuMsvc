using System;
using System.Text;

namespace SimU8engine;

internal class BM
{
	public static byte LOBYTE(ushort val)
	{
		return (byte)(val & 0xFFu);
	}

	public static byte HIBYTE(ushort val)
	{
		return (byte)((uint)(val >> 8) & 0xFFu);
	}

	public static ushort MAKEWORD(byte lo, byte hi)
	{
		return (ushort)((ushort)(hi << 8) + lo);
	}

	public static byte I2B(int val)
	{
		return (byte)val;
	}

	public static byte UI2B(uint val)
	{
		return (byte)val;
	}

	public static byte UL2B(ulong val)
	{
		return (byte)val;
	}

	public static int UL2I(ulong val)
	{
		return (int)val;
	}

	public static ushort I2W(int val)
	{
		return (ushort)val;
	}

	public static ushort UI2W(uint val)
	{
		return (ushort)val;
	}

	public static ushort UL2W(ulong val)
	{
		return (ushort)val;
	}

	public static uint L2UI(long val)
	{
		return (uint)val;
	}

	public static int L2I(long val)
	{
		return (int)val;
	}

	public static bool I2bool(int val)
	{
		return val != 0;
	}

	public static string BA2S(byte[] buf)
	{
		int num = Array.IndexOf(buf, (byte)0);
		return Encoding.ASCII.GetString(buf, 0, (num == -1) ? buf.Length : num);
	}

	public static uint I2UI(int val)
	{
		return (uint)val;
	}

	public static int UI2I(uint val)
	{
		return (int)val;
	}
}
