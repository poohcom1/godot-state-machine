# Godot C# State Machine
Simple, easy to use lightweight state machine for Godot or other games using C#. 
There are no state machine parent class; states are all maintained as individual instances which can be changed at each update cycle.

## Usage
1. In the object you want to turn into a state machine, create a field called currentState or something similar:
```csharp
using StateMachine;

public class StateMachineLike 
{
  State currentState;
  ...
}
```

2. Initialize all the required states in the constructor using the State's public constructors or `State.Builder`.

```csharp
State idle = State.Builder
    .Create("Idle") // Pass a string to name the state. Useful for storing sprite/animation names
    .OnPerform(delta => {
      // Action on idle
    })
    .OnEntry(() => {
      // Action when the state started
    })
    .Build();
```

3. Initialize all transitions using the contructor or `Transition.Builder`.

4. Connect states using `State.SetTransitions()`

5. Set `currentState` to the initial state

6. In the Update function, do:
```csharp
currentState = currentState.PerformAndTransition(delta); 
// Delta is the time since last frame; it's only used in each state's own Action functions. 
// Set it to some random number if you don't need it.
```
Perform and Transition simplies perform the Action of the current state, and check all of its transition for their respective conditions. 
It then returns a new state if a condition has been met, or the same state.

7. Voilà! You've got yourself a state machine that performs an action on each Update frame, and can transition based on conditions in each transition object.

## Idea

### State
States are basically wrapper for an Action function, with string data for Name and Sprite, and two optional actions for Entry and Exit. 
They hold a list of transitions, which are ways of connecting states to other states.

### Transitions
Transitions are just a wrapper for a Func that takes a state as a param and returns a bool. They also hold a single state object which represents the state to transition to.
If you imagine a state machine like a graph, a transition is the line connecting to another state, just with an extra connection to the destination state. 
