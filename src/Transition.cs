using System;

namespace StateMachine {
    public class Transition {
        internal State next;
        internal Func<State, bool> ShouldTransition;

        internal Action OnTransition { get; private set; }

        public Transition(State next, Func<State, bool> condition, Action onTransition) {
            this.next = next;
            this.ShouldTransition = condition;
            this.OnTransition = onTransition;
        }

        public static Builder Build(State next) {
            return Builder.StartBuild(next);
        }

        /* --------------------------------- Builder -------------------------------- */
        public abstract class AbstractBuilder<TTran, TBuilder> : BaseAbstractBuilder<TTran, TBuilder>,
            AbstractBuilder<TTran, TBuilder>.IConditionStep,
            AbstractBuilder<TTran, TBuilder>.IOptionalStep
            where TTran : Transition
            where TBuilder : AbstractBuilder<TTran, TBuilder>, new() {
            // Fields
            protected State next;
            protected Func<State, bool> condition;
            protected Action onTransition = State.NO_EVENT;

            public static TBuilder StartBuild(State next) {
                return new TBuilder {
                    next = next
                };
            }

            public IOptionalStep If(Func<State, bool> condition) {
                this.condition = condition;
                return this;
            }

            public IOptionalStep After(float seconds) {
                this.condition = s => s.Time > seconds;
                return this;
            }

            public IOptionalStep OnTransition(Action onTransition) {
                this.onTransition = onTransition;
                return this;
            }

            /// <summary>
            /// Required step to build transition
            /// Next: If() - Regular condition for transition
            /// Next: After() - Timed condition for transition
            /// </summary>
            public interface IConditionStep {
                /// <summary>
                /// Set the condition callback for the transition
                /// </summary>
                /// <param name="condition">Condition callback</param>
                /// <returns>Next optional step or build step</returns>
                IOptionalStep If(Func<State, bool> condition);
                /// <summary>
                /// Set the time until the transition
                /// </summary>
                /// <param name="seconds">Time</param>
                /// <returns>Next optional step or build step</returns>
                IOptionalStep After(float seconds);
            }

            public interface IOptionalStep {
                IOptionalStep OnTransition(Action onTransition);
                TTran Build();
            }
        }

        public class Builder : AbstractBuilder<Transition, Builder> {
            public override Transition Build() {
                return new Transition(next, condition, onTransition);
            }
        }

    }
}