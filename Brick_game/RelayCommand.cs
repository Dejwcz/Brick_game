using System.Windows.Input;

namespace Brick_game {
    public class RelayCommand : ICommand {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// An event triggered when the state changes to determine whether a command can be executed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Creates a new command that can always be executed.
        /// </summary>
        /// <param name="execute">The action to be performed when the command is run.</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null) {
        }

        /// <summary>
        /// Creates a new command..
        /// </summary>
        /// <param name="execute">Action to be performed when the command is run.</param>
        /// <param name="canExecute">A function that determines whether a command can be executed..</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute) {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
            CommandManager.RequerySuggested += (s, e) => OnCanExecuteChanged();
        }
        protected virtual void OnCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Determines whether the command can currently be executed.
        /// </summary>
        /// <param name="parameter">Command parameter.</param>
        /// <returns>True if the command can be executed; otherwise false.</returns>
        public bool CanExecute(object parameter) {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Performs the command action.
        /// </summary>
        /// <param name="parameter">Command parameter.</param>
        public void Execute(object parameter) {
            _execute(parameter);
        }
    }
}
