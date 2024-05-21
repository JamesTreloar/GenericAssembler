using System.Text.Json;
using System.Text.Json.Nodes;

namespace GenericAssembler;

public class SettingReader(string fileName) {
	public (Configuration?, ErrorValue) Read() {
		string jsonString = File.ReadAllText(fileName);
		JsonNode node;
		ErrorValue ev = new(ErrorNumbers.Okay);
		Configuration? config;
		try {
			node = JsonNode.Parse(jsonString)!;

			config = JsonSerializer.Deserialize<Configuration>(jsonString);
		} catch (Exception e) {
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
			ev = new(ErrorNumbers.InvalidRLength, new int[] {instructionLen, rSum});
			return (null, ev);
		}

		int iSum = config.OpCodeLength + config.RegisterLength * 2 + config.ImmediateLength;
		if (iSum != config.InstructionLength) {
			ev = new(ErrorNumbers.InvalidILength, new int[] {instructionLen, rSum});
			return (null, ev);
		}

		int jSum = config.OpCodeLength + config.AddressLength;
		if (jSum != config.InstructionLength) {
			ev = new(ErrorNumbers.InvalidJLength, new int[] {instructionLen, rSum});
			return (null, ev);
		}

		foreach (JsonNode jsonNode in instructions) {
			Instruction add;
			string nemonic = jsonNode!["Nemonic"]!.ToString();
			int opcode;
			bool success = int.TryParse(jsonNode!["OpCode"]!.ToString(), out opcode);
			if (!success) {
				ev = new(ErrorNumbers.BadOpCode);
				return (null, ev);
			}

			if (opcode >= 1 << config.OpCodeLength) {
				ev = new(ErrorNumbers.OpCodeTooLong, new[] {opcode, 1 << config.OpCodeLength});
				return (null, ev);
			}
			
			int funct;
			switch (jsonNode!["Format"]!.ToString()) {
				case "R":
					int shamt;
					success = int.TryParse(jsonNode!["Shamt"]!.ToString(), out shamt);
					if (!success) {
						ev = new(ErrorNumbers.BadShamt);
						return (null, ev);
					}

					if (shamt >= 1 << config.ShamtLength) {
						ev = new(ErrorNumbers.ShamtTooLong, new[] {shamt, 1 << config.ShamtLength});
						return (null, ev);
					}
					
					success = int.TryParse(jsonNode!["Funct"]!.ToString(), out funct);
					if (!success) {
						ev = new(ErrorNumbers.BadFunct);
						return (null, ev);
					}

					if (funct >= (1 << config.FunctLength)) {
						ev = new(ErrorNumbers.FunctTooLong, new[] {funct, 1 << config.FunctLength});
						return (null, ev);
					}
					
					add = new(nemonic, InstructionFormat.R, opcode, shamt, funct);
					config.Instructions.Add(add);
					break;
				case "RShift":
					success = int.TryParse(jsonNode!["Funct"]!.ToString(), out funct);
					if (!success) {
						ev = new(ErrorNumbers.BadFunct);
						return (null, ev);
					}

					if (funct >= 1 << config.FunctLength) {
						ev = new(ErrorNumbers.FunctTooLong, new[] {funct, 1 << config.FunctLength});
						return (null, ev);
					}
					
					add = new(nemonic, InstructionFormat.RShift, opcode,
						// the instruction will have a value for shamt which is used over the 0 here
						0, (int)jsonNode!["Funct"]!);
					config.Instructions.Add(add);
					break;
				case "RSingle":
					add = new(nemonic, InstructionFormat.RSingle,
						opcode, (int)jsonNode!["Shamt"]!,
						(int)jsonNode!["Funct"]!);
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
					ev = new(ErrorNumbers.InvalidInstructionFormat, new int[] {jsonNode.GetElementIndex()}, new string[]{ jsonNode!["Format"]!.ToString() });
					return (null, ev);
			}
		}

		return (config, ev);
	}
}