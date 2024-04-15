namespace GenericAssembler; 

public class ProcessFile(Configuration configuration) {
	public List<string> Run(string[] lines) {
		List<string> result = new();

		foreach (string line in lines) {
			string t = line.Trim();
			if (t[0] == '#') {
				continue;
			}
			string[] temp = t.Split(',', ' ');
			temp = temp.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

			List<Instruction> definitions = new(configuration.Instructions);
			int index = definitions.FindIndex(x => x.Nemonic == temp[0]);
			if (index == -1) {
				throw new($"Invalid instruction {temp[0]} used in assembly file");
			}

			Instruction instruction = definitions[index];

			string calculatedInstruction = "";

			string data = Convert.ToString(instruction.OpCode, 2);
			data = data.PadLeft(configuration.OpCodeLength, '0');

			calculatedInstruction += data;
			switch (instruction.Format) {
				case InstructionFormat.R:
					calculatedInstruction += ProcessRType(temp[2], temp[3], temp[1], instruction.Shamt, instruction.Funct);
					break;
				case InstructionFormat.RShift:
					calculatedInstruction += ProcessRType("$0", temp[1], temp[2], instruction.Shamt, instruction.Funct);
					break;
				case InstructionFormat.RSingle:
					calculatedInstruction += ProcessRType(temp[1], "$0", "$0", instruction.Shamt, instruction.Funct);
					break;
				case InstructionFormat.I:
					calculatedInstruction += ProcessIType(temp[1], temp[2], int.Parse(temp[3]));
					break;
				case InstructionFormat.IMem:
					string[] foo = temp[2].Split("(");
					calculatedInstruction += ProcessIType(temp[1], foo[1][..^1], int.Parse(foo[0]));
					break;
				case InstructionFormat.ISingle:
					calculatedInstruction += ProcessIType("$0", temp[1], int.Parse(temp[2]));
					break;
				case InstructionFormat.J:
					string address = Convert.ToString(int.Parse(temp[1]), 2);
					address = address.PadLeft(configuration.AddressLength, '0');
					calculatedInstruction += address;
					break;
		}
			result.Add(calculatedInstruction);
		}

		return result;
	}

	private string ProcessRType(string rs, string rt, string rd, int shamt, int funct) {
		string calculatedInstruction = "";
		calculatedInstruction += ProcessRegister(rs);
		calculatedInstruction += ProcessRegister(rt);
		calculatedInstruction += ProcessRegister(rd);
		if (configuration.ShamtLength != 0) {
			calculatedInstruction += Convert.ToString(shamt, 2).PadLeft(configuration.ShamtLength, '0');
		}
					
		if (configuration.FunctLength != 0) {
			calculatedInstruction += Convert.ToString(funct, 2).PadLeft(configuration.FunctLength, '0');
		}

		return calculatedInstruction;
	}

	private string ProcessIType(string rs, string rt, int imme) {
		string calculatedInstruction = "";
		calculatedInstruction += ProcessRegister(rs);
		calculatedInstruction += ProcessRegister(rt);
		calculatedInstruction += Convert.ToString(imme, 2).PadLeft(configuration.ImmediateLength, '0');;
		return calculatedInstruction;
	}
	
	private string ProcessRegister(string registerCommand) {
		if (registerCommand[0] != '$') {
			throw new("Register references must begin with a $");
		}

		string reg = Convert.ToString(int.Parse(registerCommand[1..]), 2);
		reg = reg.PadLeft(configuration.RegisterLength, '0');
		return reg;
	}
}