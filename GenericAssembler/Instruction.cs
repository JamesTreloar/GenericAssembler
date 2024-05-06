namespace GenericAssembler; 

public struct Instruction {
	public readonly string Nemonic;
	public readonly InstructionFormat Format;
	public readonly int OpCode;
	public readonly int Shamt;
	public readonly int Funct;


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