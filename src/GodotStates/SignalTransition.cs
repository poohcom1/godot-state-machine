namespace StateMachine.GodotStates {
    public class SignalTransition : Transition {
        readonly protected Receiver receiver;
        protected string signal;

        protected SignalTransition(State next, string signal, Godot.Object sender) : base(next, s => false, State.NO_EVENT) {
            this.signal = signal;
            this.receiver = new Receiver();

            ShouldTransition = s => {
                if (receiver.SignalReceived) {
                    receiver.SignalReceived = false;
                    return true;
                }
                return false;
            };

            sender.Connect(signal, receiver, nameof(receiver.OnSignalReceived));
        }

        protected sealed class Receiver : Godot.Object {
            public bool SignalReceived;

            public void OnSignalReceived() {
                SignalReceived = true;
            }
        }
    }
}