namespace GenericAssembler;

public readonly struct PartialInstruction {
	public string generated {get; init; }
	public string? jumpTo { get; init; }

	public int jumpLength { get; init; }
	public PartialInstruction(string generated, string? jumpTo = null, int jumpLength = 0) {
		this.generated = generated;
		this.jumpTo = jumpTo;
		this.jumpLength = jumpLength;
	}
}
