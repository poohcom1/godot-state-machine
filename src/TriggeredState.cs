using System;

namespace StateMachine {
    /// <summary>
    /// A state with an incoming transition attached.
    /// Can be implicitly converted to a Transition.
    /// </summary>
    public sealed class TriggeredState : State {
        /// <summary>
        /// Incoming transition attached to the state
        /// </summary>
        /// <value>Transition</value>
        public Transition Trigger { internal set; get; }

        public TriggeredState(string name, string sprite, Action<float> action, Action onEntry, Action onExit, Func<State, bool> trigger)
            : base(name, sprite, action, onEntry, onExit) {
            Trigger = new Transition(this, trigger, NO_EVENT);
        }

        public new class Builder : AbstractBuilder<TriggeredState, Builder> {
            private Func<State, bool> Trigger;

            public Builder SetTrigger(Func<State, bool> condition) {
                Trigger = condition;
                return Self;
            }

            public override TriggeredState Build() {
                return new TriggeredState(Name, Sprite, Action, EntryAction, ExitAction, Trigger);
            }
        }

        public static implicit operator Transition(TriggeredState triggeredState) {
            return triggeredState.Trigger;
        }
    }
}