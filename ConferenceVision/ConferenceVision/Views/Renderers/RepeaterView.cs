using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;


namespace ConferenceVision.Views.Renderers
{
	public delegate void RepeaterViewItemAddedEventHandler(object sender, RepeaterViewItemAddedEventArgs args);

	/// <summary>
	/// Don't set IsVisible to false or you will have a bad time
	/// this won't relayout its child elements when you change the visibility
	/// </summary>
	// in lieu of an actual Xamarin Forms ItemsControl, this is a heavily modified version of code from https://forums.xamarin.com/discussion/21635/xforms-needs-an-itemscontrol
	public class RepeaterView : FlexLayout
	{
		public RepeaterView()
		{
			Dictionary<Element, IDisposable> activatedViews
				= new Dictionary<Element, IDisposable>();
		}

		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create(
				"ItemsSource",
				typeof(IEnumerable),
				typeof(RepeaterView),
				defaultValue: null,
				defaultBindingMode: BindingMode.OneWay,
				propertyChanged: ItemsChanged);


		public static readonly BindableProperty ItemTemplateProperty =
			BindableProperty.Create(
				"ViewModel",
				typeof(DataTemplate),
				typeof(RepeaterView),
				defaultValue: null,
				defaultBindingMode: BindingMode.OneWay);


		bool waitingForBindingContext = false;
		public event RepeaterViewItemAddedEventHandler ItemCreated;

		public IEnumerable ItemsSource
		{
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public DataTemplate ItemTemplate
		{
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}


		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (BindingContext != null && waitingForBindingContext && ItemsSource != null)
			{
				ItemsChanged(this, null, ItemsSource);
			}
		}

		private static void ItemsChanged(BindableObject bindable, object old, object newVal)
		{
			IEnumerable oldValue = old as IEnumerable;
			IEnumerable newValue = newVal as IEnumerable;

			var control = (RepeaterView)bindable;

			var oldObservableCollection = oldValue as INotifyCollectionChanged;

			if (oldObservableCollection != null)
			{
				oldObservableCollection.CollectionChanged -= control.OnItemsSourceCollectionChanged;
			}

			//HACK:SHANE
			if (control.BindingContext == null)
			{
				control.waitingForBindingContext = true;
				//this means this control has been removed from the visual tree
				//so don't update it other wise you get random null reference exceptions
				return;
			}

			control.waitingForBindingContext = false;

			var newObservableCollection = newValue as INotifyCollectionChanged;

			if (newObservableCollection != null)
			{
				newObservableCollection.CollectionChanged += control.OnItemsSourceCollectionChanged;
			}

			try
			{
				control.Children.Clear();

				if (newValue != null)
				{
					foreach (var item in newValue)
					{
						var view = control.CreateChildViewFor(item);
						control.Children.Add(view);
						control.OnItemCreated(view);
					}
				}

				control.UpdateChildrenLayout();
				control.InvalidateLayout();
			}
			catch (NullReferenceException)
			{
				try
				{
					Debug.WriteLine(
						String.Format($"RepeaterView: NullReferenceException Parent:{control.Parent} ParentView:{control.Parent} IsVisible:{control.IsVisible}")
					);
				}
				catch (Exception exc)
				{
					Debug.WriteLine($"NullReferenceException Logging Failed {exc}");
				}
			}
		}



		protected virtual void OnItemCreated(View view)
		{

			if (this.ItemCreated != null)
			{
				ItemCreated.Invoke(this, new RepeaterViewItemAddedEventArgs(view, view.BindingContext));
			}


		}

		private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			try
			{
				var invalidate = false;

				List<View> createdViews = new List<View>();
				if (e.Action == NotifyCollectionChangedAction.Reset)
				{
					var list = sender as IEnumerable;


					this.Children.SyncList(
						list,
						(item) =>
						{
							var view = this.CreateChildViewFor(item);
							createdViews.Add(view);
							return view;
						}, (item, view) => view.BindingContext == item,
						null);

					foreach (View view in createdViews)
					{
						OnItemCreated(view);
					}

					invalidate = true;
				}

				if (e.OldItems != null)
				{
					this.Children.RemoveAt(e.OldStartingIndex);
					invalidate = true;
				}

				if (e.NewItems != null)
				{
					for (var i = 0; i < e.NewItems.Count; ++i)
					{
						var item = e.NewItems[i];
						var view = this.CreateChildViewFor(item);

						this.Children.Insert(i + e.NewStartingIndex, view);
						OnItemCreated(view);
					}

					invalidate = true;
				}

				if (invalidate)
				{
					this.UpdateChildrenLayout();
					this.InvalidateLayout();
				}
			}
			catch (NullReferenceException)
			{
				try
				{
					Debug.WriteLine(
						$"RepeaterView.OnItemsSourceCollectionChanged: NullReferenceException Parent:{Parent} ParentView:{Parent} IsVisible:{IsVisible} BindingContext: {BindingContext}"
					);
				}
				catch (Exception exc)
				{
					Debug.WriteLine($"OnItemsSourceCollectionChanged: NullReferenceException Logging Failed {exc}");
				}
			}
		}

		private View CreateChildViewFor(object item)
		{
			this.ItemTemplate.SetValue(BindableObject.BindingContextProperty, item);

			if (this.ItemTemplate is DataTemplateSelector)
			{
				var dts = (DataTemplateSelector)this.ItemTemplate;
				return (View)dts.SelectTemplate(item, null).CreateContent();
			}
			else
			{
				return (View)this.ItemTemplate.CreateContent();
			}
		}
	}

	public class RepeaterViewItemAddedEventArgs : EventArgs
	{
		private readonly View view;
		private readonly object model;

		public RepeaterViewItemAddedEventArgs(View view, object model)
		{
			this.view = view;
			this.model = model;
		}

		public View View { get { return view; } }

		public object Model { get { return model; } }
	}

	public static class IListMixIns
	{

		public static void SyncList<T>(
			this IList<T> This,
			IEnumerable<T> sourceList)
		{
			This.SyncList<T, T>(sourceList, x => x, (x, y) => x.Equals(y), null);
		}


		public static void SyncList<T>(
			this IList<T> This,
			IEnumerable sourceList,
			Func<object, T> selector,
			Func<object, T, bool> areEqual,
			Action<object, T> updateExisting,
			bool dontRemove = false)
		{
			var sourceListEnum = sourceList.OfType<Object>().ToList();

			//passengers
			foreach (T dest in This.ToList())
			{
				var match = sourceListEnum.FirstOrDefault(p => areEqual(p, dest));
				if (match != null)
				{
					if (updateExisting != null)
						updateExisting(match, dest);
				}
				else if (!dontRemove)
				{
					This.Remove(dest);
				}
			}

			sourceListEnum.Where(x => !This.Any(p => areEqual(x, p)))
				.ToList().ForEach(p =>
				{
					if (This.Count >= sourceListEnum.IndexOf(p))
						This.Insert(sourceListEnum.IndexOf(p), selector(p));
					else
					{
						var result = selector(p);
						if (!EqualityComparer<T>.Default.Equals(result, default(T)))
							This.Add(result);
					}
				});
		}

		public static bool IsEmpty<T>(this IEnumerable<T> list)
		{
			return !list.Any();
		}

		public static bool IsEmpty<T>(this Array list)
		{
			return list.Length == 0;
		}

		public static bool IsEmpty<T>(this List<T> list)
		{
			return list.Count == 0;
		}

		public static void ForEach<T>(this IEnumerable<T> list, Action<T> doMe)
		{
			foreach (var item in list)
			{
				doMe(item);
			}
		}

		public static void SyncList<T, Source>(
			this IList<T> This,
			IEnumerable<Source> sourceList,
			Func<Source, T> selector,
			Func<Source, T, bool> areEqual,
			Action<Source, T> updateExisting,
			bool dontRemove = false)
		{
			//passengers
			foreach (T dest in This.ToList())
			{
				var match = sourceList.FirstOrDefault(p => areEqual(p, dest));
				if (!EqualityComparer<Source>.Default.Equals(match, default(Source)))
				{
					updateExisting?.Invoke(match, dest);
				}
				else if (!dontRemove)
				{
					This.Remove(dest);
				}
			}

			sourceList.Where(x => !This.Any(p => areEqual(x, p)))
				.ToList().ForEach(p =>
				{
					var result = selector(p);
					if (!EqualityComparer<T>.Default.Equals(result, default(T)))
						This.Add(result);
				});
		}
	}

}