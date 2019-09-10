using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShutdownTimer.Server.Data;

namespace ShutdownTimer.Server.Extensions
{
	public static class ModelStateExtensions
	{
		public static void FromDescriptiveResult(this ModelStateDictionary source, DescriptiveResult result)
		{
			if (result.Succeeded)
				return;

			source.AddModelError(string.Empty, string.Join("</br>", result.Errors.Select(d => d.Description)));
		}
	}
}