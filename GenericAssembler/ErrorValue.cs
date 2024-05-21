namespace GenericAssembler;

public class ErrorValue {
	private ErrorNumbers errno;
	private int lineNum;
	private int[] errorDataInt;
	private string[] errorDataString;

	public ErrorValue(ErrorNumbers errno) {
		this.errno = errno;
	}

	public ErrorValue(ErrorNumbers errno, int lineNum) {
		this.errno = errno;
		this.lineNum = lineNum;
	}

	public ErrorValue(ErrorNumbers errno, int lineNum, int[] errorDataInt) {
		this.errno = errno;
		this.lineNum = lineNum;
		this.errorDataInt = errorDataInt;
	}

	public ErrorValue(ErrorNumbers errno, int[] errorDataInt) {
		this.errno = errno;
		this.errorDataInt = errorDataInt;
	}

	public ErrorValue(ErrorNumbers errno, int lineNum, string[] errorDataString) {
		this.errno = errno;
		this.lineNum = lineNum;
		this.errorDataString = errorDataString;
	}

	public ErrorValue(ErrorNumbers errno, string[] errorDataString) {
		this.errno = errno;
		this.errorDataString = errorDataString;
	}

	public ErrorValue(ErrorNumbers errno, int lineNum, int[] errorDataInt, string[] errorDataString) {
		this.errno = errno;
		this.lineNum = lineNum;
		this.errorDataInt = errorDataInt;
		this.errorDataString = errorDataString;
	}

	public ErrorValue(ErrorNumbers errno, int[] errorDataInt, string[] errorDataString) {
		this.errno = errno;
		this.errorDataInt = errorDataInt;
		this.errorDataString = errorDataString;
	}

	public bool IsOkay() {
		return errno == ErrorNumbers.Okay;
	}

	public void DisplayError() {
		TextWriter errorWritter = Console.Error;
		errorWritter.WriteLine("ERROR");
		switch (errno) {
			case ErrorNumbers.InvalidOptions:
				errorWritter.WriteLine("USAGE: ./GenericAssembler schema.json input.s");
				break;
			case ErrorNumbers.InvalidJson:
				errorWritter.WriteLine("Invalid JSON provided");
				break;
			case ErrorNumbers.NoInstrInJson:
				errorWritter.WriteLine("No instructions were provided in the json input.");
				break;
			case ErrorNumbers.InvalidRLength:
				errorWritter.WriteLine("Invalid lengths for i-type instructions.");
				errorWritter.WriteLine(
					"Instruction length needs to be equal to length for opcode, 2 registers, and immediate value");
				errorWritter.WriteLine($"Currently instruction length is {errorDataInt[0]} and calculated length is {errorDataInt[1]}");
				break;
			case ErrorNumbers.InvalidILength:
				errorWritter.WriteLine("Invalid lengths for i-type instructions.");
				errorWritter.WriteLine(
					"Instruction length needs to be equal to length for opcode, 2 registers, and immediate value");
				errorWritter.WriteLine($"Currently instruction length is {errorDataInt[0]} and calculated length is {errorDataInt[1]}");
				break;
			case ErrorNumbers.InvalidJLength:
				errorWritter.WriteLine("Invalid lengths for j-type instructions.");
				errorWritter.WriteLine("Instruction length needs to be equal to length for opcode, and address");
				errorWritter.WriteLine($"Currently instruction length is {errorDataInt[0]} and calculated length is {errorDataInt[1]}");
				break;
			case ErrorNumbers.BadOpCode:
				errorWritter.WriteLine("Error in opcode format");
				break;
			case ErrorNumbers.OpCodeTooLong:
				errorWritter.WriteLine("Opcode is too long for length provided");
				errorWritter.WriteLine($"Provided value is {errorDataInt[0]}, when maximum value is {errorDataInt[1]}");
				break;
			case ErrorNumbers.BadShamt:
				errorWritter.WriteLine("Error in shamt format");
				break;
			case ErrorNumbers.ShamtTooLong:
				errorWritter.WriteLine("Shamt is too long for length provided");
				break;
			case ErrorNumbers.BadFunct:
				errorWritter.WriteLine("Error in funct format");
				break;
			case ErrorNumbers.FunctTooLong:
				errorWritter.WriteLine("Funct is too long for length provided");
				break;
			case ErrorNumbers.InvalidInstructionFormat:
				errorWritter.WriteLine($"Invalid instruction format ({errorDataString[0]}) " +
				                  $"encountered on the {errorDataInt[0]}th instruction definition.");
				break;
			case ErrorNumbers.InvalidInstrInAsm:
				errorWritter.WriteLine($"Invalid instruction {errorDataInt[0]} used in assembly file on line {lineNum}");
				break;
			case ErrorNumbers.InvalidRegisterFormat:
				errorWritter.WriteLine($"Invalid register format on line {lineNum}");
				break;
			case ErrorNumbers.InvalidImmediateLength:
				errorWritter.WriteLine($"Immediate on line {lineNum} does not fit within the space provided");
				break;
			case ErrorNumbers.InvalidAddressLength:
				errorWritter.WriteLine($"Address on line {lineNum} does not fit within the space provided");
				break;
			default:
				throw new NotImplementedException();
		}
	}
}


public enum ErrorNumbers {
	Okay,
	InvalidOptions,
	InvalidJson,
	NoInstrInJson,
	InvalidRLength,
	InvalidILength,
	InvalidJLength,
	BadOpCode,
	OpCodeTooLong,
	BadShamt,
	ShamtTooLong,
	BadFunct,
	FunctTooLong,
	InvalidInstructionFormat,
	InvalidInstrInAsm,
	InvalidRegisterFormat,
	InvalidImmediateLength,
	InvalidAddressLength
}