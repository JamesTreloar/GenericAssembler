namespace GenericAssembler; 

public class Configuration {
	public int InstructionLength { get; set; }
	public int OpCodeLength { get; set; }
	public int RegisterLength { get; set; }
	public int ImmediateLength { get; set; }
	public int AddressLength { get; set; }
	public int ShamtLength { get; set; }
	public int FunctLength { get; set; }

	public Dictionary<string, int> RegisterMap = new();
	
    public List<Instruction> Instructions = new();
}
