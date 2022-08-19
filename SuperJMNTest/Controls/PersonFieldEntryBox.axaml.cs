using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using ReactiveUI;

namespace SuperJMNTest.Controls
{

	public class PersonFieldEntryBox : TextBox
	{
		public static readonly StyledProperty<string> LabelFieldProperty =
			AvaloniaProperty.Register<PersonFieldEntryBox, string>(nameof(LabelField), "");

		public static readonly StyledProperty<string> LabelWatermarkProperty =
			AvaloniaProperty.Register<PersonFieldEntryBox, string>(nameof(LabelWatermark), "");

		public static readonly StyledProperty<int> MinLettersProperty =
			AvaloniaProperty.Register<PersonFieldEntryBox, int>(nameof(MinLetters), 3);

		public static readonly StyledProperty<int> MaxLettersProperty =
			AvaloniaProperty.Register<PersonFieldEntryBox, int>(nameof(MaxLetters), 12);

		public static readonly StyledProperty<bool> EnforceCapsProperty =
			AvaloniaProperty.Register<PersonFieldEntryBox, bool>(nameof(EnforceCaps), true);

		public static readonly StyledProperty<int> MinAgeProperty =
			AvaloniaProperty.Register<PersonFieldEntryBox, int>(nameof(MinAge), 1);

		public static readonly StyledProperty<int> MaxAgeProperty =
			AvaloniaProperty.Register<PersonFieldEntryBox, int>(nameof(MaxAge), 99);

		private readonly Regex _regexIsLettersOptionalHyphen;
		private readonly Regex _regexIsOnlyDigits;
		private readonly Regex _regexAreCapsCorrect;

		public PersonFieldEntryBox()
		{
			Text = string.Empty;

			_regexIsOnlyDigits =
				new Regex(
					$"^[0-9]+$", RegexOptions.Compiled);
			_regexIsLettersOptionalHyphen =
				new Regex(
					$@"^[A-Za-z]+.(\-.[A-Za-z]+)*$", RegexOptions.Compiled);
			_regexAreCapsCorrect =
				new Regex(
					$@"^[A-Z].[a-z]+.(\-.[A-Z])?.[a-z]+$", RegexOptions.Compiled);

			ModifiedPaste = ReactiveCommand.Create(ModifiedPasteAsync, this.GetObservable(CanPasteProperty));
		}

		public ICommand ModifiedPaste { get; }

		public string LabelField
		{
			get => GetValue(LabelFieldProperty);
			set => SetValue(LabelFieldProperty, value);
		}

		public string LabelWatermark
		{
			get => GetValue(LabelWatermarkProperty);
			set => SetValue(LabelWatermarkProperty, value);
		}
		public int MinLetters
		{
			get => GetValue(MinLettersProperty);
			set => SetValue(MinLettersProperty, value);
		}

		public int MaxLetters
		{
			get => GetValue(MaxLettersProperty);
			set => SetValue(MaxLettersProperty, value);
		}

		public bool EnforceCaps
		{
			get => GetValue(EnforceCapsProperty);
			set => SetValue(EnforceCapsProperty, value);
		}

		public int MinAge
		{
			get => GetValue(MinAgeProperty);
			set => SetValue(MinAgeProperty, value);
		}

		public int MaxAge
		{
			get => GetValue(MaxAgeProperty);
			set => SetValue(MaxAgeProperty, value);
		}

		protected override void OnGotFocus(GotFocusEventArgs e)
		{
			base.OnGotFocus(e);

			CaretIndex = Text?.Length ?? 0;

			Dispatcher.UIThread.Post(SelectAll);
		}

		protected override void OnTextInput(TextInputEventArgs e)
		{
			var input = e.Text ?? "";

			var preComposedText = PreComposeText(input);


			e.Handled = !ValidateEntryText(preComposedText);

			base.OnTextInput(e);
		}

		private bool ValidateEntryText(string preComposedText)
		{
			if (LabelField == "Edad")
            {
                if (!_regexIsOnlyDigits.IsMatch(preComposedText))
				{
					return false;
                }
				if (!int.TryParse(preComposedText, out int edad)) 
				{
					return false;
				}
				if(edad < MinAge || edad > MaxAge)
				{
					return false;
                }
            }
            else
            {
				if (EnforceCaps ? !(_regexAreCapsCorrect.IsMatch(preComposedText))
								: !(_regexIsLettersOptionalHyphen.IsMatch(preComposedText)))
                {
					return false;
                }
            }
			return true;
		}

		public async void ModifiedPasteAsync()
		{
			if (AvaloniaLocator.Current.GetService<IClipboard>() is { } clipboard)
			{
				var text = await clipboard.GetTextAsync();

				if (string.IsNullOrEmpty(text))
				{
					return;
				}

				text = text.Replace("\r", "").Replace("\n", "").Trim();

				if (ValidateEntryText(text))
				{
					OnTextInput(new TextInputEventArgs { Text = text });
				}
			}
		}

		// Pre-composes the TextInputEventArgs to see the potential Text that is to
		// be committed to the TextPresenter in this control.

		// An event in Avalonia's TextBox with this function should be implemented there for brevity.
		private string PreComposeText(string input)
		{
			input = RemoveInvalidCharacters(input);
			var preComposedText = Text ?? "";
			var caretIndex = CaretIndex;
			var selectionStart = SelectionStart;
			var selectionEnd = SelectionEnd;

			if (!string.IsNullOrEmpty(input) && (MaxLength == 0 ||
												 input.Length + preComposedText.Length -
												 Math.Abs(selectionStart - selectionEnd) <= MaxLength))
			{
				if (selectionStart != selectionEnd)
				{
					var start = Math.Min(selectionStart, selectionEnd);
					var end = Math.Max(selectionStart, selectionEnd);
					preComposedText = $"{preComposedText[..start]}{preComposedText[end..]}";
					caretIndex = start;
				}

				return $"{preComposedText[..caretIndex]}{input}{preComposedText[caretIndex..]}";
			}

			return "";
		}

		protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
		{
			base.OnPropertyChanged(change);
		}
	}
}