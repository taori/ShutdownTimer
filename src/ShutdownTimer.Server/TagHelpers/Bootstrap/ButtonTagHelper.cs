using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TagHelperSamples.Authorization;

namespace ShutdownTimer.Server.TagHelpers.Bootstrap
{
	public enum FeedbackTheme
	{
		None,
		Primary,
		Secondary,
		Success,
		Danger,
		Warning,
		Info,
		Light,
		Dark,
		Link
	}

	public enum ButtonSize
	{
		Small,
		Medium,
		Large
	}

	public enum ListGroupHorizontalWidth
	{
		FullWidth,
		Small,
		Medium,
		Large,
		ExtraLarge
	}

	public enum ColumnWidth
	{
		Custom,
		ExtraSmall,
		Small,
		Medium,
		Large,
		ExtraLarge
	}

	[HtmlTargetElement(Attributes = "bs-listGroup")]
	public class ListGroupTagHelper : TagHelper
	{
		[HtmlAttributeName("bs-border")]
		public bool HideBorder { get; set; }

		[HtmlAttributeName("bs-horizontal")]
		public bool Horizontal { get; set; }

		[HtmlAttributeName("bs-horizontalWidth")]
		public ListGroupHorizontalWidth HorizontalWidth { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.AddClass("list-group", HtmlEncoder.Default);

			if(HideBorder)
				output.AddClass("list-group-flush", HtmlEncoder.Default);

			if (Horizontal)
			{
				switch (HorizontalWidth)
				{
					case ListGroupHorizontalWidth.FullWidth:
						output.AddClass("list-group-horizontal", HtmlEncoder.Default);
						break;
					case ListGroupHorizontalWidth.Small:
						output.AddClass("list-group-horizontal-sm", HtmlEncoder.Default);
						break;
					case ListGroupHorizontalWidth.Medium:
						output.AddClass("list-group-horizontal-md", HtmlEncoder.Default);
						break;
					case ListGroupHorizontalWidth.Large:
						output.AddClass("list-group-horizontal-lg", HtmlEncoder.Default);
						break;
					case ListGroupHorizontalWidth.ExtraLarge:
						output.AddClass("list-group-horizontal-xl", HtmlEncoder.Default);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			if (output.Attributes.TryGetAttribute("bs-listGroup", out var attribute))
				output.Attributes.Remove(attribute);

			base.Process(context, output);
		}
	}

	[HtmlTargetElement(Attributes = "bs-listGroupItem")]
	public class ListGroupItemTagHelper : TagHelper
	{
		[HtmlAttributeName("bs-active")]
		public bool Active { get; set; }
		
		[HtmlAttributeName("bs-disabled")]
		public bool Disabled { get; set; }
		
		[HtmlAttributeName("bs-expandAsButton")]
		public bool ExpandAsButton { get; set; }
		
		[HtmlAttributeName("bs-fillHorizontalWidth")]
		public bool FillWidth { get; set; }
		
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.AddClass("list-group-item", HtmlEncoder.Default);

			if (FillWidth)
				output.AddClass("flex-fill", HtmlEncoder.Default);

			if (Active)
				output.AddClass("active", HtmlEncoder.Default);

			if (ExpandAsButton && (output.TagName == "a" || output.TagName == "button"))
				output.AddClass("list-group-item-action", HtmlEncoder.Default);

			if (Disabled)
			{
				output.Attributes.Add("aria-disabled", "true");
				output.AddClass("disabled", HtmlEncoder.Default);
			}

			if (output.Attributes.TryGetAttribute("bs-listGroupItem", out var attribute))
				output.Attributes.Remove(attribute);

			base.Process(context, output);
		}
	}

	// https://getbootstrap.com/docs/4.3/components/buttons/
	[HtmlTargetElement("button")]
	[HtmlTargetElement("a")]
	[HtmlTargetElement("input", Attributes = "[type=submit]", TagStructure = TagStructure.WithoutEndTag)]
	[HtmlTargetElement("input", Attributes = "[type=reset]", TagStructure = TagStructure.WithoutEndTag)]
	[HtmlTargetElement("input", Attributes = "[type=button]", TagStructure = TagStructure.WithoutEndTag)]
	public class ButtonTagHelper : TagHelper
	{
		[HtmlAttributeName("bs-type")]
		public FeedbackTheme Type { get; set; }

		[HtmlAttributeName("bs-outline")]
		public bool Outline { get; set; }

		[HtmlAttributeName("bs-fullWidth")]
		public bool FullWidth { get; set; }

		[HtmlAttributeName("bs-disabled")]
		public bool Disabled { get; set; }

		[HtmlAttributeName("bs-size")]
		public ButtonSize Size { get; set; } = ButtonSize.Medium;

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if (Type != FeedbackTheme.None)
			{
				output.AddClass("btn", HtmlEncoder.Default);
				output.AddClass(GetTypeClass(Type, Outline), HtmlEncoder.Default);
				switch (Size)
				{
					case ButtonSize.Small:
						output.AddClass("btn-sm", HtmlEncoder.Default);
						break;
					case ButtonSize.Medium:
						break;
					case ButtonSize.Large:
						output.AddClass("btn-lg", HtmlEncoder.Default);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				if (FullWidth)
					output.AddClass("btn-block", HtmlEncoder.Default);

				if (Disabled)
				{
					if (output.TagName == "a")
					{
						output.AddClass("disabled", HtmlEncoder.Default);
					}
					else
					{
						output.Attributes.Add("disabled", "disabled");
					}

					output.Attributes.Add("aria-disabled", "true");
				}
			}

			base.Process(context, output);
		}

		private static string GetTypeClass(FeedbackTheme type, bool outline)
		{
			if (outline)
			{
				switch (type)
				{
					case FeedbackTheme.Primary:
						return "btn-outline-primary";
					case FeedbackTheme.Secondary:
						return "btn-outline-secondary";
					case FeedbackTheme.Success:
						return "btn-outline-success";
					case FeedbackTheme.Danger:
						return "btn-outline-danger";
					case FeedbackTheme.Warning:
						return "btn-outline-warning";
					case FeedbackTheme.Info:
						return "btn-outline-info";
					case FeedbackTheme.Light:
						return "btn-outline-light";
					case FeedbackTheme.Dark:
						return "btn-outline-dark";
					case FeedbackTheme.Link:
						return "btn-outline-link";
					default:
						throw new ArgumentOutOfRangeException(nameof(type), type, null);
				}
			}
			else
			{
				switch (type)
				{
					case FeedbackTheme.Primary:
						return "btn-primary";
					case FeedbackTheme.Secondary:
						return "btn-secondary";
					case FeedbackTheme.Success:
						return "btn-success";
					case FeedbackTheme.Danger:
						return "btn-danger";
					case FeedbackTheme.Warning:
						return "btn-warning";
					case FeedbackTheme.Info:
						return "btn-info";
					case FeedbackTheme.Light:
						return "btn-light";
					case FeedbackTheme.Dark:
						return "btn-dark";
					case FeedbackTheme.Link:
						return "btn-link";
					default:
						throw new ArgumentOutOfRangeException(nameof(type), type, null);
				}
			}
		}
	}
}