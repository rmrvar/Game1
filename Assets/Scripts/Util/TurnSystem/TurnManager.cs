using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Util.TurnSystem
{
    public class TurnManager<T>
    {
        public event System.Action<TurnContext<T>> OnTurnStarted;
        public event System.Action<TurnContext<T>> OnTurnEnded;
        public event System.Action<TurnContext<T>> OnRoundStarted;
        public event System.Action<TurnContext<T>> OnRoundEnded;
        public event System.Action OnWaitingForTokens;

        public TurnManager(MonoBehaviour coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }
        
        #region --- Token Management ---
        public void AddToken(T token)
        {
            _tokens.Add(token);
        }

        public void RemToken(T token)
        {
            var index = _tokens.IndexOf(token);
            if (index == -1)
            {
                return;
            }
            _tokens.RemoveAt(index);
            if (index <= _tokenIndex)
            {
                --_tokenIndex;
            }
            // TODO: Should also cancel task if delete currently running index.
        }

        public void ClearTokens()
        {
            _tokens.Clear();
            _tokenIndex = -1;
        }
        #endregion
        
        #region --- Run Methods ---
        public bool IsRunning { get; private set; }
        
        public void Run()
        {
            if (IsRunning)
            {
                Debug.LogWarning("Turn Manager is already running.");
                return;
            }
            
            Startup();
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                Debug.LogWarning("Turn Manager is already stopped.");
                return;
            }
            
            Cleanup();
        }
        
        private void Cleanup()
        {
            IsRunning = false;
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
            _coroutineRunner.StopCoroutine(_coroutine);
            _coroutine = null;
        }

        private void Startup()
        {
            IsRunning = true;
            _tokenIndex = -1;
            _cts = new CancellationTokenSource();
            _coroutine = _coroutineRunner.StartCoroutine(IE_Run());
        }
        
        private IEnumerator IE_Run()
        {
            try
            {
                while (true)
                {
                    if (_tokens.Count == 0)
                    {
                        OnWaitingForTokens?.Invoke();
                        yield return null;
                        continue;
                    }

                    _tokenIndex = (_tokenIndex + 1) % _tokens.Count;

                    var context = new TurnContext<T>
                    {
                        Token = _tokens[_tokenIndex],
                        CoroutineRunner = _coroutineRunner
                    };

                    if (_tokenIndex == 0)
                    {
                        OnRoundStarted?.Invoke(context);
                    }
                
                    OnTurnStarted?.Invoke(context);
                    if (_tasks != null)
                    {
                        foreach (var task in _tasks)
                        {
                            if (task.ShouldSkip())
                            {
                                continue;
                            }
                            yield return task.IE_Execute(context, _cts.Token);
                        }   
                    }
                    OnTurnEnded?.Invoke(context);

                    if (_tokenIndex >= _tokens.Count - 1)
                    {
                        OnRoundEnded?.Invoke(context);
                        // Minimally invasive way to prevent hanging if every task immediately completes for some reason.
                        // yield return null;
                    }
                }
            }
            finally
            {
                // Won't be called if coroutine is stopped by calling Stop().
                Cleanup();
            }
        }
        #endregion

        // This can be called in OnTurnStarted to provide specific tasks depending on context (e.g., enemy vs player).
        public void SetTasks(ITurnTask<T>[] tasks)
        {
            _tasks = tasks;
        }
        
        private readonly MonoBehaviour _coroutineRunner;
        private ITurnTask<T>[] _tasks;
        private readonly List<T> _tokens = new();
        private int _tokenIndex;
        private CancellationTokenSource _cts;
        private Coroutine _coroutine;
    }
}
