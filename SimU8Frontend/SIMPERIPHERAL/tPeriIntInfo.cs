namespace SIMPERIPHERAL;

public struct tPeriIntInfo
{
	public uint Irq_Addr;

	public ushort Irq_Bit;

	public byte[] IntSym;

	public void InitIntSym()
	{
		IntSym = new byte[32];
	}
}
