using System;
using System.Collections.Generic;

using Bot.Builder.Community.FormFlow.Advanced;

namespace Bot.Builder.Community.FormFlow
{
    [Serializable]
    public class FormState
    {
        // Last sent prompt which is used when feedback is supplied
        public FormPrompt LastPrompt;

        // Used when navigating to reflect choices for next
        public NextStep Next;

        // Currently executing step
        public int Step;

        // History of executed steps
        public Stack<int> History;

        // Current phase of each step
        public StepPhase[] Phases;

        // Internal state of a step
        public object StepState;

        // Field number and input
        public List<Tuple<int, string>> FieldInputs;

        // True when we have started processing FieldInputs
        public bool ProcessInputs;

        public FormState(int steps)
        {
            Phases = new StepPhase[steps];
            Reset();
        }

        public void Reset()
        {
            LastPrompt = new FormPrompt();
            Next = null;
            Step = 0;
            History = new Stack<int>();
            Phases = new StepPhase[Phases.Length];
            StepState = null;
            FieldInputs = null;
            ProcessInputs = false;
        }

        public StepPhase Phase()
        {
            return Phases[Step];
        }

        public StepPhase Phase(int step)
        {
            return Phases[step];
        }

        public void SetPhase(StepPhase phase)
        {
            Phases[Step] = phase;
        }

        public void SetPhase(int step, StepPhase phase)
        {
            Phases[step] = phase;
        }
    }
}
