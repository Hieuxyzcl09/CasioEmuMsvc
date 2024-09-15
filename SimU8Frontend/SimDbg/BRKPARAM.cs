namespace SimDbg;

public struct BRKPARAM
{
	public uint adrbrk_adrs;

	public ushort adrbrk_pcnt;

	public DMPARAM[] dm_param;

	public ushort dm_pcnt;

	public byte dm_pair;

	public byte brkcond;

	public void InitDMParam()
	{
		dm_param = new DMPARAM[4];
	}
}
