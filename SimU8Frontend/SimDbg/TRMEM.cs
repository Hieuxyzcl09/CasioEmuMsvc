namespace SimDbg;

public struct TRMEM
{
	public uint pc;

	public ushort psw;

	public uint ramad;

	public byte ramdt;

	public ushort ramdt16;

	public byte probe;

	public byte intcycle;

	public byte atr;
}
