namespace GenericAssembler;

public readonly struct Result<T> {
	public bool IsOk { get; }
	public T Value { get; }
	private readonly ErrorValue? _ev;
	
	public string GetError  => _ev.ToString();

	private Result(T value, ErrorValue? ev, bool isOk) {
		IsOk = isOk;
		Value = value;
		_ev = ev;
	}

	public static Result<T> Ok(T value) {
		return new (value, null, true);
	}

	public static Result<T> Err (ErrorValue ev) {
		return new(default, ev, false);
	}
}
