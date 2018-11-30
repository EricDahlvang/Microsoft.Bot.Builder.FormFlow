using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Bot.Builder.Community.FormFlow.Advanced
{
    public enum StepPhase { Ready, Responding, Completed };
	public enum StepType { Field, Confirm, Navigation, Message };

	public struct StepResult
    {
        internal StepResult(bool success, NextStep next, FormPrompt feedback, FormPrompt prompt)
        {
            this.Success = success;
            this.Next = next;
            this.Feedback = feedback;
            this.Prompt = prompt;
        }

        internal NextStep Next { get; set; }
        internal FormPrompt Feedback { get; set; }
        internal FormPrompt Prompt { get; set; }
        internal bool Success { get; set; }
    }

    internal interface IStep<T>
        where T : class
    {
        string Name { get; }

        StepType Type { get; }

        TemplateBaseAttribute Annotation { get; }

        IField<T> Field { get; }

        void SaveResources();

        void Localize();

        bool Active(T state);

        Task<bool> DefineAsync(T state);

        FormPrompt Start(DialogContext context, T state, FormState form);

        bool InClarify(FormState form);

        IEnumerable<TermMatch> Match(DialogContext context, T state, FormState form, IMessageActivity input);

        Task<StepResult> ProcessAsync(DialogContext context, T state, FormState form, IMessageActivity input, IEnumerable<TermMatch> matches);

        FormPrompt NotUnderstood(DialogContext context, T state, FormState form, IMessageActivity input);

        FormPrompt Help(T state, FormState form, string commandHelp);

        bool Back(DialogContext context, T state, FormState form);

        IEnumerable<string> Dependencies { get; }
    }

}
