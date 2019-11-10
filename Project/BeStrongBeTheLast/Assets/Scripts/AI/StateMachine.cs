/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Michele Maione
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    internal class Transition
    {
        internal State toState, fromState;
        internal Func<bool> condition;
        internal IEnumerator waiter;


        internal Transition(State fromState, State toState)
        {
            this.fromState = fromState;
            this.toState = toState;
        }

        internal Transition(State fromState, Func<bool> condition, State toState) : this(fromState, toState)
        {
            this.condition = condition;
        }

        internal Transition(State fromState, State toState, IEnumerator waiter) : this(fromState, toState)
        {
            this.waiter = waiter;
        }

        internal Transition(State fromState, State toState, Func<bool> endCondition) : this(fromState, toState, new WaitUntil(endCondition)) { }
    }

    internal class State
    {
        internal MonoBehaviour monoBehaviour;
        internal Delegate enterAction, exitAction;
        internal object[] enterParameters, exitParameters;

        internal HashSet<Transition> transitions;


        internal State(MonoBehaviour monoBehaviour)
        {
            this.monoBehaviour = monoBehaviour;
        }

        internal void addTransition(State fromState, State toState)
        {
            addTransition(new Transition(fromState, toState));
        }

        internal void addTransition(Transition t)
        {
            transitions.Add(t);
        }
    }

    internal class SM
    {
        MonoBehaviour monoBehaviour;

        State current;
        HashSet<State> states = new HashSet<State>();


        internal SM(MonoBehaviour monoBehaviour)
        {
            this.monoBehaviour = monoBehaviour;
        }

        internal void run(State to_)
        {
            run(to_, current);
        }

        void run(State to_, State from_)
        {
            var transactionToThisStateIsPossible = (from_ == null);

            if (!transactionToThisStateIsPossible)
                foreach (var t in from_.transitions)

                    if (t.toState == to_)
                    {
                        transactionToThisStateIsPossible = true;
                        break;
                    }

            if (transactionToThisStateIsPossible)
            {
                current = to_;

                if (current.enterAction != null)
                    current.enterAction.DynamicInvoke(current.enterParameters);

                foreach (var t in current.transitions)
                {
                    if (t.waiter == null)
                    {
                        if (t.condition != null)
                            if (t.condition())
                                run(t.toState);
                    }
                    else
                    {
                        var ge = new SMEnumerable(
                            t,
                            new Action<State, State>(run)
                        );

                        current.monoBehaviour.StartCoroutine(ge.GetEnumerator());
                    }
                }
            }
        }

        internal State addState()
        {
            var fromState = new State(monoBehaviour);

            fromState.transitions = new HashSet<Transition>();

            states.Add(fromState);

            return fromState;
        }

        internal State addState(Delegate enterAction)
        {
            var fromState = addState();

            fromState.enterAction = enterAction;

            return fromState;
        }

        internal State addState(Delegate enterAction, object[] enterParameters)
        {
            var fromState = addState(enterAction);

            fromState.enterParameters = enterParameters;

            return fromState;
        }

        internal State addState(string name, MonoBehaviour monoBehaviour, IEnumerator waiter, Delegate exitAction, State toState)
        {
            var fromState = addState();

            fromState.exitAction = exitAction;
            fromState.transitions.Add(new Transition(fromState, toState, waiter));

            return fromState;
        }

        internal State addState(Delegate enterAction, IEnumerator waiter, State toState)
        {
            var fromState = addState();

            fromState.enterAction = enterAction;
            fromState.transitions.Add(new Transition(fromState, toState, waiter));

            return fromState;
        }

        internal State addState(Delegate enterAction, Func<bool> endCondition, State toState)
        {
            var fromState = addState();

            fromState.enterAction = enterAction;
            fromState.transitions.Add(new Transition(fromState, toState, endCondition));

            return fromState;
        }

        internal State addState(Func<bool> endCondition, Delegate exitAction, State toState)
        {
            var fromState = addState();

            fromState.exitAction = exitAction;
            fromState.transitions.Add(new Transition(fromState, toState, endCondition));

            return fromState;
        }

        internal State addState(Func<bool> endCondition, State toState)
        {
            var fromState = addState();

            fromState.transitions.Add(new Transition(fromState, toState, endCondition));

            return fromState;
        }
    }


    sealed class SMEnumerator : IEnumerator
    {
        private Transition transition;
        private Delegate callback_function;


        public SMEnumerator(Transition transition, Delegate callback_function)
        {
            this.transition = transition;
            this.callback_function = callback_function;
        }

        public object Current => transition.waiter.Current;

        public bool MoveNext()
        {
            var fromState = transition.fromState;

            var more_elements_available = transition.waiter.MoveNext();

            if (!more_elements_available)
            {
                if (fromState.exitAction != null)
                    fromState.exitAction.DynamicInvoke(fromState.exitParameters);

                var next_state = transition.toState;

                if (callback_function != null)
                    callback_function.DynamicInvoke(next_state, fromState);
            }

            return more_elements_available;
        }

        public void Reset()
        {
            transition.waiter.Reset();
        }
    }

    sealed class SMEnumerable : IEnumerable
    {
        private SMEnumerator smEnumerator;

        public SMEnumerable(Transition transition, Delegate callback_function)
        {
            this.smEnumerator = new SMEnumerator(transition, callback_function);
        }

        public IEnumerator GetEnumerator()
        {
            return smEnumerator;
        }
    }

}