using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Bot.Builder.Community.FormFlow.Advanced;

namespace Bot.Builder.Community.FormFlow
{
    #region Documentation
    /// <summary>   Form definition interface. </summary>
    /// <typeparam name="T">    Form state. </typeparam>
    #endregion
    public abstract class IForm<T>
        where T : class
    {
        /// <summary>
        /// Fields that make up form.
        /// </summary>
        public abstract IFields<T> Fields { get; }

        #region Documentation
        /// <summary>   Save all string resources to binary stream for future localization. </summary>
        /// <param name="writer">   Where to write resources. </param>
        #endregion
        public abstract void SaveResources(IResourceWriter writer);

        #region Documentation
        /// <summary>   Localize all string resources from binary stream. </summary>
        /// <param name="reader">   Where to read resources. </param>
        /// <param name="missing">  [out] Any values in the form, but missing from the stream. </param>
        /// <param name="extra">    [out] Any values in the stream, but missing from the form. </param>
        /// <remarks>When you localize all form string resources will be overridden if present in the stream.
        ///          Otherwise the value will remain unchanged.
        /// </remarks>
        #endregion
        public abstract void Localize(IDictionaryEnumerator reader, out IEnumerable<string> missing, out IEnumerable<string> extra);

        // Internals
        internal abstract ILocalizer Resources { get; }
        internal abstract FormConfiguration Configuration { get; }
        internal abstract IReadOnlyList<IStep<T>> Steps { get; }
        internal abstract OnCompletionAsyncDelegate<T> Completion { get; }
        internal abstract Task<FormPrompt> Prompt(DialogContext context, FormPrompt prompt, T state, IField<T> field);
    }
}
