using UnityEngine;
using System.Collections.Generic;
using System;

namespace SolarGames.Async
{
    /// <summary>
    /// Состояния задачи
    /// </summary>
    public enum Statuses
    {
        /// <summary>
        /// Завершить задачу успешно
        /// </summary>
        Next,
        NextImmediately,

        /// <summary>
        /// Завершить задачу с ошибкой
        /// </summary>
        Error,
        
        /// <summary>
        /// Продолжить исполнение задачи
        /// </summary>
        Wait
    }

    public delegate Statuses DContinueActionWithPath(ContinuesPath path);
    public delegate Statuses DContinueAction();
    public delegate void DContinueVoidAction();

    public delegate void DContinuesPathEvent(ContinuesPath path);

    /// <summary>
    /// Путь задач
    /// </summary>
    public class ContinuesPath
    {
        List<Delegate> actions;

        public event DContinuesPathEvent OnNext;
        public event DContinuesPathEvent OnSuccess;
        public event DContinuesPathEvent OnFinish;
        public event DContinuesPathEvent OnError;

        public float DeltaF
        {
            get
            {
                return Time.time - switchTime;
            }
        }

        float switchTime;

        public ContinuesPath()
        {
            actions = new List<Delegate>();
        }

        /// <summary>
        /// Количество задач в очереди
        /// </summary>
        /// <value>The count.</value>
        public int Count { get { return actions.Count; } }

        /// <summary>
        /// Добавить задачу в конец очереди
        /// </summary>
        /// <param name="action">Action.</param>
        public ContinuesPath Add(DContinueVoidAction action)
        {
            if (action != null)
                actions.Add(action);
            return this;
        }

        /// <summary>
        /// Добавить задачу в конец очереди
        /// </summary>
        /// <param name="action">Action.</param>
        public ContinuesPath Add(DContinueActionWithPath action)
        {
            UpdateSwitchTime();
            if (action != null)
                actions.Add(action);
            return this;
        }

        /// <summary>
        /// Добавить задачу в конец очереди
        /// </summary>
        /// <param name="action">Action.</param>
        public ContinuesPath Add(DContinueAction action)
        {
            UpdateSwitchTime();
            if (action != null)
                actions.Add(action);
            return this;
        }

        public ContinuesPath Add(IEnumerable<DContinueAction> actions)
        {
            UpdateSwitchTime();
            if (actions != null)
                foreach (DContinueAction a in actions)
                    this.actions.Add(a);
            return this;
        }

        public ContinuesPath Add(IEnumerable<DContinueActionWithPath> actions)
        {
            UpdateSwitchTime();
            if (actions != null)
                foreach (DContinueActionWithPath a in actions)
                    this.actions.Add(a);
            return this;
        }

        void UpdateSwitchTime()
        {
            if (actions.Count == 0)
                switchTime = Time.time;
        }

        /// <summary>
        /// Добавить задачу в начало очереди
        /// </summary>
        /// <param name="action">Action.</param>
        public ContinuesPath InsertFirst(DContinueAction action)
        {
            UpdateSwitchTime();
            actions.Insert(0, action);
            return this;
        }

        /// <summary>
        /// Добавить задачу в начало очереди
        /// </summary>
        /// <param name="action">Action.</param>
        public ContinuesPath InsertFirst(DContinueActionWithPath action)
        {
            actions.Insert(0, action);
            return this;
        }

        /// <summary>
        /// Бросить эту задачу и начать следующую
        /// </summary>
        public ContinuesPath Next()
        {
            if (actions.Count > 0)
            {
                actions.RemoveAt(0);
                OnNextInternal();
            }
            return this;
        }

        /// <summary>
        /// Остановить очередь задач
        /// </summary>
        public ContinuesPath Stop()
        {
            if (actions.Count > 0)
            {
                actions.Clear();

                if (actions.Count == 0 && OnFinish != null)
                    OnFinish(this);
            }
            return this;
        }

        void OnNextInternal()
        {
            if (actions.Count == 0)
                return;

            switchTime = Time.time;
            if (OnNext != null)
                OnNext(this);
        }

        /// <summary>
        /// Обновления состояния задачи
        /// </summary>
        public void Update()
        {
            if (actions.Count > 0)
            {
                Delegate d = actions[0];

                Statuses st = Statuses.Next;
                if (d.GetType() == typeof(DContinueActionWithPath))
                    st = ((DContinueActionWithPath)actions[0]).Invoke(this);
                else if (d.GetType() == typeof(DContinueAction))
                    st = ((DContinueAction)actions[0]).Invoke();
                else
                {
                    ((DContinueVoidAction)actions[0]).Invoke();
                    st = Statuses.Next;
                }
                bool error = false;
                switch (st)
                {
                    case Statuses.Next:
                        if (actions.Count > 0)
                            actions.RemoveAt(0);
                        OnNextInternal();
                        break;
                    case Statuses.NextImmediately:
                        if (actions.Count > 0)
                            actions.RemoveAt(0);
                        OnNextInternal();
                        if (actions.Count > 0)
                            Update();
                        break;
                    case Statuses.Error:
                        actions.Clear();
                        error = true;
                        if (OnError != null)
                            OnError(this);
                        break;
                    default:
                        break;
                }

                if (actions.Count == 0 && OnFinish != null)
                    OnFinish(this);
                if (actions.Count == 0 && OnSuccess != null && !error)
                    OnSuccess(this);
            }
        }
    }
}