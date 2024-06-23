namespace GenericAssembler;

public class ProcessFile(Configuration configuration) {
	public (List<string>?, ErrorValue) Run(string[] lines) {
		List<string> result = new();
		ErrorValue ev = new(ErrorNumbers.Okay);
		int lineNum = 0;
		bool failed = false;
		foreach (string line in lines) {
			lineNum++;
			string t = line.Trim();
			if (t[0] == '#') {
				continue;
			}

			string[] temp = t.Split(',', ' ');
			temp = temp.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

			List<Instruction> definitions = new(configuration.Instructions);
			int index = definitions.FindIndex(x => x.Nemonic == temp[0]);
			if (index == -1) {
				ev = new(ErrorNumbers.InvalidInstrInAsm, lineNum, new string[] { temp[0] });
				return (null, ev);
			}

			Instruction instruction = definitions[index];

			string calculatedInstruction = "";

			string data = Convert.ToString(instruction.OpCode, 2);
			data = data.PadLeft(configuration.OpCodeLength, '0');

			calculatedInstruction += data;
			bool success;
			int imme;
			switch (instruction.Format) {
				case InstructionFormat.R:
					if (!ValidateRegisters(temp[1], temp[2], temp[3])) {
						ev = new(ErrorNumbers.InvalidRegisterFormat, lineNum);
						failed = true;
					} else {
						calculatedInstruction +=
							ProcessRType(temp[2], temp[3], temp[1], instruction.Shamt, instruction.Funct);
					}

					break;
				case InstructionFormat.RShift:
					if (!ValidateRegisters(temp[1], temp[2])) {
						ev = new(ErrorNumbers.InvalidRegisterFormat, lineNum);
						failed = true;
					} else {
						calculatedInstruction +=
							ProcessRType("$0", temp[1], temp[2], instruction.Shamt, instruction.Funct);
					}

					break;
				case InstructionFormat.RSingle:
					if (!ValidateRegisters(temp[1])) {
						ev = new(ErrorNumbers.InvalidRegisterFormat, lineNum);
						failed = true;
					} else {
						calculatedInstruction +=
							ProcessRType(temp[1], "$0", "$0", instruction.Shamt, instruction.Funct);
					}

					break;
				case InstructionFormat.I:
					success = Utils.TryIntParse(temp[3], out imme);
					if (!success) {
						ev = new(ErrorNumbers.InvalidImmediateFormat, lineNum);
						return (null, ev);
					}

					if (!ValidateRegisters(temp[1], temp[2])) {
						ev = new(ErrorNumbers.InvalidRegisterFormat, lineNum);
						failed = true;
					} else if (!ValidateImmediate(imme)) {
						ev = new(ErrorNumbers.InvalidImmediateLength, lineNum);
						failed = true;
					} else {
						calculatedInstruction += ProcessIType(temp[1], temp[2], imme);
					}

					break;
				case InstructionFormat.IMem:
					string[] memSplit = temp[2].Split("(");
					success = Utils.TryIntParse(memSplit[0], out imme);
					if (!success) {
						ev = new(ErrorNumbers.InvalidImmediateFormat, lineNum);
						return (null, ev);
					}

					if (!ValidateRegisters(temp[1], memSplit[1][..^1])) {
						ev = new(ErrorNumbers.InvalidRegisterFormat, lineNum);
						failed = true;
					} else if (!ValidateImmediate(imme)) {
						ev = new(ErrorNumbers.InvalidImmediateLength, lineNum);
						failed = true;
					} else {
						calculatedInstruction += ProcessIType(temp[1], memSplit[1][..^1], imme);
					}

					break;
				case InstructionFormat.ISingle:
					success = Utils.TryIntParse(temp[2], out imme);
					if (!success) {
						ev = new(ErrorNumbers.InvalidImmediateFormat, lineNum);
						failed = true;
					}

					if (!ValidateRegisters(temp[1])) {
						ev = new(ErrorNumbers.InvalidRegisterFormat, lineNum);
						failed = true;
					} else if (!ValidateImmediate(imme)) {
						ev = new(ErrorNumbers.InvalidImmediateLength, lineNum);
						failed = true;
					} else {
						calculatedInstruction += ProcessIType("$0", temp[1], imme);
					}

					break;
				case InstructionFormat.J:
					int addr;
					success = Utils.TryIntParse(temp[1], out addr);
					if (!success) {
						ev = new(ErrorNumbers.InvalidAddressFormat, lineNum);
						return (null, ev);
					}

					if (!ValidateAddress(addr)) {
						ev = new(ErrorNumbers.InvalidAddressLength, lineNum);
						failed = true;
						break;
					}
					
					calculatedInstruction += Utils.BinaryStringConvert(addr, configuration.AddressLength);;
					break;
				default:
					throw new NotImplementedException();
			}

			if (failed) {
				return (null, ev);
			}

			result.Add(calculatedInstruction);
		}

		return (result, ev);
	}

	private bool ValidateImmediate(int val) {
		return ValidateConstant(val, configuration.ImmediateLength);
	}

	private bool ValidateAddress(int val) {
		return ValidateConstant(val, configuration.AddressLength);
	}

	private bool ValidateConstant(int val, int size) {
		return val < 1 << size && val > -1 * 1 << (size - 1);
	}

	private bool ValidateRegisters(params string[] registers) {
		foreach (string r in registers) {
			if (r[0] != '$') {
				return false;
			}

			if (r.Length == 1) {
				return false;
			}

			if (!int.TryParse(r[1..], out int _)) {
				if (!configuration.RegisterMap.ContainsKey(r[1..])) {
					return false;
				}
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
		calculatedInstruction += Utils.BinaryStringConvert(imme, configuration.ImmediateLength);
		
		return calculatedInstruction;
	}

	private string ProcessRegister(string registerCommand) {
		int regNum;
		bool res = int.TryParse(registerCommand[1..], out regNum);
		if (!res) {
			regNum = configuration.RegisterMap[registerCommand[1..]];
		}
		string reg = Convert.ToString(regNum, 2);
		reg = reg.PadLeft(configuration.RegisterLength, '0');
		return reg;
	}
}