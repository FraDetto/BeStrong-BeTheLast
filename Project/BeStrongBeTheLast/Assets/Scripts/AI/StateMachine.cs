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
        internal State _toState, _fromState;
        internal Func<bool> _condition;
        internal IEnumerator _waiter;


        internal Transition(State fromState, State toState)
        {
            _fromState = fromState;
            _toState = toState;
        }

        internal Transition(State fromState, Func<bool> condition, State toState) : this(fromState, toState)
        {
            _condition = condition;
        }

        internal Transition(State fromState, State toState, IEnumerator waiter) : this(fromState, toState)
        {
            _waiter = waiter;
        }

        internal Transition(State fromState, State toState, Func<bool> endCondition) : this(fromState, toState, new WaitUntil(endCondition)) { }
    }

    internal class State
    {
        internal PausableMonoBehaviour _monoBehaviour;
        internal Delegate _enterAction, _exitAction;
        internal object[] _enterParameters, _exitParameters;

        internal HashSet<Transition> transitions;


        internal State(PausableMonoBehaviour monoBehaviour)
        {
            _monoBehaviour = monoBehaviour;
        }

        internal void addTransition(Transition t)
        {
            transitions.Add(t);
        }

        internal void addTransition(State fromState, State toState)
        {
            addTransition(new Transition(fromState, toState));
        }

        internal void addTransitionFromMe(State toState)
        {
            addTransition(new Transition(this, toState));
        }

        internal void addTransitionToMe(State fromState)
        {
            addTransition(new Transition(fromState, this));
        }
    }

    internal class SM
    {
        PausableMonoBehaviour _monoBehaviour;
        State _current;
        HashSet<State> _states = new HashSet<State>();


        internal SM(PausableMonoBehaviour monoBehaviour)
        {
            _monoBehaviour = monoBehaviour;
        }

        internal void run(State to_)
        {
            run(to_, _current);
        }

        void run(State to_, State from_)
        {
            var transactionToThisStateIsPossible = (from_ == null);

            if (!transactionToThisStateIsPossible)
                foreach (var t in from_.transitions)
                    if (t._toState == to_)
                    {
                        transactionToThisStateIsPossible = true;
                        break;
                    }

            if (transactionToThisStateIsPossible)
            {
                _current = to_;

                if (_current._enterAction != null)
                    _current._enterAction.DynamicInvoke(_current._enterParameters);

                foreach (var t in _current.transitions)
                    if (t._waiter == null)
                    {
                        if (t._condition != null)
                            if (t._condition())
                                run(t._toState);
                    }
                    else
                    {
                        var ge = new SMEnumerable(
                            t,
                            new Action<State, State>(run)
                        );

                        _current._monoBehaviour.StartCoroutine(ge.GetEnumerator());
                    }
            }
        }

        internal State addState()
        {
            var fromState = new State(_monoBehaviour);

            fromState.transitions = new HashSet<Transition>();

            _states.Add(fromState);

            return fromState;
        }

        internal State addState(Delegate enterAction)
        {
            var fromState = addState();

            fromState._enterAction = enterAction;

            return fromState;
        }

        internal State addState(Delegate enterAction, object[] enterParameters)
        {
            var fromState = addState(enterAction);

            fromState._enterParameters = enterParameters;

            return fromState;
        }

        internal State addState(string name, PausableMonoBehaviour monoBehaviour, IEnumerator waiter, Delegate exitAction, State toState)
        {
            var fromState = addState();

            fromState._exitAction = exitAction;
            fromState.transitions.Add(new Transition(fromState, toState, waiter));

            return fromState;
        }

        internal State addState(Delegate enterAction, IEnumerator waiter, State toState)
        {
            var fromState = addState();

            fromState._enterAction = enterAction;
            fromState.transitions.Add(new Transition(fromState, toState, waiter));

            return fromState;
        }

        internal State addState(Delegate enterAction, Func<bool> endCondition, State toState)
        {
            var fromState = addState();

            fromState._enterAction = enterAction;
            fromState.transitions.Add(new Transition(fromState, toState, endCondition));

            return fromState;
        }

        internal State addState(Func<bool> endCondition, Delegate exitAction, State toState)
        {
            var fromState = addState();

            fromState._exitAction = exitAction;
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

        public bool MoveNext()
        {
            var fromState = transition._fromState;

            var more_elements_available = transition._waiter.MoveNext();

            if (!more_elements_available)
            {
                if (fromState._exitAction != null)
                    fromState._exitAction.DynamicInvoke(fromState._exitParameters);

                var next_state = transition._toState;

                if (callback_function != null)
                    callback_function.DynamicInvoke(next_state, fromState);
            }

            return more_elements_available;
        }

        public void Reset()
        {
            transition._waiter.Reset();
        }

        public object Current => transition._waiter.Current;
    }

    sealed class SMEnumerable : IEnumerable
    {
        private SMEnumerator smEnumerator;

        public SMEnumerable(Transition transition, Delegate callback_function)
        {
            smEnumerator = new SMEnumerator(transition, callback_function);
        }

        public IEnumerator GetEnumerator()
        {
            return smEnumerator;
        }
    }

}