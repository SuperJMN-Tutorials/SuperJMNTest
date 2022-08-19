using ReactiveUI;
using SuperJMNTest.Models;
using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace SuperJMNTest.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public IObservable<string> NombreCompletoTextBox { get; }
        public IObservable<bool> IsButtonEnabled { get; }
        public IObservable<bool> IsFormCompleted { get; }
        //private string NombreCompletoTextBox { get; set; }
        //private bool IsButtonEnabled { get; set; }
        private bool _isFormCompletedField { get; set; }
        public bool IsFormCompletedField
        {
            get => _isFormCompletedField;
            set
            {
                _isFormCompletedField = value;
                this.RaisePropertyChanged(nameof(IsFormCompletedField));
            }
        }

        private string _nombreTextBox = "";
        public string NombreTextBox
        {
            get => _nombreTextBox;
            set
            {
                _nombreTextBox = value;
                this.RaisePropertyChanged(nameof(NombreTextBox));
            }
        }
        private string _apellidoTextBox = "";
        public string ApellidoTextBox
        {
            get => _apellidoTextBox;
            set
            {
                _apellidoTextBox = value;
                this.RaisePropertyChanged(nameof(ApellidoTextBox));
            }
        }
        private string _edadTextBox = "";
        public string EdadTextBox
        {
            get => _edadTextBox;
            set
            {
                _edadTextBox = value;
                this.RaisePropertyChanged(nameof(EdadTextBox));
            }
        }

        public MainWindowViewModel()
        {
            //this.WhenAnyValue(x => x.NombreTextBox, x => x.ApellidoTextBox)
            //.Subscribe(tuple =>
            //{
            //    var (nombreTextBox, apellidoTextBox) = tuple;
            //    NombreCompletoTextBox = nombreTextBox + " " + apellidoTextBox;
            //    Trace.WriteLine("test: " + NombreCompletoTextBox);
            //});

            //this.WhenAnyValue(x => x.NombreTextBox, x => x.ApellidoTextBox, x => x.EdadTextBox)
            //.Subscribe(tuple =>
            //{
            //    var (nombreTextBox, apellidoTextBox, edadTextBox) = tuple;
            //    IsButtonEnabled = (nombreTextBox ?? "") != "" && (apellidoTextBox ?? "") != "" && (edadTextBox ?? "") != "";
            //    Trace.WriteLine("test: " + IsButtonEnabled.ToString());
            //});

            NombreCompletoTextBox =
            this.WhenAnyValue(x => x.NombreTextBox, x => x.ApellidoTextBox)
                .Select(tuple =>
                {
                    var (nombreTextBox, apellidoTextBox) = tuple;
                    Trace.WriteLine("OK:" + tuple.ToString());
                    return nombreTextBox + " " + apellidoTextBox;
                });

            IsButtonEnabled =
            this.WhenAnyValue(x => x.NombreTextBox, x => x.ApellidoTextBox, x => x.EdadTextBox)
                .Select(tuple =>
                {
                    var (nombreTextBox, apellidoTextBox, edadTextBox) = tuple;
                    return (nombreTextBox ?? "") != "" && (apellidoTextBox ?? "") != "" && (edadTextBox ?? "") != "";
                });

            IsFormCompleted =
            this.WhenAnyValue(x => x.IsFormCompletedField)
                .Select(x =>
                {
                    return x;
                });
        }
        public void OnClickCommand()
        {
            IsFormCompletedField = true;
        }

    }
}