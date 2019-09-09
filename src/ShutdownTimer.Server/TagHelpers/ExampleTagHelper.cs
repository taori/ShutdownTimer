using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ShutdownTimer.Server.TagHelpers
{
	public class ExampleTagHelper : TagHelper
	{
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "input";
		}
	}
}