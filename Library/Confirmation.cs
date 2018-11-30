using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Builder.Community.FormFlow.Advanced
{
    /// <summary>
    /// Confirmation 
    /// </summary>
    /// <typeparam name="T">Form state.</typeparam>
    public class Confirmation<T> : Field<T>
        where T : class
    {
        /// <summary>
        /// Construct a confirmation.
        /// </summary>
        /// <param name="prompt">Confirmation prompt expressed using \ref patterns.</param>
        /// <param name="condition">Delegate for whether confirmation applies.</param>
        /// <param name="dependencies">Fields that must have values before confirmation can run.</param>
        /// <param name="form">Form that contains confirmation.</param>
        public Confirmation(PromptAttribute prompt, ActiveDelegate<T> condition, IEnumerable<string> dependencies, IForm<T> form)
            : base("confirmation" + form.Steps.Count, FieldRole.Confirm)
        {
            SetPrompt(prompt);
            SetType(typeof(bool));
            SetDependencies(dependencies.ToArray());
            SetActive(condition);
            SetFieldDescription(new DescribeAttribute(form.Configuration.Confirmation) { IsLocalizable = false });
            var noStep = (dependencies.Any() ? new NextStep(dependencies) : new NextStep());
            _next = (value, state) => (bool)value ? new NextStep() : noStep;
        }

        /// <summary>
        /// Construct a confirmation dynamically.
        /// </summary>
        /// <param name="generateMessage">Delegate for building confirmation.</param>
        /// <param name="condition">Delegate to see if confirmation is active.</param>
        /// <param name="dependencies">Fields that must have values before confirmation can run.</param>
        /// <param name="form">Form that contains confirmation.</param>
        public Confirmation(MessageDelegate<T> generateMessage, ActiveDelegate<T> condition, IEnumerable<string> dependencies, IForm<T> form)
            : base("confirmation" + form.Steps.Count, FieldRole.Confirm)
        {
            SetDefine(async (state, field) => { field.SetPrompt(await generateMessage(state)); return true; });
            SetType(typeof(bool));
            SetDependencies(dependencies.ToArray());
            SetActive(condition);
            SetFieldDescription(new DescribeAttribute(form.Configuration.Confirmation) { IsLocalizable = false });
            var noStep = (dependencies.Any() ? new NextStep(dependencies) : new NextStep());
            SetNext((value, state) => (bool)value ? new NextStep() : noStep);
        }

        public override object GetValue(T state)
        {
            return null;
        }

        public override IEnumerable<string> Dependencies
        {
            get
            {
                return _dependencies;
            }
        }

        #region IFieldPrompt
        public override bool Active(T state)
        {
            return _condition(state);
        }

        public override NextStep Next(object value, T state)
        {
            return _next((bool)value, state);
        }

        public override void SetValue(T state, object value)
        {
            throw new NotImplementedException();
        }

        public override bool IsUnknown(T state)
        {
            return true;
        }

        public override void SetUnknown(T state)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
