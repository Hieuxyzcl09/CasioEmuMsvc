using System;
using System.Text;

namespace SimU8engine;

public class COutMod
{
	private StringBuilder _bld;

	public COutMod()
	{
		_bld = new StringBuilder();
	}

	public COutMod W(string v)
	{
		_bld.Append(v);
		return this;
	}

	public COutMod W(uint v)
	{
		_bld.Append(v);
		return this;
	}

	public COutMod dec(uint v)
	{
		_bld.Append(v.ToString("d"));
		return this;
	}

	public COutMod hex(uint v)
	{
		_bld.Append(v.ToString("x"));
		return this;
	}

	public COutMod hex2dig(uint v)
	{
		_bld.Append(v.ToString("x2"));
		return this;
	}

	public COutMod hex4dig(uint v)
	{
		_bld.Append(v.ToString("x4"));
		return this;
	}

	public COutMod endl()
	{
		Console.Out.WriteLine(_bld.ToString());
		_bld.Clear();
		return this;
	}
}
