using System;
using System.Collections.Generic;

namespace StateMachine {
    /// <summary>
    /// A basic action wrapper.
    /// Use a "currentState" State variable, and call the PerformAndTransition at each update frame while setting
    /// tcurrentState to the return of that method to convert the holding class into a state machine.
    /// </summary>
    public class State {
        // Statics
        public static Action<float> NO_ACTION = delta => { };
        public static Action NO_EVENT = () => { };

        // Public fields
        /// <summary>Name of state</summary>
        public string Name;
        /// <summary>Name of sprite</summary>
        public string Sprite;

        /// <summary>Time in seconds since the state has entered</summary>
        public float Time = 0;
        /// <summary>Number of frames that the state has performed since the state has entered</summary>
        public int Frame = 0;

        // State fields
        private readonly List<Action<float>> actions = new List<Action<float>>();
        private List<Transition> transitions = new List<Transition>();

        /// <summary>Entry action</summary>
        protected Action OnEntry;
        /// <summary>Exit action</summary>
        protected Action OnExit;

        /// <summary>
        /// Creates a state
        /// </summary>
        /// <param name="name">Name string</param>
        /// <param name="sprite">Sprite string</param>
        /// <param name="action">Action on each frame</param>
        /// <param name="entryAction">Action when the state starts</param>
        /// <param name="exitAction">Action when the state exits</param>
        public State(string name, string sprite, Action<float> action, Action entryAction, Action exitAction) {
            Name = name;
            Sprite = sprite;
            if (action != NO_ACTION) actions.Add(action);
            OnEntry = entryAction;
            OnExit = exitAction;
        }

        /// <summary>
        /// Adds an action to the state
        /// </summary>
        /// <param name="action">Action to add</param>
        public void AttachAction(Action<float> action) {
            this.actions.Add(action);
        }

        /// <summary>
        /// Clears all transition and adds new ones
        /// </summary>
        /// <param name="transitions">Transitions to add</param>
        public void SetTransitions(params Transition[] transitions) {
            this.transitions = new List<Transition>(transitions);
        }

        public void ClearTransitions() {
            this.transitions.Clear();
        }

        public void AddTransitions(params Transition[] transitions) {
            this.transitions.AddRange(transitions);
        }
        /// <summary>
        /// Action of the state to perform, and check transitional states.
        /// </summary>
        /// <param name="delta">Delta time</param>
        /// <returns>State to transition to</returns>
        public State PerformAndTransition(float delta) {
            Time += delta;
            Frame++;
            // Perform action if there are no state changes
            actions.ForEach(action => action(delta));

            // Check for states to change to
            foreach (Transition transition in transitions) {
                if (transition.ShouldTransition(this)) {
                    Time = 0; // Reset time
                    Frame = 0;

                    OnExit(); // Current action's exit
                    transition.OnTransition(); // Transition's action
                    transition.next.OnEntry(); // Next action's entry
                    return transition.next;
                }
            }

            // Return self as state hasn't changed
            return this;
        }

        /* ----------------------- Inbuilt transition builder ----------------------- */

        /// <summary>
        /// Attaches a new transition to the state.
        /// Build() must be called for the transition to be attached to the state.
        /// </summary>
        /// <param name="state">State to transition to</param>
        public Transition.Builder To(State state) {
            return new StateTransitionBuilder(this, state);
        }

        private class StateTransitionBuilder : Transition.Builder {
            private readonly State state;

            public StateTransitionBuilder(State state, State next) {
                this.state = state;
                this.next = next;
            }

            /// <summary>
            /// Commits the transition
            /// </summary>
            /// <returns>The new transition</returns>
            public override Transition Build() {
                Transition transition = base.Build();
                state.AddTransitions(transition);

                return transition;
            }
        }

        /// <summary>
        /// Initiate state builder
        /// </summary>
        /// <param name="name">Name of state and sprite</param>
        /// <returns>State builder</returns>
        public static Builder Build(string name) {
            return Builder.Create(name);
        }




        /// <summary>
        /// Abstract builder for inheriting states.
        /// Name, Sprite, Action, EntryAction, ExitAction are provided.
        /// TState Build() must be implemented with the built State subclass as the return type.
        /// </summary>
        /// <typeparam name="TState">State being built</typeparam>
        /// <typeparam name="TBuilder">Reflecting builder</typeparam>
        public abstract class AbstractBuilder<TState, TBuilder> : BaseAbstractBuilder<TState, TBuilder>
            where TState : State
            where TBuilder : AbstractBuilder<TState, TBuilder>, new() {

            // Fields
            protected string Name;
            protected string Sprite;
            protected Action<float> Action = NO_ACTION;
            protected Action EntryAction = NO_EVENT;
            protected Action ExitAction = NO_EVENT;

            public static TBuilder Create(string name) {
                return new TBuilder {
                    Name = name,
                    Sprite = name
                };
            }

            /// <summary>
            /// Optional: Set the Sprite to a different string than Name
            /// </summary>
            /// <param name="spriteName">Sprite name</param>
            /// <returns>State builder</returns>
            public TBuilder SetSprite(string spriteName) {
                Sprite = spriteName;
                return Self;
            }

            /// <summary>
            /// Sets the default action of the state
            /// </summary>
            /// <param name="action">State action</param>
            /// <returns>State builder</returns>
            public TBuilder OnPerform(Action<float> action) {
                this.Action = action;
                return Self;
            }

            /// <summary>
            /// Sets the action to perform when the state starts
            /// </summary>
            /// <param name="action">On entry action</param>
            /// <returns>State builder</returns>
            public TBuilder OnEntry(Action action) {
                EntryAction = action;
                return Self;
            }

            /// <summary>
            /// Sets the action to perform when the state ends
            /// </summary>
            /// <param name="action">On exit action</param>
            /// <returns>State builder</returns>
            public TBuilder OnExit(Action action) {
                ExitAction = action;
                return Self;
            }

            public static implicit operator AbstractBuilder<TState, TBuilder>(TState state) {
                return new TBuilder();
            }
        }

        /// <summary>
        /// Builder for the base State class.
        /// To make a custom builder for inherited class, extend State.AbstractBuilder
        /// </summary>
        public class Builder : AbstractBuilder<State, Builder> {
            /// <summary>
            /// Builds the state
            /// </summary>
            /// <returns>State</returns>
            public override State Build() {
                return new State(Name, Sprite, Action, EntryAction, ExitAction);
            }
        }
    }
}

