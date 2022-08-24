using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace SuperJMNTest.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            NombreCompletoTextBox = this.WhenAnyValue(x => x.NombreTextBox, x => x.ApellidoTextBox,
                (a, b) => string.Join(" ", a, b));

            IsValid = this.WhenAnyValue(x => x.NombreTextBox, x => x.ApellidoTextBox, x => x.EdadTextBox,
                (a, b, c) => IsFulfilled(a, b) && c >= 18);
            
            Submit = ReactiveCommand.Create(() => { /* the command itself does nothing */ }, IsValid);

            IsSubmitted = Submit
                .Any()
                .StartWith(false);
        }

        private static bool IsFulfilled(params string[] strings)
        {
            return !strings.Any(string.IsNullOrEmpty);
        }

        public IObservable<bool> IsSubmitted { get; }

        public ReactiveCommand<Unit, Unit> Submit { get; }

        public IObservable<string> NombreCompletoTextBox { get; }

        public IObservable<bool> IsValid { get; }

        [Reactive] public string NombreTextBox { get; set; }

        [Reactive] public string ApellidoTextBox { get; set; }

        [Reactive] public int EdadTextBox { get; set; }
    }
}