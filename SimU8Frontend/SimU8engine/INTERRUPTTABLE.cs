namespace SimU8engine;

public struct INTERRUPTTABLE
{
	public uint vector_adrs;

	public uint ie_adrs;

	public byte ie_bit;

	public byte ie_mask;

	public uint irq_adrs;

	public byte irq_bit;

	public byte irq_mask;

	public byte[] intname;

	public void InitIntname()
	{
		intname = new byte[20];
	}
}
