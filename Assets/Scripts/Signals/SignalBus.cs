using System;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.Signals
{
	public static class SignalBus
	{
		private static Dictionary<Type, SubscriptionsList> _subscriptions;

		static SignalBus()
		{
			_subscriptions = new Dictionary<Type, SubscriptionsList>();
		}

		public static void Subscribe<TSignal>(Action<TSignal> subscriber) where TSignal : BaseSignal
		{
			Type signalType = typeof(TSignal);

			if (!_subscriptions.ContainsKey(signalType))
				_subscriptions[signalType] = new SubscriptionsList();

			_subscriptions[signalType].Subscribe(subscriber);
		}

		public static void Unsubscribe<TSignal>(Action<TSignal> subscriber) where TSignal : BaseSignal
		{
			Type signalType = typeof(TSignal);
			if (_subscriptions.ContainsKey(signalType))
			{
				_subscriptions[signalType].Unsubscribe(subscriber);
			}
		}

		public static void Publish(BaseSignal sugnal)
		{
			Type signalType = sugnal.GetType();
			if (_subscriptions.ContainsKey(signalType))
			{
				_subscriptions[signalType].Publish(sugnal);
			}
			else
			{
				//Debug.LogWarning("No subscribers for the sugnal: " + signalType);
			}
		}

		private class SubscriptionsList
		{
			private bool needsCleanUp = false;

			private bool executing;

			public List<ISubscription> list = new List<ISubscription>();

			public void Subscribe<TSignal>(Action<TSignal> subscriber) where TSignal : BaseSignal
			{
				list.Add(new Subscription<Action<TSignal>>(subscriber));
			}

			public void Unsubscribe<TSignal>(Action<TSignal> subscriber) where TSignal : BaseSignal
			{
				int i = IndexOf(subscriber);
				if (i >= 0)
				{
					if (executing)
					{
						needsCleanUp = true;
						list[i] = null;
					}
					else
					{
						list.RemoveAt(i);
					}
				}
			}

			public int IndexOf<TSignal>(Action<TSignal> subscriber) where TSignal : BaseSignal
			{
				for (int i = 0; i < list.Count; i++)
				{
					if ((list[i] != null) && list[i].IsSubscriberEqual(subscriber))
					{
						return i;
					}
				}
				return -1;
			}

			public void Cleanup()
			{
				if (!needsCleanUp)
				{
					return;
				}

				list.RemoveAll(s => s == null);
				//list.RemoveAll(s => (s == null) || (s.Subscriber == null) || (s.Subscriber.Target == null));
				needsCleanUp = false;
			}

			public void Publish(BaseSignal signal)
			{
				executing = true;
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != null)
					{
						list[i]?.Publish(signal);
					}
				}
				executing = false;

				Cleanup();

				for (int i = 0; i < list.Count; i++)
				{
					list[i]?.Reset();
				}
			}
		}

		private interface ISubscription
		{
			//WeakReference Subscriber { get; }
			bool IsSubscriberEqual(object otherSubscriber);
			void Publish(BaseSignal signal);
			void Reset();
		}

		private class Subscription<T> : ISubscription
		{
			//private WeakReference subscriber;
			//public WeakReference Subscriber => subscriber;

			private T subscriber;

			private int publishesCount;

			public Subscription(T s)
			{
				//subscriber = new WeakReference(s);
				subscriber = s;
			}

			public bool IsSubscriberEqual(object otherSubscriber)
			{
				//return EqualityComparer<T>.Default.Equals((T)subscriber.Target, (T)otherSubscriber);
				return EqualityComparer<T>.Default.Equals(subscriber, (T)otherSubscriber);
			}

			public void Publish(BaseSignal signal)
			{
				if (publishesCount >= 10)
				{
					Debug.LogError("Cyclic call of subscribers for the signal: " + signal.GetType());
					return;
				}
				publishesCount++;
				//(subscriber.Target as Delegate).DynamicInvoke(signal);
				(subscriber as Delegate).DynamicInvoke(signal);
			}

			public void Reset()
			{
				publishesCount = 0;
			}
		}
	}
}
