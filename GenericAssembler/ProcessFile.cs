namespace GenericAssembler; 

public class ProcessFile(Configuration? configuration) {
	public List<string>? Run(string[] lines) {
		List<string>? result = new();

		int linenum = 0;
		bool failed = false;
		foreach (string line in lines) {
			linenum++;
			string t = line.Trim();
			if (t[0] == '#') {
				continue;
			}
			string[] temp = t.Split(',', ' ');
			temp = temp.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

			List<Instruction> definitions = new(configuration.Instructions);
			int index = definitions.FindIndex(x => x.Nemonic == temp[0]);
			if (index == -1) {
				Console.WriteLine("ERROR");
				Console.WriteLine($"Invalid instruction {temp[0]} used in assembly file on line {linenum}");
				return null;
			}

			Instruction instruction = definitions[index];

			string calculatedInstruction = "";

			string data = Convert.ToString(instruction.OpCode, 2);
			data = data.PadLeft(configuration.OpCodeLength, '0');

			calculatedInstruction += data;
			switch (instruction.Format) {
				case InstructionFormat.R:
					if (!ValidateRegisters(temp[1], temp[2], temp[3])) {
						failed = true;
					} else {
						calculatedInstruction +=
							ProcessRType(temp[2], temp[3], temp[1], instruction.Shamt, instruction.Funct);
					}
					break;
				case InstructionFormat.RShift:
					if (!ValidateRegisters(temp[1], temp[2])) {
						failed = true;
					} else {
						calculatedInstruction +=
							ProcessRType("$0", temp[1], temp[2], instruction.Shamt, instruction.Funct);
					}
					break;
				case InstructionFormat.RSingle:
					if (!ValidateRegisters(temp[1])) {
						failed = true;
					} else {
						calculatedInstruction +=
							ProcessRType(temp[1], "$0", "$0", instruction.Shamt, instruction.Funct);
					}
					break;
				case InstructionFormat.I:
					if (!ValidateRegisters(temp[1], temp[2])) {
						failed = true;
					} else {
						calculatedInstruction += ProcessIType(temp[1], temp[2], int.Parse(temp[3]));
					}
					break;
				case InstructionFormat.IMem:
					string[] foo = temp[2].Split("(");
					if (!ValidateRegisters(temp[1], foo[1][..^1])) {
						failed = true;
					} else {
						calculatedInstruction += ProcessIType(temp[1], foo[1][..^1], int.Parse(foo[0]));
					}
					break;
				case InstructionFormat.ISingle:
					if (!ValidateRegisters(temp[1])) {
						failed = true;
					} else {
						calculatedInstruction += ProcessIType("$0", temp[1], int.Parse(temp[2]));
					}
					break;
				case InstructionFormat.J:
					string address = Convert.ToString(int.Parse(temp[1]), 2);
					address = address.PadLeft(configuration.AddressLength, '0');
					calculatedInstruction += address;
					break;
			}

			if (failed) {
				Console.WriteLine("ERROR");
				Console.WriteLine($"Invalid register format on line {linenum}");
				return null;
			}
			result.Add(calculatedInstruction);
		}

		return result;
	}

	private bool ValidateRegisters(params string[] registers) {
		foreach (string r in registers) {
			if (r[0] != '$') {
				return false;
			}

			if (r.Length == 1) {
				return false;
			}
		}

		return true;
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
		string reg = Convert.ToString(int.Parse(registerCommand[1..]), 2);
		reg = reg.PadLeft(configuration.RegisterLength, '0');
		return reg;
	}
}