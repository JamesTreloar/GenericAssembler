namespace GenericAssembler; 

public struct Instruction {
	public string Nemonic { get; }
	public InstructionFormat Format { get; }
	public int OpCode { get; }
	public int Shamt { get; }
	public int Funct { get; }


	public Instruction(string nemonic, InstructionFormat format, int opCode) {
		if (format is InstructionFormat.R or InstructionFormat.RShift or InstructionFormat.RSingle) {
			throw new ArgumentException("R type instructions need a shamt and funct");
		}

		Nemonic = nemonic;
		Format = format;
		OpCode = opCode;
	}

	public Instruction(string nemonic, InstructionFormat format, int opCode, int shamt, int funct) {
		if (format is InstructionFormat.I or InstructionFormat.J or InstructionFormat.IMem or InstructionFormat.ISingle) {
			throw new ArgumentException("I and J type instructions do not have a shamt and funct");
		}
		Nemonic = nemonic;
		Format = format;
		OpCode = opCode;
		Shamt = shamt;
		Funct = funct;
	}
}

public enum InstructionFormat {
	R,
	I,
	J,
	RShift,
	RSingle,
	IMem,
	ISingle
}