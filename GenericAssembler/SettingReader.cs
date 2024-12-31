using System.Text.Json;
using System.Text.Json.Nodes;

namespace GenericAssembler;

public class SettingReader(string jsonString) {
	public Result<Configuration> Read() {
		JsonNode node;
		ErrorValue ev = new(ErrorNumbers.Okay);
		Configuration? config;
		try {
			node = JsonNode.Parse(jsonString)!;

			config = JsonSerializer.Deserialize<Configuration>(jsonString);
		} catch (Exception e) {
			ev = new(ErrorNumbers.InvalidJson);
			return Result<Configuration>.Err(ev);
		}

		if (config == null) {
			ev = new(ErrorNumbers.InvalidJson);
			return Result<Configuration>.Err(ev);
		}

		JsonArray instructions = node!["Instructions"]!.AsArray();
		if (instructions.Count < 1) {
			ev = new(ErrorNumbers.NoInstrInJson);
			return Result<Configuration>.Err(ev);
		}

		int instructionLen = config.InstructionLength;
		int rSum = config.OpCodeLength + config.RegisterLength * 3 + config.FunctLength + config.ShamtLength;
		if (rSum != instructionLen) {
			ev = new(ErrorNumbers.InvalidRLength, new int[] { instructionLen, rSum });
			return Result<Configuration>.Err(ev);
		}

		int iSum = config.OpCodeLength + config.RegisterLength * 2 + config.ImmediateLength;
		if (iSum != instructionLen) {
			ev = new(ErrorNumbers.InvalidILength, new int[] { instructionLen, iSum });
			return Result<Configuration>.Err(ev);
		}

		int jSum = config.OpCodeLength + config.AddressLength;
		if (jSum != instructionLen) {
			ev = new(ErrorNumbers.InvalidJLength, new int[] { instructionLen, jSum });
			return Result<Configuration>.Err(ev);
		}

		foreach (JsonNode jsonNode in instructions) {
			// Validate Nemonic
			bool success = jsonNode.AsObject().ContainsKey("Nemonic");
			if (!success) {
				ev = new(ErrorNumbers.MissingNemonic, jsonNode.GetElementIndex());
				return Result<Configuration>.Err(ev);
			}

			string nemonic = jsonNode!["Nemonic"]!.ToString();

			// Validate Format
			success = jsonNode.AsObject().ContainsKey("Format");
			if (!success) {
				ev = new(ErrorNumbers.MissingFormat, jsonNode.GetElementIndex());
				return Result<Configuration>.Err(ev);
			}

			string format = jsonNode!["Format"]!.ToString();

			// Validate Opcode
			success = jsonNode.AsObject().ContainsKey("OpCode");
			if (!success) {
				ev = new(ErrorNumbers.MissingOpCode, jsonNode.GetElementIndex());
				return Result<Configuration>.Err(ev);
			}

			int opcode;
			success = Utils.TryIntParse(jsonNode!["OpCode"]!.ToString(), out opcode);
			if (!success) {
				ev = new(ErrorNumbers.BadOpCode, jsonNode.GetElementIndex());
				return Result<Configuration>.Err(ev);
			}

			if (opcode >= 1 << config.OpCodeLength) {
				ev = new(ErrorNumbers.OpCodeTooLong, jsonNode.GetElementIndex(),
					new[] { opcode, 1 << config.OpCodeLength });
				return Result<Configuration>.Err(ev);
			}

			// Validate Shamt
			int shamt;
			success = jsonNode.AsObject().ContainsKey("Shamt");
			if (!success) {
				shamt = 0;
			} else {
				success = Utils.TryIntParse(jsonNode!["Shamt"]!.ToString(), out shamt);
				if (!success) {
					ev = new(ErrorNumbers.BadShamt, jsonNode.GetElementIndex());
					return Result<Configuration>.Err(ev);
				}

				if (shamt >= 1 << config.ShamtLength) {
					ev = new(ErrorNumbers.ShamtTooLong, jsonNode.GetElementIndex(),
						new[] { shamt, 1 << config.ShamtLength });
					return Result<Configuration>.Err(ev);
				}
			}

			// Validate Funct
			int funct;
			success = jsonNode.AsObject().ContainsKey("Funct");
			if (!success) {
				funct = 0;
			} else {
				success = Utils.TryIntParse(jsonNode!["Funct"]!.ToString(), out funct);
				if (!success) {
					ev = new(ErrorNumbers.BadFunct, jsonNode.GetElementIndex());
					return Result<Configuration>.Err(ev);
				}

				if (funct >= 1 << config.ShamtLength) {
					ev = new(ErrorNumbers.FunctTooLong, jsonNode.GetElementIndex(),
						new[] { funct, 1 << config.FunctLength });
					return Result<Configuration>.Err(ev);
				}
			}

			Instruction add;
			switch (format) {
				case "R":
					add = new(nemonic, InstructionFormat.R, opcode, shamt, funct);
					config.Instructions.Add(add);
					break;
				case "RShift":
					add = new(nemonic, InstructionFormat.RShift, opcode, shamt, funct);
					config.Instructions.Add(add);
					break;
				case "RSingle":
					add = new(nemonic, InstructionFormat.RSingle, opcode, shamt, funct);
					config.Instructions.Add(add);
					break;
				case "I":
					add = new(nemonic, InstructionFormat.I, opcode);
					config.Instructions.Add(add);
					break;
				case "IMem":
					add = new(nemonic, InstructionFormat.IMem, opcode);
					config.Instructions.Add(add);
					break;
				case "ISingle":
					add = new(nemonic, InstructionFormat.ISingle, opcode);
					config.Instructions.Add(add);
					break;
				case "J":
					add = new(nemonic, InstructionFormat.J, opcode);
					config.Instructions.Add(add);
					break;
				default:
					ev = new(ErrorNumbers.InvalidInstructionFormat, jsonNode.GetElementIndex(),
						new[] { jsonNode!["Format"]!.ToString() });
					return Result<Configuration>.Err(ev);
			}
		}


		if (node.AsObject().ContainsKey("Registers")) {
			JsonArray registers = node!["Registers"]!.AsArray();

			foreach (JsonNode register in registers) {
				config.RegisterMap[register!["name"]!.ToString()] = int.Parse(register!["number"]!.ToString());
			}
		}

		return Result<Configuration>.Ok(config);
	}
}