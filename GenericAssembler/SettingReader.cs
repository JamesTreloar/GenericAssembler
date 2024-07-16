using System.Text.Json;
using System.Text.Json.Nodes;

namespace GenericAssembler;

public class SettingReader(string jsonString) {
	public (Configuration?, ErrorValue) Read() {
		JsonNode node;
		ErrorValue ev = new(ErrorNumbers.Okay);
		Configuration? config;
		try {
			node = JsonNode.Parse(jsonString)!;
			config = JsonSerializer.Deserialize<Configuration>(jsonString);
		} catch {
			ev = new(ErrorNumbers.InvalidJson);
			return (null, ev);
		}

		if (config == null) {
			ev = new(ErrorNumbers.InvalidJson);
			return (null, ev);
		}

		JsonArray instructions = node!["Instructions"]!.AsArray();
		if (instructions.Count < 1) {
			ev = new(ErrorNumbers.NoInstrInJson);
			return (null, ev);
		}

		int instructionLen = config.InstructionLength;
		int rSum = config.OpCodeLength + config.RegisterLength * 3 + config.FunctLength + config.ShamtLength;
		if (rSum != instructionLen) {
			ev = new(ErrorNumbers.InvalidRLength, new [] { instructionLen, rSum });
			return (null, ev);
		}

		int iSum = config.OpCodeLength + config.RegisterLength * 2 + config.ImmediateLength;
		if (iSum != instructionLen) {
			ev = new(ErrorNumbers.InvalidILength, new [] { instructionLen, iSum });
			return (null, ev);
		}

		int jSum = config.OpCodeLength + config.AddressLength;
		if (jSum != instructionLen) {
			ev = new(ErrorNumbers.InvalidJLength, new [] { instructionLen, jSum });
			return (null, ev);
		}

		foreach (JsonNode jsonNode in instructions) {
			// Validate Nemonic
			bool success = jsonNode.AsObject().ContainsKey("Nemonic");
			if (!success) {
				ev = new(ErrorNumbers.MissingNemonic, jsonNode.GetElementIndex());
				return (null, ev);
			}

			string nemonic = jsonNode!["Nemonic"]!.ToString();

			// Validate Format
			success = jsonNode.AsObject().ContainsKey("Format");
			if (!success) {
				ev = new(ErrorNumbers.MissingFormat, jsonNode.GetElementIndex());
				return (null, ev);
			}

			string format = jsonNode!["Format"]!.ToString();

			// Validate Opcode
			success = jsonNode.AsObject().ContainsKey("OpCode");
			if (!success) {
				ev = new(ErrorNumbers.MissingOpCode, jsonNode.GetElementIndex());
				return (null, ev);
			}

			int opcode;
			success = Utils.TryIntParse(jsonNode!["OpCode"]!.ToString(), out opcode);
			if (!success) {
				ev = new(ErrorNumbers.BadOpCode, jsonNode.GetElementIndex());
				return (null, ev);
			}

			if (opcode >= 1 << config.OpCodeLength) {
				ev = new(ErrorNumbers.OpCodeTooLong, jsonNode.GetElementIndex(),
					new[] { opcode, 1 << config.OpCodeLength });
				return (null, ev);
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
					return (null, ev);
				}

				if (shamt >= 1 << config.ShamtLength) {
					ev = new(ErrorNumbers.ShamtTooLong, jsonNode.GetElementIndex(),
						new[] { shamt, 1 << config.ShamtLength });
					return (null, ev);
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
					return (null, ev);
				}

				if (funct >= 1 << config.ShamtLength) {
					ev = new(ErrorNumbers.FunctTooLong, jsonNode.GetElementIndex(),
						new[] { funct, 1 << config.FunctLength });
					return (null, ev);
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
					return (null, ev);
			}
		}

		if (node.AsObject().ContainsKey("Registers")) {
			JsonArray registers = node!["Registers"]!.AsArray();
			foreach (JsonNode register in registers) {
				config.RegisterMap[register!["name"]!.ToString()] = int.Parse(register!["number"]!.ToString());
			}
		}

		return (config, ev);
	}
}