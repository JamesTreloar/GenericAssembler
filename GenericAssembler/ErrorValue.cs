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


	public string ToString() {
		switch (errno) {
			case ErrorNumbers.InvalidJson:
				return "Invalid JSON provided";
			case ErrorNumbers.NoInstrInJson:
				return "No instructions were provided in the json input.";
			case ErrorNumbers.InvalidRLength:
				return "Invalid lengths for r-type instructions." +
				       "Instruction length needs to be equal to length for opcode, 2 registers, and immediate value" +
				       $"Currently instruction length is {errorDataInt[0]} and calculated length is {errorDataInt[1]}";
			case ErrorNumbers.InvalidILength:
				return "Invalid lengths for i-type instructions." +
				       "Instruction length needs to be equal to length for opcode, 2 registers, and immediate value" +
				       $"Currently instruction length is {errorDataInt[0]} and calculated length is {errorDataInt[1]}";
			case ErrorNumbers.InvalidJLength:
				return "Invalid lengths for j-type instructions." +
				       "Instruction length needs to be equal to length for opcode, and address" +
				       $"Currently instruction length is {errorDataInt[0]} and calculated length is {errorDataInt[1]}";
			case ErrorNumbers.MissingName:
				return $"There is no Name field in the {lineNum}th instruction definition";
			case ErrorNumbers.MissingOpCode:
				return $"There is no opcode field in the {lineNum}th instruction definition";
			case ErrorNumbers.BadOpCode:
				return $"Error in opcode format in the {lineNum}th instruction definition";
			case ErrorNumbers.OpCodeTooLong:
				return $"Opcode is too long for length provided in the {lineNum}th instruction definition" +
				       $"Provided value is {errorDataInt[0]}, when maximum value is {errorDataInt[1]}";
			case ErrorNumbers.BadShamt:
				return $"Error in shamt format in the {lineNum}th instruction definition";
			case ErrorNumbers.ShamtTooLong:
				return $"Shamt is too long for length provided in the {lineNum}th instruction definition" +
				       $"Provided value is {errorDataInt[0]}, when maximum value is {errorDataInt[1]}";
			case ErrorNumbers.BadFunct:
				return $"Error in funct format in the {lineNum}th instruction definition";
			case ErrorNumbers.FunctTooLong:
				return $"Funct is too long for length provided in the {lineNum}th instruction definition" +
				       $"Provided value is {errorDataInt[0]}, when maximum value is {errorDataInt[1]}";
			case ErrorNumbers.InvalidInstructionFormat:
				return $"Invalid instruction format ({errorDataString[0]}) " +
				       $"encountered on the {errorDataInt[0]}th instruction definition.";
			case ErrorNumbers.InvalidInstrInAsm:
				return $"Invalid instruction {errorDataString[0]} used in assembly file on line {lineNum}";
			case ErrorNumbers.InvalidRegisterFormat:
				return $"Invalid register format on line {lineNum}";
			case ErrorNumbers.InvalidImmediateFormat:
				return $"Immediate value on line {lineNum} is of an invalid format";
			case ErrorNumbers.InvalidImmediateLength:
				return $"Immediate on line {lineNum} does not fit within the space provided";
			case ErrorNumbers.InvalidAddressFormat:
				return $"Address value on line {lineNum} is of an invalid format";
			case ErrorNumbers.InvalidAddressLength:
				return $"Address on line {lineNum} does not fit within the space provided";
			default:
				return errno.ToString();
		}
	}
}


public enum ErrorNumbers {
	Okay,
	InvalidJson,
	NoInstrInJson,
	InvalidRLength,
	InvalidILength,
	InvalidJLength,
	MissingName,
	MissingFormat,
	MissingOpCode,
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
	InvalidImmediateFormat,
	InvalidAddressLength,
	InvalidAddressFormat
}
