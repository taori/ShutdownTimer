namespace ShutdownTimer.Server.Data
{
	public class TypedCheckboxOption<T>
	{
		public TypedCheckboxOption(string description, bool @checked, T value)
		{
			Description = description;
			Checked = @checked;
			Value = value;
		}

		public TypedCheckboxOption()
		{
		}

		public string Description { get; set; }
		
		public bool Checked { get; set; }

		public T Value { get; set; }
	}
}