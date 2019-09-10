using System;
using System.Collections.Generic;
using System.Linq;

namespace ShutdownTimer.Server.Data
{
	public class DescriptiveResult
	{
		private DescriptiveResult(bool success, IEnumerable<DescriptiveError> errors)
		{
			Succeeded = success;
			Errors.AddRange(errors);
		}

		public List<DescriptiveError> Errors { get; set; } = new List<DescriptiveError>();

		public bool Succeeded { get; set; }

		public static DescriptiveResult Success { get; } = new DescriptiveResult(true, new List<DescriptiveError>());

		public static DescriptiveResultErrorBuilder Fail () => new DescriptiveResultErrorBuilder(new DescriptiveResult(false, Enumerable.Empty<DescriptiveError>()));

		public static DescriptiveResult Fail (params DescriptiveError[] errors) => new DescriptiveResult(false, errors);

		public class DescriptiveResultErrorBuilder
		{
			private readonly DescriptiveResult _descriptiveResult;

			public DescriptiveResultErrorBuilder(DescriptiveResult descriptiveResult)
			{
				_descriptiveResult = descriptiveResult;
			}

			public DescriptiveResultErrorBuilder With(string code, string description)
			{
				_descriptiveResult.Errors.Add(new DescriptiveError() { Code = code, Description = description });
				return this;
			}

			public DescriptiveResultErrorBuilder With(DescriptiveError error)
			{
				_descriptiveResult.Errors.Add(error);
				return this;
			}

			public static implicit operator DescriptiveResult(DescriptiveResultErrorBuilder source)
			{
				return source._descriptiveResult;
			}
		}
	}

	public class DescriptiveError
	{
		public DescriptiveError(string code, string description)
		{
			Code = code;
			Description = description;
		}

		public DescriptiveError()
		{
		}

		/// <summary>Gets or sets the code for this error.</summary>
		/// <value>The code for this error.</value>
		public string Code { get; set; }

		/// <summary>Gets or sets the description for this error.</summary>
		/// <value>The description for this error.</value>
		public string Description { get; set; }
	}
}